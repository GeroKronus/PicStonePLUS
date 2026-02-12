using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PicStonePlus.Models;
using PicStonePlus.Processing;
using PicStonePlus.SDK;

namespace PicStonePlus.Forms
{
    public partial class MainForm : Form
    {
        private NikonManager _nikonManager;
        private bool _isLiveViewActive;
        private bool _isUpdatingControls; // evita loops de eventos
        private string _savePath;
        private System.Windows.Forms.Timer _debounceTimer; // debounce para CapChange events
        private bool _capChangePending;
        private System.Windows.Forms.Timer _reconnectTimer; // timer para reconexão automática
        private bool _isReconnecting;
        private List<MaterialPreset> _presets;
        private MaterialPreset _currentPreset; // preset ativo (para pós-produção)
        private Bitmap _originalImage;    // imagem original capturada (sem pós-produção)
        private Bitmap _processedImage;   // imagem com pós-produção aplicada
        private Bitmap _beforeImage;      // imagem como estava ANTES da última mudança de PP

        public MainForm()
        {
            InitializeComponent();
            _nikonManager = new NikonManager();
            _nikonManager.CameraEvent += OnCameraEvent;
            _nikonManager.ImageReady += OnImageReady;
            _nikonManager.Progress += OnProgress;
            _nikonManager.CameraDisconnected += OnCameraDisconnected;

            _savePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                "NikonD7500");

            if (!Directory.Exists(_savePath))
                Directory.CreateDirectory(_savePath);

            // Debounce timer para CapChange - atualiza controles no máximo 1x a cada 2s
            _debounceTimer = new System.Windows.Forms.Timer();
            _debounceTimer.Interval = 2000;
            _debounceTimer.Tick += (s, ev) =>
            {
                _debounceTimer.Stop();
                if (_capChangePending && !_isUpdatingControls)
                {
                    _capChangePending = false;
                    PopulateAllControls();
                }
            };

            // Timer de reconexão automática (tenta a cada 3 segundos)
            _reconnectTimer = new System.Windows.Forms.Timer();
            _reconnectTimer.Interval = 3000;
            _reconnectTimer.Tick += async (s, ev) =>
            {
                _reconnectTimer.Stop();
                if (_isReconnecting)
                    return;
                _isReconnecting = true;
                await DoConnect(showErrors: false);
                _isReconnecting = false;
                if (!_nikonManager.IsConnected)
                    _reconnectTimer.Start(); // continuar tentando
            };

            UpdateUIState(false);

            // Manter PictureBox em proporção 3:2 (formato câmera Nikon)
            splitMain.Panel1.Resize += (s, ev) => ResizePictureBox();
            Load += (s, ev) => ResizePictureBox();

            // Auto-connect ao abrir
            Load += async (s, ev) => await AutoConnect();

            // Carregar presets de material
            LoadPresets();
        }

        #region Conexão

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (_nikonManager.IsConnected)
            {
                DoDisconnect();
                return;
            }

