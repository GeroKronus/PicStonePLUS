using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace PicStonePlus.SDK
{
    // EventArgs personalizados
    public class CameraEventArgs : EventArgs
    {
        public eNkMAIDEvent EventType { get; set; }
        public IntPtr Data { get; set; }
        public string Message { get; set; }
    }

    public class ImageReadyEventArgs : EventArgs
    {
        public byte[] ImageData { get; set; }
        public string FileName { get; set; }
        public bool IsRemoved { get; set; }
    }

    public class ProgressEventArgs : EventArgs
    {
        public uint Done { get; set; }
        public uint Total { get; set; }
        public int Percent => Total > 0 ? (int)(100 * Done / Total) : 0;
    }

    public class NikonManager : IDisposable
    {
        // Handle do módulo nativo carregado
        private IntPtr _hModule = IntPtr.Zero;
        // Entry point do SDK
        private MAIDEntryPointProc _entryPoint;
        // Delegates mantidos vivos via GCHandle
        private MAIDCompletionProc _completionProc;
        private MAIDEventProc _moduleEventProc;
        private MAIDEventProc _sourceEventProc;
        private MAIDEventProc _itemEventProc;
        private MAIDDataProc _dataProc;
        private MAIDProgressProc _progressProc;
        private MAIDUIRequestProc _uiRequestProc;

        // GCHandles para manter delegates vivos
        private List<GCHandle> _gcHandles = new List<GCHandle>();

        // Objetos do SDK
        private IntPtr _pModuleObject = IntPtr.Zero;
        private IntPtr _pSourceObject = IntPtr.Zero;

        // Capabilities
        private NkMAIDCapInfo[] _moduleCapabilities;
        private NkMAIDCapInfo[] _sourceCapabilities;
        private uint _moduleCapCount;
        private uint _sourceCapCount;

        // Estado
        private bool _isModuleLoaded;
        private bool _isModuleOpen;
        private bool _isSourceOpen;
        private bool _disposed;
        private uint _sourceId;

        // Serialização de acesso ao SDK
        private readonly object _sdkLock = new object();

        // Buffer para dados de imagem (usado pelo DataProc)
        private byte[] _imageBuffer;

        // IDs coletados durante EnumChildren
        private List<uint> _enumChildrenIds;

        // IDs de sources (câmeras) descobertas via AddChild event do módulo
        private List<uint> _moduleChildIds = new List<uint>();

        // Mapa de valores para capabilities Unsigned lidas como "enum" no UI
        // capId → lista ordenada de valores uint (índice do combo = índice nesta lista)
        private Dictionary<uint, List<uint>> _unsignedCapValues = new Dictionary<uint, List<uint>>();

        // Eventos
        public event EventHandler<CameraEventArgs> CameraEvent;
        public event EventHandler<ImageReadyEventArgs> ImageReady;
        public event EventHandler<ProgressEventArgs> Progress;
        public event EventHandler CameraDisconnected;

        // Propriedades
        public bool IsModuleLoaded => _isModuleLoaded;
        public bool IsConnected => _isSourceOpen;
        public string ModulePath { get; private set; }

        public NikonManager()
        {
            // Criar delegates e mantê-los vivos
            _completionProc = new MAIDCompletionProc(CompletionProcHandler);
            _moduleEventProc = new MAIDEventProc(ModuleEventProcHandler);
            _sourceEventProc = new MAIDEventProc(SourceEventProcHandler);
            _itemEventProc = new MAIDEventProc(ItemEventProcHandler);
            _progressProc = new MAIDProgressProc(ProgressProcHandler);
            _uiRequestProc = new MAIDUIRequestProc(UIRequestProcHandler);
            _dataProc = new MAIDDataProc(DataProcHandler);

            // Pin delegates
            PinDelegate(_completionProc);
            PinDelegate(_moduleEventProc);
            PinDelegate(_sourceEventProc);
            PinDelegate(_itemEventProc);
            PinDelegate(_progressProc);
            PinDelegate(_uiRequestProc);
            PinDelegate(_dataProc);
        }

        private void PinDelegate(Delegate d)
        {
            _gcHandles.Add(GCHandle.Alloc(d));
        }

        #region Module Load/Unload

        // Log de diagnóstico
        private static string _logPath = Path.Combine(
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? ".",
            "nikon_debug.log");

        private void Log(string message)
        {
            try
            {
                string line = $"[{DateTime.Now:HH:mm:ss.fff}] {message}\r\n";
                File.AppendAllText(_logPath, line);
            }
            catch { }
        }

        public bool LoadModule(string modulePath)
        {
            lock (_sdkLock)
            {
                if (_isModuleLoaded)
                    UnloadModuleInternal();

                ModulePath = modulePath;
                Log($"LoadModule: {modulePath}");

                if (!File.Exists(modulePath))
                {
                    Log("  FALHA: arquivo não existe");
                    return false;
                }

                _hModule = NativeMethods.LoadLibrary(modulePath);
                if (_hModule == IntPtr.Zero)
                {
                    int err = Marshal.GetLastWin32Error();
                    Log($"  FALHA: LoadLibrary retornou null, Win32 error={err}");
                    return false;
                }

                Log($"  LoadLibrary OK, handle=0x{_hModule.ToInt64():X}");

                IntPtr procAddress = NativeMethods.GetProcAddress(_hModule, "MAIDEntryPoint");
                if (procAddress == IntPtr.Zero)
                {
                    Log("  FALHA: GetProcAddress MAIDEntryPoint não encontrado");
                    NativeMethods.FreeLibrary(_hModule);
                    _hModule = IntPtr.Zero;
                    return false;
                }

                Log("  MAIDEntryPoint encontrado OK");
                _entryPoint = Marshal.GetDelegateForFunctionPointer<MAIDEntryPointProc>(procAddress);
                _isModuleLoaded = true;
                return true;
            }
        }

        public void UnloadModule()
        {
            lock (_sdkLock)
            {
                UnloadModuleInternal();
            }
        }

        private void UnloadModuleInternal()
        {
            Log("UnloadModule: início");

            if (_isSourceOpen)
            {
                Log("  CloseSource...");
                CloseSource();
                Log("  CloseSource OK");
            }

            if (_isModuleOpen)
            {
                Log("  CloseModule...");
                CloseModule();
                Log("  CloseModule OK");
            }

            if (_hModule != IntPtr.Zero)
            {
                Log("  FreeLibrary...");
                NativeMethods.FreeLibrary(_hModule);
                _hModule = IntPtr.Zero;
                Log("  FreeLibrary OK");
            }

            _entryPoint = null;
            _isModuleLoaded = false;
            _moduleChildIds.Clear();
            Log("UnloadModule: completo");
        }

        #endregion

        #region Entry Point Wrapper

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private int CallEntryPoint(IntPtr pObject, eNkMAIDCommand command, uint param,
            eNkMAIDDataType dataType, IntPtr data, IntPtr pfnComplete, IntPtr refComplete)
        {
            if (_entryPoint == null)
                return (int)eNkMAIDResult.kNkMAIDResult_NotSupported;

            try
            {
                return _entryPoint(pObject, (uint)command, param, (uint)dataType, data, pfnComplete, refComplete);
            }
            catch (AccessViolationException ex)
            {
                Log($"NATIVE CRASH em CallEntryPoint: cmd={command} cap=0x{param:X}: {ex.Message}");
                return (int)eNkMAIDResult.kNkMAIDResult_UnexpectedError;
            }
            catch (Exception ex)
            {
                Log($"Exceção em CallEntryPoint: cmd={command} cap=0x{param:X}: {ex.GetType().Name}: {ex.Message}");
                return (int)eNkMAIDResult.kNkMAIDResult_UnexpectedError;
            }
        }

        // Verifica se resultado é aceitável (NoError, Pending, OrphanedChildren)
        private bool IsResultOk(int result)
        {
            return result == (int)eNkMAIDResult.kNkMAIDResult_NoError ||
                   result == (int)eNkMAIDResult.kNkMAIDResult_Pending ||
                   result == (int)eNkMAIDResult.kNkMAIDResult_OrphanedChildren;
        }

        private bool CommandAsync(IntPtr pObject)
        {
            int result = CallEntryPoint(pObject, eNkMAIDCommand.kNkMAIDCommand_Async,
                0, eNkMAIDDataType.kNkMAIDDataType_Null, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            return IsResultOk(result);
        }

        // Wrapper com completion callback: chama entry point, espera conclusão.
        // Segue o padrão do SDK sample D7200: struct com count inline,
        // loop Async sem sleep, caller libera a struct após o loop.
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private bool CallWithCompletion(IntPtr pObject, eNkMAIDCommand command, uint param,
            eNkMAIDDataType dataType, IntPtr data, int maxWaitMs = 5000)
        {
            // Log apenas para CapStart (evitar flood)
            bool isCapStart = (command == eNkMAIDCommand.kNkMAIDCommand_CapStart);
            if (isCapStart)
                Log($"CallWithCompletion: cmd={command} cap=0x{param:X} maxWait={maxWaitMs}ms");

            IntPtr pRefCompletion = IntPtr.Zero;

            try
            {
                // Alocar struct RefCompletionProc com count inline (conforme SDK sample)
                pRefCompletion = Marshal.AllocHGlobal(Marshal.SizeOf<RefCompletionProc>());
                var refComp = new RefCompletionProc
                {
                    ulCount = 0,
                    nResult = 0,
                    pRef = IntPtr.Zero
                };
                Marshal.StructureToPtr(refComp, pRefCompletion, false);

                IntPtr completionPtr = Marshal.GetFunctionPointerForDelegate(_completionProc);

                if (isCapStart) Log("  CallEntryPoint...");
                int nResult = CallEntryPoint(pObject, command, param, dataType, data,
                    completionPtr, pRefCompletion);
                if (isCapStart) Log($"  CallEntryPoint result={(eNkMAIDResult)nResult}");

                if (!IsResultOk(nResult))
                {
                    Marshal.FreeHGlobal(pRefCompletion);
                    pRefCompletion = IntPtr.Zero;
                    return false;
                }

                // Aguardar completion com loop Async (sem sleep, conforme PicStone/SDK sample)
                // Usar Stopwatch para timeout preciso
                var sw = System.Diagnostics.Stopwatch.StartNew();
                int loopCount = 0;
                uint countValue = 0;
                while (countValue < 1 && sw.ElapsedMilliseconds < maxWaitMs)
                {
                    CommandAsync(pObject);
                    // Ler ulCount diretamente do offset 0 da struct
                    countValue = (uint)Marshal.ReadInt32(pRefCompletion);
                    loopCount++;

                    // Yield ocasional para não travar CPU (a cada 500 iterações)
                    if (loopCount % 500 == 0)
                    {
                        Thread.Sleep(0);
                        if (isCapStart && loopCount % 5000 == 0)
                            Log($"  Async loop: {loopCount} ({sw.ElapsedMilliseconds}ms)");
                    }
                }

                bool completionFired = (countValue >= 1);
                if (isCapStart) Log($"  Completion: fired={completionFired} loops={loopCount} time={sw.ElapsedMilliseconds}ms");

                if (completionFired)
                {
                    // Ler resultado do completion (offset 4)
                    int completionResult = Marshal.ReadInt32(pRefCompletion, 4);
                    Marshal.FreeHGlobal(pRefCompletion);
                    pRefCompletion = IntPtr.Zero;
                }
                else
                {
                    // Timeout: NÃO liberar pRefCompletion pois o completion pode
                    // ainda disparar e escrever na memória (leak intencional para segurança)
                    Log($"TIMEOUT em CallWithCompletion: cmd={command} cap=0x{param:X} após {sw.ElapsedMilliseconds}ms");
                    pRefCompletion = IntPtr.Zero; // Não tentar liberar no finally
                }

                return completionFired;
            }
            catch (AccessViolationException ex)
            {
                Log($"CallWithCompletion NATIVE CRASH: cmd={command} cap=0x{param:X}: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Log($"CallWithCompletion EXCEÇÃO: cmd={command} cap=0x{param:X}: {ex.GetType().Name}: {ex.Message}");
                return false;
            }
            finally
            {
                // Liberar se ainda não foi liberado (caso de erro antes do loop)
                if (pRefCompletion != IntPtr.Zero)
                {
                    try { Marshal.FreeHGlobal(pRefCompletion); } catch { }
                }
            }
        }

        // Versão que retorna o resultado específico do entry point (para Capture com erro detalhado)
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private eNkMAIDResult CallWithCompletionResult(IntPtr pObject, eNkMAIDCommand command, uint param,
            eNkMAIDDataType dataType, IntPtr data, int maxWaitMs = 5000)
        {
            IntPtr pRefCompletion = IntPtr.Zero;
            try
            {
                pRefCompletion = Marshal.AllocHGlobal(Marshal.SizeOf<RefCompletionProc>());
                var refComp = new RefCompletionProc { ulCount = 0, nResult = 0, pRef = IntPtr.Zero };
                Marshal.StructureToPtr(refComp, pRefCompletion, false);

                IntPtr completionPtr = Marshal.GetFunctionPointerForDelegate(_completionProc);

                Log($"CallWithCompletion: cmd={command} cap=0x{param:X} maxWait={maxWaitMs}ms");
                Log("  CallEntryPoint...");
                int nResult = CallEntryPoint(pObject, command, param, dataType, data,
                    completionPtr, pRefCompletion);
                Log($"  CallEntryPoint result={(eNkMAIDResult)nResult}");

                if (!IsResultOk(nResult))
                {
                    Marshal.FreeHGlobal(pRefCompletion);
                    pRefCompletion = IntPtr.Zero;
                    return (eNkMAIDResult)nResult;
                }

                var sw = System.Diagnostics.Stopwatch.StartNew();
                int loopCount = 0;
                uint countValue = 0;
                while (countValue < 1 && sw.ElapsedMilliseconds < maxWaitMs)
                {
                    CommandAsync(pObject);
                    countValue = (uint)Marshal.ReadInt32(pRefCompletion);
                    loopCount++;
                    if (loopCount % 500 == 0) Thread.Sleep(0);
                }

                Log($"  Completion: fired={countValue >= 1} loops={loopCount} time={sw.ElapsedMilliseconds}ms");

                if (countValue >= 1)
                {
                    Marshal.FreeHGlobal(pRefCompletion);
                    pRefCompletion = IntPtr.Zero;
                    return eNkMAIDResult.kNkMAIDResult_NoError;
                }
                else
                {
                    pRefCompletion = IntPtr.Zero; // leak intencional
                    return eNkMAIDResult.kNkMAIDResult_UnexpectedError;
                }
            }
            catch
            {
                return eNkMAIDResult.kNkMAIDResult_UnexpectedError;
            }
            finally
            {
                if (pRefCompletion != IntPtr.Zero)
                    try { Marshal.FreeHGlobal(pRefCompletion); } catch { }
            }
        }

        // CapGet com completion
        private bool CommandCapGet(IntPtr pObject, uint capId, eNkMAIDDataType dataType, IntPtr pData)
        {
            return CallWithCompletion(pObject, eNkMAIDCommand.kNkMAIDCommand_CapGet,
                capId, dataType, pData);
        }

        // CapSet com completion
        private bool CommandCapSet(IntPtr pObject, uint capId, eNkMAIDDataType dataType, IntPtr pData)
        {
            return CallWithCompletion(pObject, eNkMAIDCommand.kNkMAIDCommand_CapSet,
                capId, dataType, pData);
        }

        // CapGetArray com completion
        private bool CommandCapGetArray(IntPtr pObject, uint capId, eNkMAIDDataType dataType, IntPtr pData)
        {
            return CallWithCompletion(pObject, eNkMAIDCommand.kNkMAIDCommand_CapGetArray,
                capId, dataType, pData);
        }

        // CapStart com wait
        private bool CommandCapStart(IntPtr pObject, uint capId, int maxWaitMs = 30000)
        {
            return CallWithCompletion(pObject, eNkMAIDCommand.kNkMAIDCommand_CapStart,
                capId, eNkMAIDDataType.kNkMAIDDataType_Null, IntPtr.Zero, maxWaitMs);
        }

        // CapStart que retorna o código de resultado do entry point
        private eNkMAIDResult CommandCapStartWithResult(IntPtr pObject, uint capId, int maxWaitMs = 30000)
        {
            return CallWithCompletionResult(pObject, eNkMAIDCommand.kNkMAIDCommand_CapStart,
                capId, eNkMAIDDataType.kNkMAIDDataType_Null, IntPtr.Zero, maxWaitMs);
        }

        // EnumChildren command (conforme o wrapper que funciona)
        private bool CommandEnumChildren(IntPtr pObject)
        {
            return CallWithCompletion(pObject, eNkMAIDCommand.kNkMAIDCommand_EnumChildren,
                0, eNkMAIDDataType.kNkMAIDDataType_Null, IntPtr.Zero);
        }

        #endregion

        #region Module/Source Open/Close

        public bool OpenModule()
        {
            lock (_sdkLock)
            {
                if (!_isModuleLoaded || _isModuleOpen)
                    return false;

                // Alocar NkMAIDObject para módulo
                _pModuleObject = Marshal.AllocHGlobal(Marshal.SizeOf<NkMAIDObject>());
                var modObj = new NkMAIDObject
                {
                    ulType = 0,
                    ulID = 0,
                    refClient = IntPtr.Zero,
                    refModule = IntPtr.Zero
                };
                Marshal.StructureToPtr(modObj, _pModuleObject, false);

                // Open module
                Log("OpenModule: chamando Open...");
                int result = CallEntryPoint(IntPtr.Zero, eNkMAIDCommand.kNkMAIDCommand_Open,
                    0, eNkMAIDDataType.kNkMAIDDataType_ObjectPtr, _pModuleObject, IntPtr.Zero, IntPtr.Zero);

                Log($"  Open result={(eNkMAIDResult)result}");
                if (result != (int)eNkMAIDResult.kNkMAIDResult_NoError)
                {
                    Marshal.FreeHGlobal(_pModuleObject);
                    _pModuleObject = IntPtr.Zero;
                    return false;
                }

                _isModuleOpen = true;

                // Enumerar capabilities do módulo
                Log("  EnumCapabilities...");
                if (!EnumCapabilities(_pModuleObject, out _moduleCapCount, out _moduleCapabilities))
                {
                    Log($"  FALHA: EnumCapabilities");
                    return false;
                }
                Log($"  Caps encontradas: {_moduleCapCount}");

                // Registrar callbacks no módulo
                SetCallbacksOnObject(_pModuleObject, _moduleCapabilities, _moduleEventProc);
                Log("  Callbacks registrados");

                // Definir ModuleMode como Controller
                SetModuleMode();
                Log("  ModuleMode set OK");

                return true;
            }
        }

        private void SetModuleMode()
        {
            if (!HasCapability(_moduleCapabilities, (uint)eNkMAIDCapability.kNkMAIDCapability_ModuleMode, eNkMAIDCapOperation.kNkMAIDCapOperation_Set))
                return;

            CommandCapSet(_pModuleObject, (uint)eNkMAIDCapability.kNkMAIDCapability_ModuleMode,
                eNkMAIDDataType.kNkMAIDDataType_Unsigned,
                (IntPtr)(uint)eNkMAIDModuleMode.kNkMAIDModuleMode_Controller);
        }

        public bool ConnectCamera()
        {
            lock (_sdkLock)
            {
                if (!_isModuleOpen)
                    return false;

                // Limpar lista de children descobertos por evento
                _moduleChildIds.Clear();

                Log("ConnectCamera: bombeando Async para descobrir câmera...");

                // Bombear Async para dar tempo ao SDK descobrir câmeras via USB.
                // O módulo dispara AddChild event quando encontra uma câmera.
                // Também tentamos GetChildrenIds como fallback.
                List<uint> sourceIds = null;
                for (int attempt = 0; attempt < 30; attempt++)
                {
                    bool asyncOk = CommandAsync(_pModuleObject);
                    Thread.Sleep(100);

                    // Verificar se recebemos AddChild event
                    if (_moduleChildIds.Count > 0)
                    {
                        sourceIds = new List<uint>(_moduleChildIds);
                        Log($"  Câmera encontrada via AddChild! IDs: {string.Join(",", sourceIds)} (attempt {attempt})");
                        break;
                    }

                    // Fallback: tentar query direta de children
                    sourceIds = GetChildrenIds(_pModuleObject);
                    if (sourceIds != null && sourceIds.Count > 0)
                    {
                        Log($"  Câmera encontrada via GetChildren! IDs: {string.Join(",", sourceIds)} (attempt {attempt})");
                        break;
                    }

                    if (attempt % 10 == 9)
                        Log($"  Tentativa {attempt + 1}/30, asyncOk={asyncOk}, childEvents={_moduleChildIds.Count}");
                }

                if (sourceIds == null || sourceIds.Count == 0)
                {
                    Log("  FALHA: nenhuma câmera encontrada após 30 tentativas");
                    return false;
                }

                _sourceId = sourceIds[0];
                Log($"  Abrindo source ID={_sourceId}...");

                // Alocar NkMAIDObject para source
                _pSourceObject = Marshal.AllocHGlobal(Marshal.SizeOf<NkMAIDObject>());
                var srcObj = new NkMAIDObject
                {
                    ulType = 0,
                    ulID = 0,
                    refClient = IntPtr.Zero,
                    refModule = IntPtr.Zero
                };
                Marshal.StructureToPtr(srcObj, _pSourceObject, false);

                // Open source
                int result = CallEntryPoint(_pModuleObject, eNkMAIDCommand.kNkMAIDCommand_Open,
                    _sourceId, eNkMAIDDataType.kNkMAIDDataType_ObjectPtr, _pSourceObject, IntPtr.Zero, IntPtr.Zero);

                Log($"  Open source result={(eNkMAIDResult)result}");
                if (result != (int)eNkMAIDResult.kNkMAIDResult_NoError)
                {
                    Marshal.FreeHGlobal(_pSourceObject);
                    _pSourceObject = IntPtr.Zero;
                    return false;
                }

                _isSourceOpen = true;

                // Enumerar capabilities da source
                Log("  EnumCapabilities source...");
                if (!EnumCapabilities(_pSourceObject, out _sourceCapCount, out _sourceCapabilities))
                {
                    Log("  FALHA: EnumCapabilities source");
                    return false;
                }
                Log($"  Source caps: {_sourceCapCount}");

                // Registrar callbacks na source (incluindo EventProc, ProgressProc, UIRequestProc)
                SetCallbacksOnObject(_pSourceObject, _sourceCapabilities, _sourceEventProc);
                Log("  ConnectCamera OK!");

                return true;
            }
        }

        public void Disconnect()
        {
            lock (_sdkLock)
            {
                if (_isSourceOpen)
                    CloseSource();
            }
        }

        private void CloseSource()
        {
            if (_pSourceObject != IntPtr.Zero)
            {
                ResetCallbacks(_pSourceObject, _sourceCapabilities);

                CallEntryPoint(_pSourceObject, eNkMAIDCommand.kNkMAIDCommand_Close,
                    0, eNkMAIDDataType.kNkMAIDDataType_Null, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

                Marshal.FreeHGlobal(_pSourceObject);
                _pSourceObject = IntPtr.Zero;
            }
            _isSourceOpen = false;
            _sourceCapabilities = null;
            _sourceCapCount = 0;
        }

        private void CloseModule()
        {
            if (_pModuleObject != IntPtr.Zero)
            {
                ResetCallbacks(_pModuleObject, _moduleCapabilities);

                CallEntryPoint(_pModuleObject, eNkMAIDCommand.kNkMAIDCommand_Close,
                    0, eNkMAIDDataType.kNkMAIDDataType_Null, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

                Marshal.FreeHGlobal(_pModuleObject);
                _pModuleObject = IntPtr.Zero;
            }
            _isModuleOpen = false;
            _moduleCapabilities = null;
            _moduleCapCount = 0;
        }

        #endregion

        #region Capabilities

        private bool EnumCapabilities(IntPtr pObject, out uint capCount, out NkMAIDCapInfo[] capArray)
        {
            capCount = 0;
            capArray = null;

            // GetCapCount
            IntPtr pCapCount = Marshal.AllocHGlobal(sizeof(uint));
            Marshal.WriteInt32(pCapCount, 0);

            if (!CallWithCompletion(pObject, eNkMAIDCommand.kNkMAIDCommand_GetCapCount,
                0, eNkMAIDDataType.kNkMAIDDataType_UnsignedPtr, pCapCount))
            {
                Marshal.FreeHGlobal(pCapCount);
                return false;
            }

            capCount = (uint)Marshal.ReadInt32(pCapCount);
            Marshal.FreeHGlobal(pCapCount);

            if (capCount == 0)
                return true;

            // GetCapInfo
            int capInfoSize = Marshal.SizeOf<NkMAIDCapInfo>();
            IntPtr pCapArray = Marshal.AllocHGlobal((int)(capCount * capInfoSize));

            if (!CallWithCompletion(pObject, eNkMAIDCommand.kNkMAIDCommand_GetCapInfo,
                capCount, eNkMAIDDataType.kNkMAIDDataType_CapInfoPtr, pCapArray))
            {
                Marshal.FreeHGlobal(pCapArray);
                return false;
            }

            capArray = new NkMAIDCapInfo[capCount];
            for (uint i = 0; i < capCount; i++)
            {
                IntPtr pCap = IntPtr.Add(pCapArray, (int)(i * capInfoSize));
                capArray[i] = Marshal.PtrToStructure<NkMAIDCapInfo>(pCap);
            }

            Marshal.FreeHGlobal(pCapArray);
            return true;
        }

        private bool HasCapability(NkMAIDCapInfo[] caps, uint capId, eNkMAIDCapOperation operation)
        {
            if (caps == null) return false;
            for (int i = 0; i < caps.Length; i++)
            {
                if (caps[i].ulID == capId)
                    return (caps[i].ulOperations & (uint)operation) != 0;
            }
            return false;
        }

        private uint GetCapabilityType(NkMAIDCapInfo[] caps, uint capId)
        {
            if (caps == null) return 0;
            for (int i = 0; i < caps.Length; i++)
            {
                if (caps[i].ulID == capId)
                    return caps[i].ulType;
            }
            return 0;
        }

        #endregion

        #region Callbacks Registration

        /// <summary>
        /// Registra EventProc, ProgressProc, UIRequestProc em um objeto
        /// </summary>
        private void SetCallbacksOnObject(IntPtr pObject, NkMAIDCapInfo[] caps, MAIDEventProc eventProc)
        {
            // EventProc
            if (HasCapability(caps, (uint)eNkMAIDCapability.kNkMAIDCapability_EventProc, eNkMAIDCapOperation.kNkMAIDCapOperation_Set))
            {
                SetCallbackCapability(pObject, (uint)eNkMAIDCapability.kNkMAIDCapability_EventProc, eventProc);
            }

            // ProgressProc
            if (HasCapability(caps, (uint)eNkMAIDCapability.kNkMAIDCapability_ProgressProc, eNkMAIDCapOperation.kNkMAIDCapOperation_Set))
            {
                SetCallbackCapability(pObject, (uint)eNkMAIDCapability.kNkMAIDCapability_ProgressProc, _progressProc);
            }

            // UIRequestProc
            if (HasCapability(caps, (uint)eNkMAIDCapability.kNkMAIDCapability_UIRequestProc, eNkMAIDCapOperation.kNkMAIDCapOperation_Set))
            {
                SetCallbackCapability(pObject, (uint)eNkMAIDCapability.kNkMAIDCapability_UIRequestProc, _uiRequestProc);
            }
        }

        /// <summary>
        /// Registra DataProc em um objeto (usado para DataObj)
        /// </summary>
        private void SetDataProcOnObject(IntPtr pObject, NkMAIDCapInfo[] caps)
        {
            if (HasCapability(caps, (uint)eNkMAIDCapability.kNkMAIDCapability_DataProc, eNkMAIDCapOperation.kNkMAIDCapOperation_Set))
            {
                SetCallbackCapability(pObject, (uint)eNkMAIDCapability.kNkMAIDCapability_DataProc, _dataProc);
            }
        }

        private void SetCallbackCapability(IntPtr pObject, uint capId, Delegate proc)
        {
            var callback = new NkMAIDCallback
            {
                pProc = Marshal.GetFunctionPointerForDelegate(proc),
                refProc = IntPtr.Zero
            };
            IntPtr pCallback = Marshal.AllocHGlobal(Marshal.SizeOf<NkMAIDCallback>());
            Marshal.StructureToPtr(callback, pCallback, false);

            CommandCapSet(pObject, capId, eNkMAIDDataType.kNkMAIDDataType_CallbackPtr, pCallback);

            Marshal.FreeHGlobal(pCallback);
        }

        private void ResetCallbacks(IntPtr pObject, NkMAIDCapInfo[] caps)
        {
            if (caps == null) return;

            uint[] callbackCaps = {
                (uint)eNkMAIDCapability.kNkMAIDCapability_EventProc,
                (uint)eNkMAIDCapability.kNkMAIDCapability_ProgressProc,
                (uint)eNkMAIDCapability.kNkMAIDCapability_UIRequestProc,
            };

            foreach (uint capId in callbackCaps)
            {
                if (HasCapability(caps, capId, eNkMAIDCapOperation.kNkMAIDCapOperation_Set))
                {
                    var callback = new NkMAIDCallback
                    {
                        pProc = IntPtr.Zero,
                        refProc = IntPtr.Zero
                    };
                    IntPtr pCallback = Marshal.AllocHGlobal(Marshal.SizeOf<NkMAIDCallback>());
                    Marshal.StructureToPtr(callback, pCallback, false);

                    CommandCapSet(pObject, capId,
                        eNkMAIDDataType.kNkMAIDDataType_CallbackPtr, pCallback);

                    Marshal.FreeHGlobal(pCallback);
                }
            }
        }

        #endregion

        #region Callback Handlers

        private void CompletionProcHandler(IntPtr pObject, uint ulCommand, uint ulParam,
            uint ulDataType, IntPtr data, IntPtr refComplete, int nResult)
        {
            // Conforme SDK sample D7200: apenas incrementar count e gravar result.
            // NÃO liberar refComplete aqui - o caller (CallWithCompletion) libera após o loop.
            if (refComplete != IntPtr.Zero)
            {
                try
                {
                    // ulCount está no offset 0 da struct RefCompletionProc
                    uint current = (uint)Marshal.ReadInt32(refComplete);
                    Marshal.WriteInt32(refComplete, (int)(current + 1));
                    // nResult está no offset 4
                    Marshal.WriteInt32(refComplete, 4, nResult);
                }
                catch { }
            }
        }

        private void ModuleEventProcHandler(IntPtr refProc, uint ulEvent, IntPtr data)
        {
            var eventType = (eNkMAIDEvent)ulEvent;
            Log($"ModuleEvent: {eventType} data=0x{data.ToInt64():X}");

            var args = new CameraEventArgs
            {
                EventType = eventType,
                Data = data,
                Message = $"Module Event: {eventType}"
            };

            switch (eventType)
            {
                case eNkMAIDEvent.kNkMAIDEvent_AddChild:
                    // Câmera descoberta pelo módulo
                    uint sourceId = (uint)data.ToInt64();
                    _moduleChildIds.Add(sourceId);
                    Log($"  AddChild! Source ID={sourceId}");
                    args.Message = $"Câmera descoberta (Source ID: {sourceId})";
                    break;

                case eNkMAIDEvent.kNkMAIDEvent_RemoveChild:
                    // Câmera desconectada
                    uint removedId = (uint)data.ToInt64();
                    Log($"  RemoveChild! Source ID={removedId}");
                    args.Message = "Câmera desconectada";
                    _isSourceOpen = false;
                    _pSourceObject = IntPtr.Zero;
                    _sourceCapabilities = null;
                    _sourceCapCount = 0;
                    CameraDisconnected?.Invoke(this, EventArgs.Empty);
                    break;

                case eNkMAIDEvent.kNkMAIDEvent_CapChange:
                case eNkMAIDEvent.kNkMAIDEvent_CapChangeOperationOnly:
                    EnumCapabilities(_pModuleObject, out _moduleCapCount, out _moduleCapabilities);
                    break;
            }

            CameraEvent?.Invoke(this, args);
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private void SourceEventProcHandler(IntPtr refProc, uint ulEvent, IntPtr data)
        {
            var eventType = (eNkMAIDEvent)ulEvent;
            Log($"SourceEvent: {eventType} data=0x{data.ToInt64():X}");

            var args = new CameraEventArgs
            {
                EventType = eventType,
                Data = data,
                Message = $"Source Event: {eventType}"
            };

            try
            {
                switch (eventType)
                {
                    case eNkMAIDEvent.kNkMAIDEvent_AddChild:
                    case eNkMAIDEvent.kNkMAIDEvent_AddChildInCard:
                        // Processar item INLINE (conforme wrapper que funciona)
                        uint newItemId = (uint)data.ToInt64();
                        args.Message = $"Nova imagem (ID: {newItemId})";
                        Log($"  AddChild Item ID={newItemId}, processando inline...");
                        try
                        {
                            HandleAddChildItem(newItemId);
                            Log($"  AddChild Item ID={newItemId} processado OK");
                        }
                        catch (Exception ex)
                        {
                            Log($"  HandleAddChildItem ERRO: {ex.GetType().Name}: {ex.Message}");
                        }
                        break;

                    case eNkMAIDEvent.kNkMAIDEvent_CaptureComplete:
                        Log("  CaptureComplete");
                        args.Message = "Captura completa";
                        break;

                    case eNkMAIDEvent.kNkMAIDEvent_CapChange:
                    case eNkMAIDEvent.kNkMAIDEvent_CapChangeOperationOnly:
                        EnumCapabilities(_pSourceObject, out _sourceCapCount, out _sourceCapabilities);
                        break;
                }
            }
            catch (AccessViolationException ex)
            {
                Log($"SourceEvent NATIVE CRASH: {eventType}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log($"SourceEvent EXCEÇÃO: {eventType}: {ex.GetType().Name}: {ex.Message}");
            }

            try { CameraEvent?.Invoke(this, args); } catch { }
        }

        private void ItemEventProcHandler(IntPtr refProc, uint ulEvent, IntPtr data)
        {
            var eventType = (eNkMAIDEvent)ulEvent;

            // Coletar IDs de children durante EnumChildren
            if (eventType == eNkMAIDEvent.kNkMAIDEvent_AddChild && _enumChildrenIds != null)
            {
                _enumChildrenIds.Add((uint)data.ToInt64());
            }
        }

        private void ProgressProcHandler(uint ulCommand, uint ulParam, IntPtr refProc, uint ulDone, uint ulTotal)
        {
            Progress?.Invoke(this, new ProgressEventArgs { Done = ulDone, Total = ulTotal });
        }

        private uint UIRequestProcHandler(IntPtr refProc, IntPtr pUIRequest)
        {
            return 0;
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private int DataProcHandler(IntPtr refClient, IntPtr pDataInfo, IntPtr pData)
        {
            try
            {
                var dataInfo = Marshal.PtrToStructure<NkMAIDDataInfo>(pDataInfo);
                Log($"DataProc: type=0x{dataInfo.ulType:X}");

                // File data (inclui combinações File|Image, File|Thumbnail, etc.)
                if ((dataInfo.ulType & (uint)eNkMAIDDataObjType.kNkMAIDDataObjType_File) != 0)
                {
                    var fileInfo = Marshal.PtrToStructure<NkMAIDFileInfo>(pDataInfo);
                    Log($"DataProc: File total={fileInfo.ulTotalLength} start={fileInfo.ulStart} len={fileInfo.ulLength} type={fileInfo.ulFileDataType}");

                    // Inicializar buffer no primeiro chunk (ulStart == 0)
                    if (fileInfo.ulStart == 0)
                    {
                        _imageBuffer = new byte[fileInfo.ulTotalLength];
                    }

                    if (_imageBuffer != null && pData != IntPtr.Zero && fileInfo.ulLength > 0)
                    {
                        // Usar ulStart como offset (conforme wrapper que funciona)
                        int offset = (int)fileInfo.ulStart;
                        int length = (int)fileInfo.ulLength;

                        if (offset + length <= _imageBuffer.Length)
                        {
                            Marshal.Copy(pData, _imageBuffer, offset, length);
                        }

                        // Verificar se transferência está completa
                        bool complete = (fileInfo.ulTotalLength == fileInfo.ulStart + fileInfo.ulLength);

                        if (complete)
                        {
                            string ext = ".dat";
                            switch ((eNkMAIDFileDataType)fileInfo.ulFileDataType)
                            {
                                case eNkMAIDFileDataType.kNkMAIDFileDataType_JPEG: ext = ".jpg"; break;
                                case eNkMAIDFileDataType.kNkMAIDFileDataType_TIFF: ext = ".tif"; break;
                                case eNkMAIDFileDataType.kNkMAIDFileDataType_NIF: ext = ".nef"; break;
                            }

                            Log($"DataProc: Imagem completa! {fileInfo.ulTotalLength} bytes, ext={ext}");
                            byte[] completedImage = _imageBuffer;
                            _imageBuffer = null;

                            ImageReady?.Invoke(this, new ImageReadyEventArgs
                            {
                                ImageData = completedImage,
                                FileName = $"Image{ext}",
                                IsRemoved = fileInfo.fRemoveObject != 0
                            });
                        }
                    }
                }
                // Thumbnail sem File flag
                else if ((dataInfo.ulType & (uint)eNkMAIDDataObjType.kNkMAIDDataObjType_Thumbnail) != 0)
                {
                    Log("DataProc: Thumbnail (ignorando)");
                }

                return (int)eNkMAIDResult.kNkMAIDResult_NoError;
            }
            catch (AccessViolationException ex)
            {
                Log($"DataProc NATIVE CRASH: {ex.Message}");
                return (int)eNkMAIDResult.kNkMAIDResult_UnexpectedError;
            }
            catch (Exception ex)
            {
                Log($"DataProc ERRO: {ex.GetType().Name}: {ex.Message}");
                return (int)eNkMAIDResult.kNkMAIDResult_UnexpectedError;
            }
        }

        #endregion

        #region Image Acquisition Pipeline (conforme wrapper que funciona)

        /// <summary>
        /// Processa um novo Item capturado. Chamado INLINE durante AddChild event.
        /// Fluxo: Open Item → EnumChildren → para cada DataObj: Open → Acquire → Close → Close Item
        /// </summary>
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private void HandleAddChildItem(uint itemId)
        {
            IntPtr pItemObject = IntPtr.Zero;
            try
            {
                Log($"HandleAddChildItem: itemId={itemId}, abrindo...");
                // Alocar e abrir o Item
                pItemObject = Marshal.AllocHGlobal(Marshal.SizeOf<NkMAIDObject>());
                var itemObj = new NkMAIDObject
                {
                    ulType = 0,
                    ulID = 0,
                    refClient = IntPtr.Zero,
                    refModule = IntPtr.Zero
                };
                Marshal.StructureToPtr(itemObj, pItemObject, false);

                int result = CallEntryPoint(_pSourceObject, eNkMAIDCommand.kNkMAIDCommand_Open,
                    itemId, eNkMAIDDataType.kNkMAIDDataType_ObjectPtr, pItemObject,
                    IntPtr.Zero, IntPtr.Zero);

                Log($"HandleAddChildItem: Open result={(eNkMAIDResult)result}");
                if (!IsResultOk(result))
                    return;

                // Enumerar capabilities do Item
                uint itemCapCount;
                NkMAIDCapInfo[] itemCaps;
                EnumCapabilities(pItemObject, out itemCapCount, out itemCaps);
                Log($"HandleAddChildItem: itemCaps={itemCapCount}");

                // Registrar EventProc no Item (para coletar DataObj IDs durante EnumChildren)
                SetCallbacksOnObject(pItemObject, itemCaps, _itemEventProc);

                // Usar EnumChildren (conforme wrapper que funciona) em vez de GetChildrenIds
                _enumChildrenIds = new List<uint>();
                CommandEnumChildren(pItemObject);
                List<uint> dataIds = new List<uint>(_enumChildrenIds);
                _enumChildrenIds = null;
                Log($"HandleAddChildItem: dataObj IDs=[{string.Join(",", dataIds)}]");

                // Para cada DataObj: abrir, registrar DataProc, executar Acquire, fechar
                foreach (uint dataId in dataIds)
                {
                    Log($"HandleAddChildItem: AcquireDataObject dataId={dataId}...");
                    AcquireDataObject(pItemObject, dataId);
                    Log($"HandleAddChildItem: AcquireDataObject dataId={dataId} OK");
                }

                // Fechar Item
                Log("HandleAddChildItem: fechando item...");
                CallEntryPoint(pItemObject, eNkMAIDCommand.kNkMAIDCommand_Close,
                    0, eNkMAIDDataType.kNkMAIDDataType_Null, IntPtr.Zero,
                    IntPtr.Zero, IntPtr.Zero);
                Log("HandleAddChildItem: COMPLETO");
            }
            catch (AccessViolationException ex)
            {
                Log($"HandleAddChildItem NATIVE CRASH: {ex.Message}\n{ex.StackTrace}");
            }
            catch (Exception ex)
            {
                Log($"HandleAddChildItem ERRO: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                if (pItemObject != IntPtr.Zero)
                    Marshal.FreeHGlobal(pItemObject);
            }
        }

        /// <summary>
        /// Abre um DataObj, registra DataProc, executa Acquire, fecha DataObj
        /// </summary>
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private void AcquireDataObject(IntPtr pItemObject, uint dataId)
        {
            IntPtr pDataObject = IntPtr.Zero;
            try
            {
                Log($"AcquireDataObject: dataId={dataId}, abrindo...");
                // Alocar e abrir o DataObj
                pDataObject = Marshal.AllocHGlobal(Marshal.SizeOf<NkMAIDObject>());
                var dataObj = new NkMAIDObject
                {
                    ulType = 0,
                    ulID = 0,
                    refClient = IntPtr.Zero,
                    refModule = IntPtr.Zero
                };
                Marshal.StructureToPtr(dataObj, pDataObject, false);

                int result = CallEntryPoint(pItemObject, eNkMAIDCommand.kNkMAIDCommand_Open,
                    dataId, eNkMAIDDataType.kNkMAIDDataType_ObjectPtr, pDataObject,
                    IntPtr.Zero, IntPtr.Zero);

                Log($"AcquireDataObject: Open result={(eNkMAIDResult)result}");
                if (!IsResultOk(result))
                    return;

                // Enumerar capabilities do DataObj
                uint dataCapCount;
                NkMAIDCapInfo[] dataCaps;
                EnumCapabilities(pDataObject, out dataCapCount, out dataCaps);
                Log($"AcquireDataObject: dataCaps={dataCapCount}");

                // Registrar callbacks (EventProc, ProgressProc, UIRequestProc)
                SetCallbacksOnObject(pDataObject, dataCaps, _itemEventProc);

                // Registrar DataProc (essencial para receber dados!)
                SetDataProcOnObject(pDataObject, dataCaps);

                // Resetar buffer
                _imageBuffer = null;

                // Executar Acquire
                Log("AcquireDataObject: Acquire...");
                try
                {
                    bool acqOk = CommandCapStart(pDataObject, (uint)eNkMAIDCapability.kNkMAIDCapability_Acquire);
                    Log($"AcquireDataObject: Acquire resultado={acqOk}");
                }
                catch (Exception ex)
                {
                    Log($"AcquireDataObject: Acquire ERRO: {ex.GetType().Name}: {ex.Message}");
                }

                // Fechar DataObj
                Log("AcquireDataObject: fechando...");
                CallEntryPoint(pDataObject, eNkMAIDCommand.kNkMAIDCommand_Close,
                    0, eNkMAIDDataType.kNkMAIDDataType_Null, IntPtr.Zero,
                    IntPtr.Zero, IntPtr.Zero);
                Log("AcquireDataObject: COMPLETO");
            }
            catch (AccessViolationException ex)
            {
                Log($"AcquireDataObject NATIVE CRASH: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log($"AcquireDataObject ERRO: {ex.GetType().Name}: {ex.Message}");
            }
            finally
            {
                if (pDataObject != IntPtr.Zero)
                    Marshal.FreeHGlobal(pDataObject);
            }
        }

        #endregion

        #region Children

        private List<uint> GetChildrenIds(IntPtr pObject)
        {
            IntPtr pEnum = Marshal.AllocHGlobal(Marshal.SizeOf<NkMAIDEnum>());

            if (!CommandCapGet(pObject, (uint)eNkMAIDCapability.kNkMAIDCapability_Children,
                eNkMAIDDataType.kNkMAIDDataType_EnumPtr, pEnum))
            {
                Marshal.FreeHGlobal(pEnum);
                return null;
            }

            var stEnum = Marshal.PtrToStructure<NkMAIDEnum>(pEnum);

            if (stEnum.ulElements == 0)
            {
                Marshal.FreeHGlobal(pEnum);
                return new List<uint>();
            }

            stEnum.pData = Marshal.AllocHGlobal((int)(stEnum.ulElements * stEnum.wPhysicalBytes));
            Marshal.StructureToPtr(stEnum, pEnum, false);

            CommandCapGetArray(pObject, (uint)eNkMAIDCapability.kNkMAIDCapability_Children,
                eNkMAIDDataType.kNkMAIDDataType_EnumPtr, pEnum);

            var ids = new List<uint>();
            for (uint i = 0; i < stEnum.ulElements; i++)
            {
                uint id = (uint)Marshal.ReadInt32(stEnum.pData, (int)(i * 4));
                ids.Add(id);
            }

            Marshal.FreeHGlobal(stEnum.pData);
            Marshal.FreeHGlobal(pEnum);
            return ids;
        }

        #endregion

        #region Camera Properties

        public string GetCameraName()
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return "Não conectado";
                return GetStringCapability(_pSourceObject, (uint)eNkMAIDCapability.kNkMAIDCapability_Name);
            }
        }

        public int GetBatteryLevel()
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return -1;
                return GetIntegerCapability(_pSourceObject, (uint)eNkMAIDCapability.kNkMAIDCapability_BatteryLevel);
            }
        }

        public string GetLensInfo()
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return "";
                return GetStringCapability(_pSourceObject, (uint)eNkMAIDCapability.kNkMAIDCapability_LensInfo);
            }
        }

        public bool GetIsAlive()
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return false;
                IntPtr pBool = Marshal.AllocHGlobal(sizeof(int));
                Marshal.WriteInt32(pBool, 0);
                bool ok = CommandCapGet(_pSourceObject, (uint)eNkMAIDCapability.kNkMAIDCapability_IsAlive,
                    eNkMAIDDataType.kNkMAIDDataType_BooleanPtr, pBool);
                int val = Marshal.ReadInt32(pBool);
                Marshal.FreeHGlobal(pBool);
                return ok && val != 0;
            }
        }

        private string GetStringCapability(IntPtr pObject, uint capId)
        {
            IntPtr pString = Marshal.AllocHGlobal(Marshal.SizeOf<NkMAIDString>());
            bool ok = CommandCapGet(pObject, capId, eNkMAIDDataType.kNkMAIDDataType_StringPtr, pString);
            string result = "";
            if (ok)
            {
                var str = Marshal.PtrToStructure<NkMAIDString>(pString);
                result = str.str ?? "";
            }
            Marshal.FreeHGlobal(pString);
            return result;
        }

        private int GetIntegerCapability(IntPtr pObject, uint capId)
        {
            IntPtr pInt = Marshal.AllocHGlobal(sizeof(int));
            Marshal.WriteInt32(pInt, 0);
            bool ok = CommandCapGet(pObject, capId, eNkMAIDDataType.kNkMAIDDataType_IntegerPtr, pInt);
            int val = Marshal.ReadInt32(pInt);
            Marshal.FreeHGlobal(pInt);
            return ok ? val : -1;
        }

        private uint GetUnsignedCapability(IntPtr pObject, uint capId)
        {
            IntPtr pUint = Marshal.AllocHGlobal(sizeof(uint));
            Marshal.WriteInt32(pUint, 0);
            CommandCapGet(pObject, capId, eNkMAIDDataType.kNkMAIDDataType_UnsignedPtr, pUint);
            uint val = (uint)Marshal.ReadInt32(pUint);
            Marshal.FreeHGlobal(pUint);
            return val;
        }

        #endregion

        #region Enum Capabilities (Exposure Mode, Shutter Speed, Aperture, ISO, WB, etc.)

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public bool GetEnumCapability(uint capId, out List<string> options, out int currentIndex)
        {
            options = new List<string>();
            currentIndex = -1;

            lock (_sdkLock)
            {
                if (!_isSourceOpen) return false;

                // Verificar se a capability existe e suporta Get
                if (!HasCapability(_sourceCapabilities, capId, eNkMAIDCapOperation.kNkMAIDCapOperation_Get))
                {
                    Log($"GetEnumCapability: cap 0x{capId:X} não suportada");
                    return false;
                }

                uint capType = GetCapabilityType(_sourceCapabilities, capId);

                // D7200 usa kNkMAIDCapType_Unsigned para MeteringMode, FocusMode, etc.
                // D7500 usa kNkMAIDCapType_Enum para os mesmos capabilities.
                // Precisamos suportar ambos os tipos.
                if (capType == (uint)eNkMAIDCapType.kNkMAIDCapType_Unsigned)
                {
                    return GetUnsignedAsEnum(capId, out options, out currentIndex);
                }

                if (capType != (uint)eNkMAIDCapType.kNkMAIDCapType_Enum)
                {
                    Log($"GetEnumCapability: cap 0x{capId:X} tipo não suportado ({capType})");
                    return false;
                }

                // Tipo Enum: fluxo normal com retry para BufferSize (como PicStone)
                for (int retry = 0; retry < 3; retry++)
                {
                    IntPtr pEnum = Marshal.AllocHGlobal(Marshal.SizeOf<NkMAIDEnum>());

                    try
                    {
                        // Passo 1: CapGet para obter metadata (ulElements, wPhysicalBytes, etc.)
                        if (!CommandCapGet(_pSourceObject, capId, eNkMAIDDataType.kNkMAIDDataType_EnumPtr, pEnum))
                        {
                            Marshal.FreeHGlobal(pEnum);
                            Log($"GetEnumCapability: CapGet falhou para 0x{capId:X}");
                            return false;
                        }

                        var stEnum = Marshal.PtrToStructure<NkMAIDEnum>(pEnum);
                        currentIndex = (int)stEnum.ulValue;

                        // Validar metadata antes de alocar
                        if (stEnum.ulElements == 0 || stEnum.wPhysicalBytes <= 0)
                        {
                            Marshal.FreeHGlobal(pEnum);
                            return true;
                        }

                        // Limite de sanidade (não alocar mais que 1MB)
                        int dataSize = (int)(stEnum.ulElements * stEnum.wPhysicalBytes);
                        if (dataSize <= 0 || dataSize > 1048576)
                        {
                            Log($"GetEnumCapability: tamanho inválido para 0x{capId:X}: elements={stEnum.ulElements} bytes={stEnum.wPhysicalBytes}");
                            Marshal.FreeHGlobal(pEnum);
                            return false;
                        }

                        // Passo 2: Alocar buffer para dados
                        stEnum.pData = Marshal.AllocHGlobal(dataSize);
                        Marshal.StructureToPtr(stEnum, pEnum, false);

                        // Passo 3: CapGetArray para obter dados
                        if (!CommandCapGetArray(_pSourceObject, capId, eNkMAIDDataType.kNkMAIDDataType_EnumPtr, pEnum))
                        {
                            Marshal.FreeHGlobal(stEnum.pData);
                            Marshal.FreeHGlobal(pEnum);

                            // BufferSize error → retry (como PicStone)
                            Log($"GetEnumCapability: CapGetArray falhou para 0x{capId:X}, retry {retry + 1}/3");
                            continue;
                        }

                        // Passo 4: Parsear dados conforme tipo do array
                        if (stEnum.ulType == (uint)eNkMAIDArrayType.kNkMAIDArrayType_Unsigned)
                        {
                            for (uint i = 0; i < stEnum.ulElements; i++)
                            {
                                uint val;
                                if (stEnum.wPhysicalBytes == 4)
                                    val = (uint)Marshal.ReadInt32(stEnum.pData, (int)(i * 4));
                                else if (stEnum.wPhysicalBytes == 2)
                                    val = (uint)Marshal.ReadInt16(stEnum.pData, (int)(i * 2));
                                else
                                    val = Marshal.ReadByte(stEnum.pData, (int)i);

                                options.Add(GetEnumValueString(capId, val));
                            }
                        }
                        else if (stEnum.ulType == (uint)eNkMAIDArrayType.kNkMAIDArrayType_PackedString)
                        {
                            int offset = 0;
                            for (uint i = 0; i < stEnum.ulElements; i++)
                            {
                                string s = Marshal.PtrToStringAnsi(IntPtr.Add(stEnum.pData, offset));
                                options.Add(s ?? $"#{i}");
                                offset += (s?.Length ?? 0) + 1;
                            }
                        }
                        else
                        {
                            for (uint i = 0; i < stEnum.ulElements; i++)
                                options.Add($"#{i}");
                        }

                        Marshal.FreeHGlobal(stEnum.pData);
                        Marshal.FreeHGlobal(pEnum);
                        return true;
                    }
                    catch (AccessViolationException ex)
                    {
                        Log($"GetEnumCapability: NATIVE CRASH para 0x{capId:X}: {ex.Message}");
                        try { Marshal.FreeHGlobal(pEnum); } catch { }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Log($"GetEnumCapability: Exceção para 0x{capId:X}: {ex.GetType().Name}: {ex.Message}");
                        try { Marshal.FreeHGlobal(pEnum); } catch { }
                        return false;
                    }
                }

                Log($"GetEnumCapability: falhou após 3 retries para 0x{capId:X}");
                return false;
            }
        }

        /// <summary>
        /// Lê uma capability Unsigned e apresenta como enum usando tabela de valores conhecidos.
        /// Usado quando D7200 reporta MeteringMode/FocusMode como Unsigned em vez de Enum.
        /// </summary>
        private bool GetUnsignedAsEnum(uint capId, out List<string> options, out int currentIndex)
        {
            options = new List<string>();
            currentIndex = -1;

            try
            {
                // Ler valor atual
                uint currentValue = GetUnsignedCapability(_pSourceObject, capId);
                Log($"GetUnsignedAsEnum: cap 0x{capId:X} valor={currentValue}");

                // Obter valores conhecidos para esta capability
                var knownValues = GetKnownValuesForCapability(capId);

                var valueList = new List<uint>();
                for (int i = 0; i < knownValues.Count; i++)
                {
                    valueList.Add(knownValues[i].Key);
                    options.Add(knownValues[i].Value);
                    if (knownValues[i].Key == currentValue)
                        currentIndex = i;
                }

                // Se valor atual não está na lista, adicionar
                if (currentIndex < 0)
                {
                    valueList.Add(currentValue);
                    options.Add(GetEnumValueString(capId, currentValue));
                    currentIndex = options.Count - 1;
                }

                // Guardar mapa de valores para usar no Set
                _unsignedCapValues[capId] = valueList;
                return true;
            }
            catch (Exception ex)
            {
                Log($"GetUnsignedAsEnum: Exceção para 0x{capId:X}: {ex.GetType().Name}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Tabela de valores conhecidos para capabilities que podem ser Unsigned em alguns modelos.
        /// </summary>
        private List<KeyValuePair<uint, string>> GetKnownValuesForCapability(uint capId)
        {
            var list = new List<KeyValuePair<uint, string>>();
            switch (capId)
            {
                case (uint)eNkMAIDCapability.kNkMAIDCapability_MeteringMode:
                    list.Add(new KeyValuePair<uint, string>(0, "Matrix"));
                    list.Add(new KeyValuePair<uint, string>(1, "Ponderada Central"));
                    list.Add(new KeyValuePair<uint, string>(2, "Spot"));
                    list.Add(new KeyValuePair<uint, string>(4, "Highlight"));
                    break;
                case (uint)eNkMAIDCapability.kNkMAIDCapability_FocusMode:
                    list.Add(new KeyValuePair<uint, string>(0, "MF"));
                    list.Add(new KeyValuePair<uint, string>(1, "AF-S"));
                    list.Add(new KeyValuePair<uint, string>(2, "AF-C"));
                    list.Add(new KeyValuePair<uint, string>(3, "AF-A"));
                    list.Add(new KeyValuePair<uint, string>(4, "AF-F"));
                    break;
                case (uint)eNkMAIDCapability.kNkMAIDCapability_ExposureMode:
                    list.Add(new KeyValuePair<uint, string>(0, "P (Programa)"));
                    list.Add(new KeyValuePair<uint, string>(1, "A (Abertura)"));
                    list.Add(new KeyValuePair<uint, string>(2, "S (Velocidade)"));
                    list.Add(new KeyValuePair<uint, string>(3, "M (Manual)"));
                    list.Add(new KeyValuePair<uint, string>(5, "Auto"));
                    break;
                default:
                    // Capability sem tabela conhecida - mostrar valor numérico
                    break;
            }
            return list;
        }

        public bool SetEnumCapability(uint capId, int index)
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return false;

                // Verificar tipo: se Unsigned, usar Set direto com valor do mapa
                uint capType = GetCapabilityType(_sourceCapabilities, capId);
                if (capType == (uint)eNkMAIDCapType.kNkMAIDCapType_Unsigned)
                {
                    return SetUnsignedFromEnum(capId, index);
                }

                // Tipo Enum: CapGet → CapGetArray (preencher pData) → modificar ulValue → CapSet
                // O SDK requer pData válido no CapSet (conforme wrapper PicStone)
                IntPtr pEnum = Marshal.AllocHGlobal(Marshal.SizeOf<NkMAIDEnum>());

                if (!CommandCapGet(_pSourceObject, capId, eNkMAIDDataType.kNkMAIDDataType_EnumPtr, pEnum))
                {
                    Marshal.FreeHGlobal(pEnum);
                    return false;
                }

                var stEnum = Marshal.PtrToStructure<NkMAIDEnum>(pEnum);

                // Alocar e preencher pData (array de valores)
                IntPtr pData = IntPtr.Zero;
                if (stEnum.ulElements > 0 && stEnum.wPhysicalBytes > 0)
                {
                    int dataSize = (int)(stEnum.ulElements * stEnum.wPhysicalBytes);
                    pData = Marshal.AllocHGlobal(dataSize);
                    stEnum.pData = pData;
                    Marshal.StructureToPtr(stEnum, pEnum, false);

                    if (!CommandCapGetArray(_pSourceObject, capId, eNkMAIDDataType.kNkMAIDDataType_EnumPtr, pEnum))
                    {
                        Marshal.FreeHGlobal(pData);
                        Marshal.FreeHGlobal(pEnum);
                        Log($"SetEnumCapability: CapGetArray falhou para 0x{capId:X}");
                        return false;
                    }

                    // Re-ler struct após CapGetArray
                    stEnum = Marshal.PtrToStructure<NkMAIDEnum>(pEnum);
                }

                // Modificar índice selecionado
                stEnum.ulValue = (uint)index;
                Marshal.StructureToPtr(stEnum, pEnum, false);

                bool ok = CommandCapSet(_pSourceObject, capId, eNkMAIDDataType.kNkMAIDDataType_EnumPtr, pEnum);

                if (pData != IntPtr.Zero)
                    Marshal.FreeHGlobal(pData);
                Marshal.FreeHGlobal(pEnum);
                return ok;
            }
        }

        /// <summary>
        /// Seta uma capability Unsigned usando o índice do combo e o mapa de valores.
        /// </summary>
        private bool SetUnsignedFromEnum(uint capId, int index)
        {
            // Buscar valor real no mapa salvo pelo GetUnsignedAsEnum
            List<uint> values;
            if (!_unsignedCapValues.TryGetValue(capId, out values) || index < 0 || index >= values.Count)
            {
                Log($"SetUnsignedFromEnum: índice {index} inválido para cap 0x{capId:X}");
                return false;
            }

            uint value = values[index];
            Log($"SetUnsignedFromEnum: cap 0x{capId:X} index={index} valor={value}");

            return CommandCapSet(_pSourceObject, capId,
                eNkMAIDDataType.kNkMAIDDataType_Unsigned, (IntPtr)value);
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public bool GetRangeCapability(uint capId, out double value, out double lower, out double upper, out uint steps)
        {
            value = 0; lower = 0; upper = 0; steps = 0;

            lock (_sdkLock)
            {
                if (!_isSourceOpen) return false;

                if (!HasCapability(_sourceCapabilities, capId, eNkMAIDCapOperation.kNkMAIDCapOperation_Get))
                    return false;

                uint capType = GetCapabilityType(_sourceCapabilities, capId);
                if (capType != (uint)eNkMAIDCapType.kNkMAIDCapType_Range)
                    return false;

                IntPtr pRange = Marshal.AllocHGlobal(Marshal.SizeOf<NkMAIDRange>());

                try
                {
                    if (!CommandCapGet(_pSourceObject, capId, eNkMAIDDataType.kNkMAIDDataType_RangePtr, pRange))
                    {
                        Marshal.FreeHGlobal(pRange);
                        return false;
                    }

                    var range = Marshal.PtrToStructure<NkMAIDRange>(pRange);
                    lower = range.lfLower;
                    upper = range.lfUpper;
                    steps = range.ulSteps;

                    // Conforme wrapper PicStone: range discreta usa ulValueIndex, não lfValue
                    if (range.ulSteps > 0 && range.ulSteps > 1)
                    {
                        double delta = (range.lfUpper - range.lfLower) / (range.ulSteps - 1);
                        value = range.lfLower + range.ulValueIndex * delta;
                    }
                    else
                    {
                        value = range.lfValue;
                    }

                    Marshal.FreeHGlobal(pRange);
                    return true;
                }
                catch (AccessViolationException ex)
                {
                    Log($"GetRangeCapability: NATIVE CRASH para 0x{capId:X}: {ex.Message}");
                    try { Marshal.FreeHGlobal(pRange); } catch { }
                    return false;
                }
                catch (Exception ex)
                {
                    Log($"GetRangeCapability: Exceção para 0x{capId:X}: {ex.GetType().Name}: {ex.Message}");
                    try { Marshal.FreeHGlobal(pRange); } catch { }
                    return false;
                }
            }
        }

        public bool SetRangeCapability(uint capId, double value)
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return false;

                IntPtr pRange = Marshal.AllocHGlobal(Marshal.SizeOf<NkMAIDRange>());

                if (!CommandCapGet(_pSourceObject, capId, eNkMAIDDataType.kNkMAIDDataType_RangePtr, pRange))
                {
                    Marshal.FreeHGlobal(pRange);
                    return false;
                }

                var range = Marshal.PtrToStructure<NkMAIDRange>(pRange);

                // Conforme wrapper PicStone (NikonTypes.cs):
                // - ulSteps == 0 → range contínua, setar lfValue
                // - ulSteps > 0  → range discreta, setar ulValueIndex (SDK ignora lfValue!)
                if (range.ulSteps == 0)
                {
                    range.lfValue = value;
                }
                else
                {
                    // IndexFromValue: (value - lower) / delta, delta = (upper - lower) / (steps - 1)
                    double delta = (range.lfUpper - range.lfLower) / (range.ulSteps - 1);
                    uint index = (uint)Math.Floor((value - range.lfLower) / delta);
                    if (index >= range.ulSteps) index = range.ulSteps - 1;
                    range.ulValueIndex = index;
                    Log($"SetRangeCapability: cap 0x{capId:X} value={value} → index={index} (steps={range.ulSteps}, lower={range.lfLower}, upper={range.lfUpper}, delta={delta})");
                }

                Marshal.StructureToPtr(range, pRange, false);

                bool ok = CommandCapSet(_pSourceObject, capId, eNkMAIDDataType.kNkMAIDDataType_RangePtr, pRange);

                Marshal.FreeHGlobal(pRange);
                return ok;
            }
        }

        private string GetEnumValueString(uint capId, uint value)
        {
            switch (capId)
            {
                case (uint)eNkMAIDCapability.kNkMAIDCapability_ExposureMode:
                    switch (value)
                    {
                        case 0: return "P (Programa)";
                        case 1: return "A (Abertura)";
                        case 2: return "S (Velocidade)";
                        case 3: return "M (Manual)";
                        case 5: return "Auto";
                        case 13: return "Flash Off";
                        case 14: return "Scene";
                        case 17: return "Effects";
                        default: return $"Modo {value}";
                    }
                case (uint)eNkMAIDCapability.kNkMAIDCapability_MeteringMode:
                    switch (value)
                    {
                        case 0: return "Matrix";
                        case 1: return "Ponderada Central";
                        case 2: return "Spot";
                        case 4: return "Highlight";
                        default: return $"Medição {value}";
                    }
                case (uint)eNkMAIDCapability.kNkMAIDCapability_PictureControl:
                    switch (value)
                    {
                        case 1: return "Standard 0";
                        case 2: return "Neutral 0";
                        case 3: return "Vivid 0";
                        case 4: return "Monochrome";
                        case 5: return "Portrait";
                        case 6: return "Landscape";
                        case 7: return "Flat";
                        case 8: return "Auto";
                        case 201: return "Vivid sat -1";
                        case 202: return "Vivid sat -2";
                        case 203: return "Standard sat -1";
                        case 204: return "Standard sat -2";
                        case 205: return "Neutral sat -1";
                        case 206: return "Neutral sat -2";
                        case 207: return "Super Vivid 1";
                        case 208: return "Super Vivid 2";
                        case 209: return "Super Neutral";
                        default: return $"PicCtrl {value}";
                    }
                default:
                    return value.ToString();
            }
        }

        #endregion

        #region Capture

        /// <summary>
        /// Retorna resultado da captura: NoError=sucesso, outros=erro específico
        /// </summary>
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public eNkMAIDResult Capture()
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return eNkMAIDResult.kNkMAIDResult_NotInitialized;

                // Verificar se Capture é suportado
                bool hasCap = HasCapability(_sourceCapabilities,
                    (uint)eNkMAIDCapability.kNkMAIDCapability_Capture,
                    eNkMAIDCapOperation.kNkMAIDCapOperation_Start);
                Log($"Capture: iniciando... hasCap={hasCap}");

                if (!hasCap)
                {
                    Log("Capture: capability não suportada!");
                    return eNkMAIDResult.kNkMAIDResult_NotSupported;
                }

                try
                {
                    eNkMAIDResult result = CommandCapStartWithResult(_pSourceObject,
                        (uint)eNkMAIDCapability.kNkMAIDCapability_Capture);
                    Log($"Capture: resultado={result}");
                    return result;
                }
                catch (AccessViolationException ex)
                {
                    Log($"Capture: NATIVE CRASH: {ex.Message}");
                    return eNkMAIDResult.kNkMAIDResult_UnexpectedError;
                }
                catch (Exception ex)
                {
                    Log($"Capture: EXCEÇÃO: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                    return eNkMAIDResult.kNkMAIDResult_UnexpectedError;
                }
            }
        }

        public bool CaptureAsync()
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return false;
                return CommandCapStart(_pSourceObject, (uint)eNkMAIDCapability.kNkMAIDCapability_CaptureAsync);
            }
        }

        public bool AFCaptureAsync()
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return false;
                return CommandCapStart(_pSourceObject, (uint)eNkMAIDCapability.kNkMAIDCapability_AFCaptureAsync);
            }
        }

        public bool AutoFocus()
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return false;
                return CommandCapStart(_pSourceObject, (uint)eNkMAIDCapability.kNkMAIDCapability_AutoFocus);
            }
        }

        public bool DeviceReady()
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return false;
                return CommandCapStart(_pSourceObject, (uint)eNkMAIDCapability.kNkMAIDCapability_DeviceReady);
            }
        }

        /// <summary>
        /// Obtém o modo AF atual. 0=AF-S, 1=AF-C, 2=AF-A, 3=AF-F, 4=MF (conforme PicStone)
        /// </summary>
        public uint GetAFMode()
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return 0;
                return GetUnsignedCapability(_pSourceObject, (uint)eNkMAIDCapability.kNkMAIDCapability_AFMode);
            }
        }

        /// <summary>
        /// Seta o modo AF. 0=AF-S, 4=MF (conforme PicStone)
        /// </summary>
        public bool SetAFMode(uint mode)
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return false;
                Log($"SetAFMode: mode={mode}");
                return CommandCapSet(_pSourceObject,
                    (uint)eNkMAIDCapability.kNkMAIDCapability_AFMode,
                    eNkMAIDDataType.kNkMAIDDataType_Unsigned, (IntPtr)mode);
            }
        }

        /// <summary>
        /// Trava/destrava a câmera (necessário para MF conforme PicStone)
        /// </summary>
        public bool SetLockCamera(bool locked)
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return false;
                Log($"SetLockCamera: locked={locked}");
                IntPtr pBool = Marshal.AllocHGlobal(sizeof(int));
                Marshal.WriteInt32(pBool, locked ? 1 : 0);
                bool ok = CommandCapSet(_pSourceObject,
                    (uint)eNkMAIDCapability.kNkMAIDCapability_LockCamera,
                    eNkMAIDDataType.kNkMAIDDataType_BooleanPtr, pBool);
                Marshal.FreeHGlobal(pBool);
                return ok;
            }
        }

        /// <summary>
        /// Ativa MF: LockCamera=true, AFMode=4 (conforme PicStone SetarFoco)
        /// </summary>
        public bool EnableManualFocus()
        {
            Log("EnableManualFocus...");
            bool ok1 = SetLockCamera(true);
            bool ok2 = SetAFMode(4);
            Log($"EnableManualFocus: lock={ok1} afMode={ok2}");
            return ok1 && ok2;
        }

        /// <summary>
        /// Ativa AF-S: AFMode=0, LockCamera=false (conforme PicStone SetarFoco)
        /// </summary>
        public bool EnableAutoFocus()
        {
            Log("EnableAutoFocus...");
            bool ok1 = SetAFMode(0);
            bool ok2 = SetLockCamera(false);
            Log($"EnableAutoFocus: afMode={ok1} lock={ok2}");
            return ok1 && ok2;
        }

        #endregion

        #region Live View

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public bool StartLiveView()
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return false;

                Log("StartLiveView: setando LiveViewStatus=ON...");
                // Setar LiveViewStatus = ON diretamente (sem check de Prohibit, conforme wrapper)
                bool ok = CommandCapSet(_pSourceObject,
                    (uint)eNkMAIDCapability.kNkMAIDCapability_LiveViewStatus,
                    eNkMAIDDataType.kNkMAIDDataType_Unsigned,
                    (IntPtr)(uint)eNkMAIDLiveViewStatus.kNkMAIDLiveViewStatus_ON);
                Log($"StartLiveView: resultado={ok}");
                return ok;
            }
        }

        public bool StopLiveView()
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return false;

                Log("StopLiveView...");
                return CommandCapSet(_pSourceObject,
                    (uint)eNkMAIDCapability.kNkMAIDCapability_LiveViewStatus,
                    eNkMAIDDataType.kNkMAIDDataType_Unsigned,
                    (IntPtr)(uint)eNkMAIDLiveViewStatus.kNkMAIDLiveViewStatus_OFF);
            }
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public byte[] GetLiveViewImage()
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return null;

                IntPtr pArray = Marshal.AllocHGlobal(Marshal.SizeOf<NkMAIDArray>());

                try
                {
                    if (!CommandCapGet(_pSourceObject, (uint)eNkMAIDCapability.kNkMAIDCapability_GetLiveViewImage,
                        eNkMAIDDataType.kNkMAIDDataType_ArrayPtr, pArray))
                    {
                        Marshal.FreeHGlobal(pArray);
                        return null;
                    }

                    var stArray = Marshal.PtrToStructure<NkMAIDArray>(pArray);

                    if (stArray.ulElements == 0)
                    {
                        Marshal.FreeHGlobal(pArray);
                        return null;
                    }

                    int totalSize = (int)(stArray.ulElements * stArray.wPhysicalBytes);
                    stArray.pData = Marshal.AllocHGlobal(totalSize);
                    Marshal.StructureToPtr(stArray, pArray, false);

                    if (!CommandCapGetArray(_pSourceObject, (uint)eNkMAIDCapability.kNkMAIDCapability_GetLiveViewImage,
                        eNkMAIDDataType.kNkMAIDDataType_ArrayPtr, pArray))
                    {
                        Marshal.FreeHGlobal(stArray.pData);
                        Marshal.FreeHGlobal(pArray);
                        return null;
                    }

                    // Pular header de 384 bytes
                    int headerSize = MaidConstants.LiveViewHeaderSize;
                    if (totalSize <= headerSize)
                    {
                        Marshal.FreeHGlobal(stArray.pData);
                        Marshal.FreeHGlobal(pArray);
                        return null;
                    }

                    int jpegSize = totalSize - headerSize;
                    byte[] result = new byte[jpegSize];
                    Marshal.Copy(IntPtr.Add(stArray.pData, headerSize), result, 0, jpegSize);

                    Marshal.FreeHGlobal(stArray.pData);
                    Marshal.FreeHGlobal(pArray);
                    return result;
                }
                catch (AccessViolationException ex)
                {
                    Log($"GetLiveViewImage NATIVE CRASH: {ex.Message}");
                    try { Marshal.FreeHGlobal(pArray); } catch { }
                    return null;
                }
                catch (Exception ex)
                {
                    Log($"GetLiveViewImage ERRO: {ex.GetType().Name}: {ex.Message}");
                    try { Marshal.FreeHGlobal(pArray); } catch { }
                    return null;
                }
            }
        }

        public uint GetLiveViewStatus()
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return 0;
                return GetUnsignedCapability(_pSourceObject, (uint)eNkMAIDCapability.kNkMAIDCapability_LiveViewStatus);
            }
        }

        #endregion

        #region Async Operation Helper

        public void ProcessAsync()
        {
            lock (_sdkLock)
            {
                if (_isModuleOpen && _pModuleObject != IntPtr.Zero)
                    CommandAsync(_pModuleObject);
                if (_isSourceOpen && _pSourceObject != IntPtr.Zero)
                    CommandAsync(_pSourceObject);
            }
        }

        #endregion

        #region SetupInicial

        /// <summary>
        /// Seta uma capability Unsigned diretamente (sem usar enum struct).
        /// </summary>
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public bool SetUnsignedCapability(uint capId, uint value)
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return false;
                Log($"SetUnsignedCapability: cap=0x{capId:X} value={value}");
                return CommandCapSet(_pSourceObject, capId,
                    eNkMAIDDataType.kNkMAIDDataType_Unsigned, (IntPtr)value);
            }
        }

        /// <summary>
        /// Seta uma capability Boolean diretamente.
        /// </summary>
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public bool SetBooleanCapability(uint capId, bool value)
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return false;
                Log($"SetBooleanCapability: cap=0x{capId:X} value={value}");
                IntPtr pBool = Marshal.AllocHGlobal(sizeof(int));
                try
                {
                    Marshal.WriteInt32(pBool, value ? 1 : 0);
                    return CommandCapSet(_pSourceObject, capId,
                        eNkMAIDDataType.kNkMAIDDataType_BooleanPtr, pBool);
                }
                finally
                {
                    Marshal.FreeHGlobal(pBool);
                }
            }
        }

        /// <summary>
        /// Configurações globais da câmera (PicStone: SetupInicial).
        /// Chamado 1x na conexão. Peculiaridades por modelo.
        /// </summary>
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public void SetupInicial()
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return;

                string cameraName = GetCameraName();
                Log($"SetupInicial: câmera={cameraName}");

                // AFMode = 2 (AFC)
                try
                {
                    SetAFMode(2);
                }
                catch (Exception ex) { Log($"SetupInicial AFMode falhou: {ex.Message}"); }

                // AF-C Priority = 0 (Release), AF-S Priority = 0 (Release)
                try
                {
                    SetEnumCapability((uint)eNkMAIDCapability.kNkMAIDCapability_AFcPriority, 0);
                }
                catch (Exception ex) { Log($"SetupInicial AFcPriority falhou: {ex.Message}"); }

                try
                {
                    SetEnumCapability((uint)eNkMAIDCapability.kNkMAIDCapability_AFsPriority, 0);
                }
                catch (Exception ex) { Log($"SetupInicial AFsPriority falhou: {ex.Message}"); }

                // FocusAreaMode = 2 (AFS/Auto)
                try
                {
                    SetEnumCapability((uint)eNkMAIDCapability.kNkMAIDCapability_FocusAreaMode, 2);
                }
                catch (Exception ex) { Log($"SetupInicial FocusAreaMode falhou: {ex.Message}"); }

                // AfSubLight (só D7100)
                if (cameraName != null && cameraName.Contains("D7100"))
                {
                    try
                    {
                        SetBooleanCapability((uint)eNkMAIDCapability.kNkMAIDCapability_AfSubLight, false);
                        SetBooleanCapability((uint)eNkMAIDCapability.kNkMAIDCapability_AfSubLight, true);
                    }
                    catch (Exception ex) { Log($"SetupInicial AfSubLight falhou: {ex.Message}"); }
                }

                // CompressionLevel: D7500=4, outros=2
                try
                {
                    int compIndex = (cameraName != null && cameraName.Contains("D7500")) ? 4 : 2;
                    SetEnumCapability((uint)eNkMAIDCapability.kNkMAIDCapability_CompressionLevel, compIndex);
                }
                catch (Exception ex) { Log($"SetupInicial CompressionLevel falhou: {ex.Message}"); }

                // JpegCompressionPolicy = Quality (não D7500)
                if (cameraName == null || !cameraName.Contains("D7500"))
                {
                    try
                    {
                        SetUnsignedCapability(
                            (uint)eNkMAIDCapability.kNkMAIDCapability_JpegCompressionPolicy,
                            (uint)eNkMAIDJpegCompressionPolicy.kNkMAIDJpegCompressionPolicy_Quality);
                    }
                    catch (Exception ex) { Log($"SetupInicial JpegCompressionPolicy falhou: {ex.Message}"); }
                }

                // SaveMedia = SDRAM
                try
                {
                    SetUnsignedCapability(
                        (uint)eNkMAIDCapability.kNkMAIDCapability_SaveMedia,
                        (uint)eNkMAIDSaveMedia.kNkMAIDSaveMedia_SDRAM);
                }
                catch (Exception ex) { Log($"SetupInicial SaveMedia falhou: {ex.Message}"); }

                // ImageColorSpace = sRGB
                try
                {
                    SetUnsignedCapability(
                        (uint)eNkMAIDCapability.kNkMAIDCapability_ImageColorSpace,
                        (uint)eNkMAIDImageColorSpace.kNkMAIDImageColorSpace_sRGB);
                }
                catch (Exception ex) { Log($"SetupInicial ImageColorSpace falhou: {ex.Message}"); }

                // AutoDistortion = ON
                try
                {
                    SetUnsignedCapability(
                        (uint)eNkMAIDCapability.kNkMAIDCapability_AutoDistortion,
                        (uint)eNkMAIDAutoDistortion.kNkMAIDAutoDistortion_On);
                }
                catch (Exception ex) { Log($"SetupInicial AutoDistortion falhou: {ex.Message}"); }

                // IsoControl = OFF
                try
                {
                    SetBooleanCapability((uint)eNkMAIDCapability.kNkMAIDCapability_IsoControl, false);
                }
                catch (Exception ex) { Log($"SetupInicial IsoControl falhou: {ex.Message}"); }

                // VignetteControl = Off (não D7100)
                if (cameraName == null || !cameraName.Contains("D7100"))
                {
                    try
                    {
                        SetUnsignedCapability(
                            (uint)eNkMAIDCapability.kNkMAIDCapability_VignetteControl,
                            (uint)eNkMAIDVignetteControl.kNkMAIDVignetteControl_Off);
                    }
                    catch (Exception ex) { Log($"SetupInicial VignetteControl falhou: {ex.Message}"); }
                }

                // ImageSize = M (index 1)
                try
                {
                    SetEnumCapability((uint)eNkMAIDCapability.kNkMAIDCapability_ImageSize, 1);
                }
                catch (Exception ex) { Log($"SetupInicial ImageSize falhou: {ex.Message}"); }

                // Active D-Lighting = 3
                try
                {
                    SetUnsignedCapability(
                        (uint)eNkMAIDCapability.kNkMAIDCapability_Active_D_Lighting, 3);
                }
                catch (Exception ex) { Log($"SetupInicial Active_D_Lighting falhou: {ex.Message}"); }

                Log("SetupInicial: COMPLETO");
            }
        }

        #endregion

        #region SaveMedia

        public bool SetSaveMediaSDRAM()
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return false;

                // PicStone não verifica HasCapability, simplesmente tenta setar.
                // Se falhar, a captura salva no cartão SD por padrão.
                Log("SetSaveMediaSDRAM: setando...");
                bool ok = CommandCapSet(_pSourceObject,
                    (uint)eNkMAIDCapability.kNkMAIDCapability_SaveMedia,
                    eNkMAIDDataType.kNkMAIDDataType_Unsigned,
                    (IntPtr)(uint)eNkMAIDSaveMedia.kNkMAIDSaveMedia_SDRAM);
                Log($"SetSaveMediaSDRAM: resultado={ok}");
                return ok;
            }
        }

        public bool SetSaveMediaCard()
        {
            lock (_sdkLock)
            {
                if (!_isSourceOpen) return false;
                return CommandCapSet(_pSourceObject,
                    (uint)eNkMAIDCapability.kNkMAIDCapability_SaveMedia,
                    eNkMAIDDataType.kNkMAIDDataType_Unsigned,
                    (IntPtr)(uint)eNkMAIDSaveMedia.kNkMAIDSaveMedia_Card);
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                try { UnloadModule(); } catch { }
            }

            foreach (var handle in _gcHandles)
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
            _gcHandles.Clear();

            _disposed = true;
        }

        ~NikonManager()
        {
            Dispose(false);
        }

        #endregion
    }
}