            await DoConnect(showErrors: true);
        }

        private async Task AutoConnect()
        {
            SetStatus("Procurando câmera...");
            await DoConnect(showErrors: false);
        }

        private static string LastModuleFile => Path.Combine(
            Path.GetDirectoryName(Application.ExecutablePath) ?? ".", "last_module.txt");

        private async Task DoConnect(bool showErrors)
        {
            // Encontrar todos os módulos .md3 disponíveis
            var modulePaths = FindModulePaths();
            if (modulePaths.Count == 0)
            {
                if (showErrors)
                {
                    MessageBox.Show(
                        "Não foi possível encontrar nenhum módulo .md3.\n\n" +
                        "Certifique-se de que os arquivos Type00xx.md3 estão na mesma pasta do executável.",
                        "Módulo não encontrado",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    SetStatus("Nenhum módulo .md3 encontrado");
                }
                return;
            }

            btnConnect.Enabled = false;
            string connectedModule = null;
            string connectError = null;

            // Priorizar último módulo conectado com sucesso
            var sortedPaths = modulePaths.OrderByDescending(p => Path.GetFileName(p)).ToList();
            try
            {
                if (File.Exists(LastModuleFile))
                {
                    string lastModule = File.ReadAllText(LastModuleFile).Trim();
                    var lastPath = sortedPaths.FirstOrDefault(p =>
                        Path.GetFileName(p).Equals(lastModule, StringComparison.OrdinalIgnoreCase));
                    if (lastPath != null)
                    {
                        sortedPaths.Remove(lastPath);
                        sortedPaths.Insert(0, lastPath);
                    }
                }
            }
            catch { }

            try
            {
                for (int i = 0; i < sortedPaths.Count; i++)
                {
                    string modulePath = sortedPaths[i];
                    string moduleName = Path.GetFileName(modulePath);
                    SetStatus($"Tentando módulo {moduleName}...");

                    if (i > 0)
                        await Task.Delay(1000);

                    bool success = await Task.Run(() =>
                    {
                        if (!_nikonManager.LoadModule(modulePath))
                            return false;
                        if (!_nikonManager.OpenModule())
                        {
                            _nikonManager.UnloadModule();
                            return false;
                        }
                        if (!_nikonManager.ConnectCamera())
                        {
                            _nikonManager.UnloadModule();
                            return false;
                        }
                        return true;
                    });

                    if (success)
                    {
                        connectedModule = moduleName;
                        break;
                    }
                }

                if (connectedModule != null)
                {
                    // Salvar último módulo para próxima inicialização
                    try { File.WriteAllText(LastModuleFile, connectedModule); } catch { }

                    LogUI("Pós-conexão: SetupInicial...");
                    try
                    {
                        await Task.Run(() => _nikonManager.SetupInicial());
                    }
                    catch (Exception ex)
                    {
                        LogUI($"  SetupInicial falhou (não-fatal): {ex.Message}");
                    }

                    LogUI("Pós-conexão: GetCameraName...");
                    string cameraName = _nikonManager.GetCameraName();
                    SetStatus($"Conectado - {cameraName} ({connectedModule})");

                    LogUI("Pós-conexão: UpdateUIState...");
                    UpdateUIState(true);

                    LogUI("Pós-conexão: PopulateAllControls...");
                    PopulateAllControls();

                    // Ler estado AF atual
                    try
                    {
                        uint afMode = await Task.Run(() => _nikonManager.GetAFMode());
                        _isUpdatingControls = true;
                        chkAF.Checked = (afMode != 4); // 4 = MF
                        chkAF.Text = (afMode == 4) ? "MF" : "AF";
                        _isUpdatingControls = false;
                    }
                    catch { }

                    LogUI("Pós-conexão: timerStatus.Start...");
                    timerStatus.Start();

                    LogUI("Pós-conexão: COMPLETO!");
                }
            }
            catch (Exception ex)
            {
                connectError = $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}";
                LogUI($"EXCEÇÃO: {connectError}");
                SetStatus($"Erro: {ex.Message}");
                try { _nikonManager.UnloadModule(); } catch { }
            }

            if (connectedModule != null)
            {
                // Já tratado acima
            }
            else
            {
                if (showErrors)
                {
                    SetStatus("Falha na conexão");
                    string errorDetail = connectError != null
                        ? $"\n\nErro: {connectError}"
                        : "";
                    MessageBox.Show(
                        "Não foi possível conectar à câmera.\n\n" +
                        "Verifique se:\n" +
                        "- A câmera está ligada e conectada via USB\n" +
                        "- O driver NkdPTP está na mesma pasta\n" +
                        "- Nenhum outro software está usando a câmera\n\n" +
                        $"Módulos testados: {modulePaths.Count}" + errorDetail,
                        "Erro de Conexão",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    SetStatus("Câmera não encontrada - clique Conectar para tentar novamente");
                }
                btnConnect.Enabled = true;
            }
        }

        private void DoDisconnect()
        {
            _reconnectTimer.Stop();
            timerLiveView.Stop();
            timerStatus.Stop();
            _debounceTimer.Stop();
            _isLiveViewActive = false;

            _nikonManager.Disconnect();
            _nikonManager.UnloadModule();

            UpdateUIState(false);
            SetStatus("Desconectado");
            picLiveView.Image = null;
            lblLiveViewInfo.Text = "Live View desativado";
        }

        private void menuDesconectar_Click(object sender, EventArgs e)
        {
            DoDisconnect();
        }

        private void menuSair_Click(object sender, EventArgs e)
        {
            if (_nikonManager.IsConnected)
                DoDisconnect();
            Close();
        }

        private List<string> FindModulePaths()
        {
            var modules = new List<string>();
            var searchDirs = new List<string>();

            // Pasta do executável
            string exePath = Path.GetDirectoryName(Application.ExecutablePath);
            searchDirs.Add(exePath);

            // Pasta acima
            string parentPath = Path.GetDirectoryName(exePath);
            if (parentPath != null)
                searchDirs.Add(parentPath);

            // Pasta Resources (dev time)
            string resPath = Path.Combine(exePath, "Resources");
            if (Directory.Exists(resPath))
                searchDirs.Add(resPath);

            foreach (string dir in searchDirs)
            {
                try
                {
                    foreach (string file in Directory.GetFiles(dir, "Type*.md3"))
                    {
                        // Evitar duplicatas (mesmo nome de arquivo)
                        string fileName = Path.GetFileName(file);
                        bool already = false;
                        foreach (string existing in modules)
                        {
                            if (string.Equals(Path.GetFileName(existing), fileName, StringComparison.OrdinalIgnoreCase))
                            {
                                already = true;
                                break;
                            }
                        }
                        if (!already)
                            modules.Add(file);
                    }
                }
                catch { }
            }

            return modules;
        }

        private void UpdateUIState(bool connected)
        {
            btnConnect.Text = connected ? "Desconectar" : "Conectar";
            btnConnect.BackColor = connected ? Color.FromArgb(100, 100, 100) : Color.FromArgb(0, 120, 215);
            btnConnect.Enabled = true;
            btnCapture.Enabled = connected;
            btnLiveView.Enabled = connected;
            btnRefresh.Enabled = connected;
            chkAF.Enabled = connected;
            menuDesconectar.Enabled = connected;

            cboExposureMode.Enabled = connected;
            cboShutterSpeed.Enabled = connected;
            cboAperture.Enabled = connected;
            cboISO.Enabled = connected;
            trackExpComp.Enabled = connected;
            cboMetering.Enabled = connected;
            cboWBMode.Enabled = connected;
            cboFocusMode.Enabled = connected;
            cboAFArea.Enabled = connected;
            btnAutoFocus.Enabled = connected;
            cboPictureControl.Enabled = connected;
            cboPreset.Enabled = connected;

            if (!connected)
            {
                lblCameraName.Text = "Câmera: Não conectada";
                progressBattery.Value = 0;
                lblLens.Text = "Lente: --";
                ClearAllCombos();
            }
        }

        private void ClearAllCombos()
        {
            cboExposureMode.Items.Clear();
            cboShutterSpeed.Items.Clear();
            cboAperture.Items.Clear();
            cboISO.Items.Clear();
            cboMetering.Items.Clear();
            cboWBMode.Items.Clear();
            cboFocusMode.Items.Clear();
            cboAFArea.Items.Clear();
            cboPictureControl.Items.Clear();
        }

        #endregion

        #region Popular Controles

        private void PopulateAllControls()
        {
            _isUpdatingControls = true;

            try
            {
                // Info da câmera
                LogUI("  PopulateAllControls: GetCameraName");
                lblCameraName.Text = "Câmera: " + _nikonManager.GetCameraName();

                LogUI("  PopulateAllControls: GetBatteryLevel");
                int battery = _nikonManager.GetBatteryLevel();
                if (battery >= 0 && battery <= 100)
                    progressBattery.Value = battery;
                lblBattery.Text = $"Bateria: {battery}%";

                LogUI("  PopulateAllControls: GetLensInfo");
                lblLens.Text = "Lente: " + _nikonManager.GetLensInfo();

                // Cada controle protegido individualmente
                SafePopulateEnum("ExposureMode", cboExposureMode, (uint)eNkMAIDCapability.kNkMAIDCapability_ExposureMode);
                SafePopulateEnum("ShutterSpeed", cboShutterSpeed, (uint)eNkMAIDCapability.kNkMAIDCapability_ShutterSpeed);
                SafePopulateEnum("Aperture", cboAperture, (uint)eNkMAIDCapability.kNkMAIDCapability_Aperture);
                SafePopulateEnum("Sensitivity", cboISO, (uint)eNkMAIDCapability.kNkMAIDCapability_Sensitivity);

                LogUI("  PopulateAllControls: ExposureComp");
                try { PopulateExposureComp(); } catch (Exception ex) { LogUI($"    ExposureComp falhou: {ex.Message}"); }

                SafePopulateEnum("MeteringMode", cboMetering, (uint)eNkMAIDCapability.kNkMAIDCapability_MeteringMode);
                SafePopulateEnum("WBMode", cboWBMode, (uint)eNkMAIDCapability.kNkMAIDCapability_WBMode);
                SafePopulateEnum("FocusMode", cboFocusMode, (uint)eNkMAIDCapability.kNkMAIDCapability_FocusMode);
                SafePopulateEnum("FocusAreaMode", cboAFArea, (uint)eNkMAIDCapability.kNkMAIDCapability_FocusAreaMode);
                SafePopulateEnum("PictureControl", cboPictureControl, (uint)eNkMAIDCapability.kNkMAIDCapability_PictureControl);

                LogUI("  PopulateAllControls: COMPLETO");
            }
            catch (Exception ex)
            {
                LogUI($"  PopulateAllControls EXCEÇÃO: {ex.GetType().Name}: {ex.Message}");
                SetStatus("Erro ao popular controles: " + ex.Message);
            }
            finally
            {
                _isUpdatingControls = false;
            }
        }

        private void SafePopulateEnum(string name, ComboBox combo, uint capId)
        {
            LogUI($"  PopulateAllControls: {name}");
            try
            {
                PopulateEnumCombo(combo, capId);
            }
            catch (Exception ex)
            {
                LogUI($"    {name} falhou: {ex.GetType().Name}: {ex.Message}");
            }
        }

        private void PopulateEnumCombo(ComboBox combo, uint capId)
        {
            combo.Items.Clear();
            List<string> options;
            int currentIndex;

            if (_nikonManager.GetEnumCapability(capId, out options, out currentIndex))
            {
                foreach (string opt in options)
                    combo.Items.Add(opt);

                if (currentIndex >= 0 && currentIndex < combo.Items.Count)
                    combo.SelectedIndex = currentIndex;
            }
        }

        private void PopulateExposureComp()
        {
            double value, lower, upper;
            uint steps;
            if (_nikonManager.GetRangeCapability((uint)eNkMAIDCapability.kNkMAIDCapability_ExposureComp,
                out value, out lower, out upper, out steps))
            {
                // Converter para steps do trackbar (multiplicar por 10 para ter resolução de 0.1)
                trackExpComp.Minimum = (int)(lower * 10);
                trackExpComp.Maximum = (int)(upper * 10);
                trackExpComp.Value = Math.Max(trackExpComp.Minimum, Math.Min(trackExpComp.Maximum, (int)(value * 10)));
                lblExpCompValue.Text = $"{value:+0.0;-0.0;0.0} EV";
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            PopulateAllControls();
            SetStatus("Controles atualizados");
        }

        #endregion

        #region Eventos dos Controles

        private void cboExposureMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingControls || !_nikonManager.IsConnected) return;
            _ = SetEnumAsync((uint)eNkMAIDCapability.kNkMAIDCapability_ExposureMode, cboExposureMode.SelectedIndex);
        }

        private void cboShutterSpeed_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingControls || !_nikonManager.IsConnected) return;
            _ = SetEnumAsync((uint)eNkMAIDCapability.kNkMAIDCapability_ShutterSpeed, cboShutterSpeed.SelectedIndex);
        }

        private void cboAperture_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingControls || !_nikonManager.IsConnected) return;
            _ = SetEnumAsync((uint)eNkMAIDCapability.kNkMAIDCapability_Aperture, cboAperture.SelectedIndex);
        }

        private void cboISO_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingControls || !_nikonManager.IsConnected) return;
            _ = SetEnumAsync((uint)eNkMAIDCapability.kNkMAIDCapability_Sensitivity, cboISO.SelectedIndex);
        }

        private void cboMetering_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingControls || !_nikonManager.IsConnected) return;
            _ = SetEnumAsync((uint)eNkMAIDCapability.kNkMAIDCapability_MeteringMode, cboMetering.SelectedIndex);
        }

        private void cboWBMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingControls || !_nikonManager.IsConnected) return;
            _ = SetEnumAsync((uint)eNkMAIDCapability.kNkMAIDCapability_WBMode, cboWBMode.SelectedIndex);
        }

        private void cboFocusMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingControls || !_nikonManager.IsConnected) return;
            _ = SetEnumAsync((uint)eNkMAIDCapability.kNkMAIDCapability_FocusMode, cboFocusMode.SelectedIndex);
        }

        private void cboAFArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingControls || !_nikonManager.IsConnected) return;
            _ = SetEnumAsync((uint)eNkMAIDCapability.kNkMAIDCapability_FocusAreaMode, cboAFArea.SelectedIndex);
        }

        private void cboPictureControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingControls || !_nikonManager.IsConnected) return;
            _ = SetEnumAsync((uint)eNkMAIDCapability.kNkMAIDCapability_PictureControl, cboPictureControl.SelectedIndex);
        }

        private void trackExpComp_Scroll(object sender, EventArgs e)
        {
            if (_isUpdatingControls || !_nikonManager.IsConnected) return;
            double val = trackExpComp.Value / 10.0;
            lblExpCompValue.Text = $"{val:+0.0;-0.0;0.0} EV";
            _ = Task.Run(() => _nikonManager.SetRangeCapability(
                (uint)eNkMAIDCapability.kNkMAIDCapability_ExposureComp, val));
        }

        private async Task SetEnumAsync(uint capId, int index)
        {
            if (index < 0) return;
            await Task.Run(() => _nikonManager.SetEnumCapability(capId, index));
        }

        private async void chkAF_CheckedChanged(object sender, EventArgs e)
        {
            if (_isUpdatingControls || !_nikonManager.IsConnected) return;

            chkAF.Enabled = false;
            if (chkAF.Checked)
            {
                // AF ativado: AFMode=0 (AF-S), LockCamera=false
                await Task.Run(() => _nikonManager.EnableAutoFocus());
                chkAF.Text = "AF";
                SetStatus("AutoFoco ativado (AF-S)");
            }
            else
            {
                // MF ativado: LockCamera=true, AFMode=4 (MF)
                await Task.Run(() => _nikonManager.EnableManualFocus());
                chkAF.Text = "MF";
                SetStatus("Foco Manual ativado (MF)");
            }
            chkAF.Enabled = true;
        }

        #endregion

        #region Captura

        private bool _captureInProgress;
        private DateTime _statusProtectedUntil = DateTime.MinValue;

        private async void btnCapture_Click(object sender, EventArgs e)
        {
            if (!_nikonManager.IsConnected) return;

            btnCapture.Enabled = false;
            _captureInProgress = true;

            // Desativar live view antes da captura
            if (_isLiveViewActive)
            {
                SetStatus("Desativando Live View...");
                await Task.Run(() => _nikonManager.StopLiveView());
                timerLiveView.Stop();
                _isLiveViewActive = false;
                btnLiveView.Text = "Live View ON";
                lblLiveViewInfo.Text = "Live View desativado";
            }

            SetStatus("Capturando...");

            var result = await Task.Run(() => _nikonManager.Capture());

            _captureInProgress = false;

            switch (result)
            {
                case eNkMAIDResult.kNkMAIDResult_NoError:
                    SetStatus("Captura concluída");
                    _statusProtectedUntil = DateTime.Now.AddSeconds(3);
                    break;
                case eNkMAIDResult.kNkMAIDResult_OutOfFocus:
                    SetStatus("Imagem fora de foco");
                    _statusProtectedUntil = DateTime.Now.AddSeconds(5);
                    break;
                case eNkMAIDResult.kNkMAIDResult_NoMedia:
                    SetStatus("Erro: SaveMedia não configurado (NoMedia)");
                    _statusProtectedUntil = DateTime.Now.AddSeconds(5);
                    break;
                default:
                    SetStatus($"Erro na captura: {result}");
                    _statusProtectedUntil = DateTime.Now.AddSeconds(5);
                    break;
            }

            btnCapture.Enabled = true;
        }

        private async void btnAutoFocus_Click(object sender, EventArgs e)
        {
            if (!_nikonManager.IsConnected) return;

            btnAutoFocus.Enabled = false;
            SetStatus("Focando...");

            bool success = await Task.Run(() => _nikonManager.AutoFocus());

            SetStatus(success ? "Foco OK" : "Erro no AutoFocus");
            btnAutoFocus.Enabled = true;
        }

        #endregion

        #region Live View

        private void btnLiveView_Click(object sender, EventArgs e)
        {
            if (_isLiveViewActive)
            {
                StopLiveView();
            }
            else
            {
                StartLiveView();
            }
        }

        private async void StartLiveView()
        {
            if (!_nikonManager.IsConnected) return;

            SetStatus("Iniciando Live View...");

            bool success = await Task.Run(() => _nikonManager.StartLiveView());

            if (success)
            {
                _isLiveViewActive = true;
                timerLiveView.Start();
                btnLiveView.Text = "Live View OFF";
                lblLiveViewInfo.Text = "Live View ativo";
                SetStatus("Live View ativo");
            }
            else
            {
                SetStatus("Não foi possível iniciar Live View");
                MessageBox.Show(
                    "Não foi possível iniciar o Live View.\n" +
                    "Verifique se a câmera permite Live View no modo atual.",
                    "Live View", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void StopLiveView()
        {
            timerLiveView.Stop();
            _isLiveViewActive = false;

            await Task.Run(() => _nikonManager.StopLiveView());

            picLiveView.Image = null;
            btnLiveView.Text = "Live View ON";
            lblLiveViewInfo.Text = "Live View desativado";
            SetStatus("Live View desativado");
        }

        private async void timerLiveView_Tick(object sender, EventArgs e)
        {
            if (!_isLiveViewActive || !_nikonManager.IsConnected)
                return;

            timerLiveView.Stop(); // Parar timer durante processamento

            try
            {
                byte[] imageData = await Task.Run(() => _nikonManager.GetLiveViewImage());

                if (imageData != null && imageData.Length > 0)
                {
                    using (var ms = new MemoryStream(imageData))
                    {
                        var oldImage = picLiveView.Image;
                        picLiveView.Image = Image.FromStream(ms);
                        oldImage?.Dispose();
                    }
                }
            }
            catch
            {
                // Ignorar erros de frame individuais
            }

            if (_isLiveViewActive)
                timerLiveView.Start(); // Reiniciar timer
        }

        #endregion

        #region Desconexão / Reconexão

        private void OnCameraDisconnected(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnCameraDisconnected(sender, e)));
                return;
            }

            // Parar timers
            timerLiveView.Stop();
            timerStatus.Stop();
            _debounceTimer.Stop();
            _isLiveViewActive = false;

            // Limpar UI
            UpdateUIState(false);
            picLiveView.Image = null;
            lblLiveViewInfo.Text = "Live View desativado";
            SetStatus("Câmera desconectada - aguardando reconexão...");

            // Limpar módulo antigo
            try { _nikonManager.UnloadModule(); } catch { }

            // Iniciar reconexão automática
            _reconnectTimer.Start();
        }

        #endregion

        #region Status Timer

        private async void timerStatus_Tick(object sender, EventArgs e)
        {
            if (!_nikonManager.IsConnected)
            {
                timerStatus.Stop();
                return;
            }

            timerStatus.Stop(); // Parar durante processamento

            try
            {
                // Executar no background para não bloquear UI e serializar com outras operações
                var result = await Task.Run(() =>
                {
                    _nikonManager.ProcessAsync();
                    int bat = _nikonManager.GetBatteryLevel();
                    return bat;
                });

                if (result >= 0 && result <= 100)
                {
                    progressBattery.Value = result;
                    lblBattery.Text = $"Bateria: {result}%";
                }
            }
            catch { }

            if (_nikonManager.IsConnected)
                timerStatus.Start(); // Reiniciar
        }

        #endregion

        #region Eventos do SDK

        private void OnCameraEvent(object sender, CameraEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnCameraEvent(sender, e)));
                return;
            }

            // Não sobrescrever mensagem importante (ex: erro de captura) com eventos genéricos
            if (DateTime.Now < _statusProtectedUntil)
            {
                // Status protegido, não sobrescrever
            }
            else if (e.EventType != eNkMAIDEvent.kNkMAIDEvent_CaptureComplete)
            {
                // Só mostrar eventos relevantes para o usuário
                if (e.EventType == eNkMAIDEvent.kNkMAIDEvent_AddChild ||
                    e.EventType == eNkMAIDEvent.kNkMAIDEvent_RemoveChild)
                {
                    SetStatus(e.Message);
                }
            }

            // Debounce: agendar atualização de controles (máx 1x a cada 2s)
            if (!_isUpdatingControls &&
                (e.EventType == eNkMAIDEvent.kNkMAIDEvent_CapChange ||
                 e.EventType == eNkMAIDEvent.kNkMAIDEvent_CapChangeOperationOnly))
            {
                _capChangePending = true;
                _debounceTimer.Stop();
                _debounceTimer.Start();
            }
        }

        private void OnImageReady(object sender, ImageReadyEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnImageReady(sender, e)));
                return;
            }

            try
            {
                // Salvar imagem original
                string fileName = $"DSC_{DateTime.Now:yyyyMMdd_HHmmss}{Path.GetExtension(e.FileName)}";
                string fullPath = Path.Combine(_savePath, fileName);
                File.WriteAllBytes(fullPath, e.ImageData);
                SetStatus($"Imagem salva: {fileName}");

                // Mostrar imagem capturada no picturebox principal
                if (e.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                {
                    // Guardar original para preview/comparação
                    _originalImage?.Dispose();
                    using (var ms = new MemoryStream(e.ImageData))
                        _originalImage = new Bitmap(Image.FromStream(ms));

                    // Salvar versão pós-produção em disco se aplicável
                    if (_currentPreset != null && PostProduction.HasPostProduction(_currentPreset))
                    {
                        using (Bitmap processed = PostProduction.Apply(_originalImage, _currentPreset))
                        {
                            string ppFileName = Path.GetFileNameWithoutExtension(fileName) + "_pp.jpg";
                            string ppFullPath = Path.Combine(_savePath, ppFileName);
                            processed.Save(ppFullPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                        SetStatus($"Imagem salva: {fileName} (pós-produção aplicada)");
                    }

                    // Atualizar preview (mostra processada ou original)
                    UpdatePostProductionPreview();
                }
            }
            catch (Exception ex)
            {
                SetStatus($"Erro ao salvar imagem: {ex.Message}");
            }
        }

        private void OnProgress(object sender, ProgressEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnProgress(sender, e)));
                return;
            }

            if (e.Total > 0)
            {
                progressBar.Visible = true;
                progressBar.Value = e.Percent;

                if (e.Done >= e.Total)
                {
                    progressBar.Visible = false;
                }
            }
        }

        #endregion

        #region Presets

        private void LoadPresets()
        {
            _presets = PresetManager.Load();
            RefreshPresetCombo();
        }

        private void RefreshPresetCombo()
        {
            _isUpdatingControls = true;
            cboPreset.Items.Clear();
            cboPreset.Items.Add("(Nenhum)");
            foreach (var p in _presets)
                cboPreset.Items.Add(p.Nome);
            cboPreset.SelectedIndex = 0;
            _isUpdatingControls = false;
        }

        private void cboPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingControls) return;

            int idx = cboPreset.SelectedIndex;
            if (idx <= 0)
            {
                _currentPreset = null;
                UpdatePostProductionPreview();
                return;
            }

            var preset = _presets[idx - 1]; // -1 porque index 0 é "(Nenhum)"
            _currentPreset = preset;
            UpdatePostProductionPreview();
            if (_nikonManager.IsConnected)
                _ = ApplyPresetAsync(preset);
        }

        private async Task ApplyPresetAsync(MaterialPreset preset)
        {
            SetStatus($"Aplicando preset: {preset.Nome}...");

            try
            {
                await Task.Run(() =>
                {
                    // ISO (conforme PicStone: PorMaterial)
                    if (preset.ISOIndex >= 0)
                        _nikonManager.SetEnumCapability((uint)eNkMAIDCapability.kNkMAIDCapability_Sensitivity, preset.ISOIndex);

                    // Abertura
                    if (preset.ApertureIndex >= 0)
                        _nikonManager.SetEnumCapability((uint)eNkMAIDCapability.kNkMAIDCapability_Aperture, preset.ApertureIndex);

                    // Velocidade
                    if (preset.ShutterSpeedIndex >= 0)
                        _nikonManager.SetEnumCapability((uint)eNkMAIDCapability.kNkMAIDCapability_ShutterSpeed, preset.ShutterSpeedIndex);

                    // PictureControl
                    if (preset.PictureControlIndex >= 0)
                        _nikonManager.SetEnumCapability((uint)eNkMAIDCapability.kNkMAIDCapability_PictureControl, preset.PictureControlIndex);

                    // Diagnóstico: ler WBMode atual e opções disponíveis
                    List<string> wbOptions;
                    int wbCurrentIdx;
                    _nikonManager.GetEnumCapability((uint)eNkMAIDCapability.kNkMAIDCapability_WBMode, out wbOptions, out wbCurrentIdx);
                    LogUI($"ApplyPreset: WBMode ANTES: index={wbCurrentIdx}, total={wbOptions.Count}");
                    for (int i = 0; i < wbOptions.Count; i++)
                        LogUI($"  WBMode[{i}]={wbOptions[i]}{(i == wbCurrentIdx ? " <<<ATUAL" : "")}");

                    // Temperatura de cor (conforme PicStone)
                    LogUI($"ApplyPreset: Temperatura={preset.Temperatura}");
                    if (preset.Temperatura == 0)
                    {
                        // WBMode = Auto (index 0)
                        bool wbOk = _nikonManager.SetEnumCapability((uint)eNkMAIDCapability.kNkMAIDCapability_WBMode, 0);
                        LogUI($"ApplyPreset: WBMode=Auto(0) result={wbOk}");
                    }
                    else
                    {
                        // WBMode = Kelvin/Custom (index 13, conforme PicStone)
                        bool wbOk = _nikonManager.SetEnumCapability((uint)eNkMAIDCapability.kNkMAIDCapability_WBMode, 13);
                        LogUI($"ApplyPreset: WBMode=Kelvin(13) result={wbOk}");

                        // Verificar se WBMode mudou
                        _nikonManager.GetEnumCapability((uint)eNkMAIDCapability.kNkMAIDCapability_WBMode, out wbOptions, out wbCurrentIdx);
                        LogUI($"ApplyPreset: WBMode DEPOIS: index={wbCurrentIdx} = {(wbCurrentIdx >= 0 && wbCurrentIdx < wbOptions.Count ? wbOptions[wbCurrentIdx] : "?")}");

                        // D7500/D7200: WBTuneColorTempEx como Range (valor direto em Kelvin)
                        // D7100: WBTuneColorTemp como Enum (index-based) - tratado separadamente
                        string cameraName = _nikonManager.GetCameraName();
                        if (cameraName != null && cameraName.Contains("D7100"))
                        {
                            // D7100: index = temperatura - 1
                            bool tOk = _nikonManager.SetEnumCapability(
                                (uint)eNkMAIDCapability.kNkMAIDCapability_WBTuneColorTemp,
                                (int)preset.Temperatura - 1);
                            LogUI($"ApplyPreset: WBTuneColorTemp index={preset.Temperatura - 1} result={tOk}");
                        }
                        else
                        {
                            // D7500/D7200: valor direto em Kelvin
                            bool tOk = _nikonManager.SetRangeCapability(
                                (uint)eNkMAIDCapability.kNkMAIDCapability_WBTuneColorTempEx,
                                preset.Temperatura);
                            LogUI($"ApplyPreset: WBTuneColorTempEx={preset.Temperatura}K result={tOk}");

                            // Readback
                            double readVal, lo, hi; uint st;
                            _nikonManager.GetRangeCapability((uint)eNkMAIDCapability.kNkMAIDCapability_WBTuneColorTempEx,
                                out readVal, out lo, out hi, out st);
                            LogUI($"ApplyPreset: WBTuneColorTempEx READBACK={readVal}K (range {lo}-{hi})");
                        }
                    }
                });

                // Atualizar UI com as novas configurações
                PopulateAllControls();

                SetStatus($"Preset \"{preset.Nome}\" aplicado");
            }
            catch (Exception ex)
            {
                SetStatus($"Erro ao aplicar preset: {ex.Message}");
            }
        }

        private void btnPresets_Click(object sender, EventArgs e)
        {
            // Passar o nome do preset atualmente selecionado
            string currentName = (cboPreset.SelectedIndex > 0)
                ? _presets[cboPreset.SelectedIndex - 1].Nome
                : null;

            string savedName;
            using (var frm = new FrmPresets(_nikonManager, currentName))
            {
                frm.ShowDialog(this);
                savedName = frm.LastSavedPresetName;
            }

            // Recarregar presets (podem ter sido alterados)
            _presets = PresetManager.Load();
            RefreshPresetCombo();

            // Se um preset foi salvo, selecionar e aplicar à câmera
            if (!string.IsNullOrEmpty(savedName))
            {
                for (int i = 0; i < _presets.Count; i++)
                {
                    if (_presets[i].Nome == savedName)
                    {
                        _isUpdatingControls = true;
                        cboPreset.SelectedIndex = i + 1; // +1 por causa do "(Nenhum)"
                        _isUpdatingControls = false;
                        _currentPreset = _presets[i];
                        UpdatePostProductionPreview();
                        if (_nikonManager.IsConnected)
                            _ = ApplyPresetAsync(_presets[i]);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Retorna o preset ativo atual (para uso em pós-produção).
        /// </summary>
        public MaterialPreset CurrentPreset => _currentPreset;

        private void UpdatePostProductionPreview()
        {
            if (_originalImage == null)
                return;

            // Salvar estado atual como "antes" (para comparação com seta esquerda)
            _beforeImage?.Dispose();
            if (_processedImage != null)
                _beforeImage = new Bitmap(_processedImage);
            else
                _beforeImage = new Bitmap(_originalImage);

            if (_currentPreset != null && PostProduction.HasPostProduction(_currentPreset))
            {
                // Aplicar nova pós-produção
                _processedImage?.Dispose();
                _processedImage = PostProduction.Apply(_originalImage, _currentPreset);

                // Exibir versão processada
                var oldImage = picLiveView.Image;
                picLiveView.Image = new Bitmap(_processedImage);
                oldImage?.Dispose();

                lblLiveViewInfo.Text = "Pós-produção: ON  (\u2190 antes | depois \u2192)";
            }
            else
            {
                // Sem pós-produção: mostrar original
                _processedImage?.Dispose();
                _processedImage = null;

                var oldImage = picLiveView.Image;
                picLiveView.Image = new Bitmap(_originalImage);
                oldImage?.Dispose();

                lblLiveViewInfo.Text = "Foto capturada  (\u2190 antes | depois \u2192)";
            }
        }

        #endregion

        #region Comparação Antes/Depois

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Não interceptar setas em controles que precisam delas
            if (ActiveControl is ComboBox || ActiveControl is TrackBar)
                return base.ProcessCmdKey(ref msg, keyData);

            if (keyData == Keys.Left && _beforeImage != null)
            {
                // Mostrar como estava ANTES da última mudança de PP
                var oldImage = picLiveView.Image;
                picLiveView.Image = new Bitmap(_beforeImage);
                oldImage?.Dispose();
                lblLiveViewInfo.Text = "[\u25C0 ANTES]  (\u2190 antes | depois \u2192)";
                return true;
            }

            if (keyData == Keys.Right && _beforeImage != null)
            {
                // Mostrar estado atual (DEPOIS da última mudança de PP)
                Bitmap currentImage = _processedImage ?? _originalImage;
                if (currentImage != null)
                {
                    var oldImage = picLiveView.Image;
                    picLiveView.Image = new Bitmap(currentImage);
                    oldImage?.Dispose();
                    lblLiveViewInfo.Text = "[DEPOIS \u25B6]  (\u2190 antes | depois \u2192)";
                }
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region Helpers

        private void SetStatus(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => SetStatus(text)));
                return;
            }
            lblStatus.Text = text;
        }

        private static string _uiLogPath = Path.Combine(
            Path.GetDirectoryName(Application.ExecutablePath) ?? ".",
            "nikon_debug.log");

        private void LogUI(string message)
        {
            try
            {
                string line = $"[{DateTime.Now:HH:mm:ss.fff}] UI: {message}\r\n";
                File.AppendAllText(_uiLogPath, line);
            }
            catch { }
        }

        private void ResizePictureBox()
        {
            var panel = splitMain.Panel1;
            int panelW = panel.ClientSize.Width;
            int panelH = panel.ClientSize.Height;

            if (panelW <= 0 || panelH <= 0) return;

            // Proporção 3:2 (formato Nikon)
            const double aspectRatio = 3.0 / 2.0;

            int picW, picH;
            if ((double)panelW / panelH > aspectRatio)
            {
                // Panel mais largo que 3:2 → limitar pela altura
                picH = panelH;
                picW = (int)(panelH * aspectRatio);
            }
            else
            {
                // Panel mais alto que 3:2 → limitar pela largura
                picW = panelW;
                picH = (int)(panelW / aspectRatio);
            }

            // Centralizar no panel
            int x = (panelW - picW) / 2;
            int y = (panelH - picH) / 2;

            picLiveView.Location = new Point(x, y);
            picLiveView.Size = new Size(picW, picH);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _reconnectTimer.Stop();

            if (_isLiveViewActive)
                StopLiveView();

            if (_nikonManager.IsConnected)
                DoDisconnect();

            _originalImage?.Dispose();
            _processedImage?.Dispose();
            _beforeImage?.Dispose();

            base.OnFormClosing(e);
        }

        #endregion
    }
}
