using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text.RegularExpressions;
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

        // Crop
        private Point _cropStart;
        private Rectangle _cropRect;
        private bool _isCropping;
        private string _lastSavedFileName;
        private RectangleF _cropMask;      // máscara de crop em ratios (0-1) da imagem
        private bool _cropMaskActive;      // aplica crop automaticamente a novas fotos

        public MainForm()
        {
            InitializeComponent();
            LoadIcon();
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

            // PictureBox usa Dock=Fill com SizeMode=Zoom, não precisa de resize manual

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

            cboPreset.Enabled = connected;

            if (!connected)
            {
                lblCameraName.Text = "Câmera: Não conectada";
                progressBattery.Value = 0;
                lblLens.Text = "Lente: --";
            }
        }

        #endregion

        #region Popular Controles

        private void PopulateAllControls()
        {
            _isUpdatingControls = true;

            try
            {
                LogUI("  PopulateAllControls: GetCameraName");
                lblCameraName.Text = "Câmera: " + _nikonManager.GetCameraName();

                LogUI("  PopulateAllControls: GetBatteryLevel");
                int battery = _nikonManager.GetBatteryLevel();
                if (battery >= 0 && battery <= 100)
                    progressBattery.Value = battery;
                lblBattery.Text = $"Bateria: {battery}%";

                LogUI("  PopulateAllControls: GetLensInfo");
                lblLens.Text = "Lente: " + _nikonManager.GetLensInfo();

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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            PopulateAllControls();
            SetStatus("Controles atualizados");
        }

        #endregion

        #region Eventos dos Controles

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

            menuAutoFocus.Enabled = false;
            SetStatus("Focando...");

            bool success = await Task.Run(() => _nikonManager.AutoFocus());

            SetStatus(success ? "Foco OK" : "Erro no AutoFocus");
            menuAutoFocus.Enabled = true;
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
                        picLiveView.Image = Image.FromStream(ms, useEmbeddedColorManagement: true);
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
                // Naming: sequência ativa → "MATERIAL_001.ext", senão → "DSC_timestamp.ext"
                string ext = Path.GetExtension(e.FileName);
                string fileName;
                if (SequenciaAtiva)
                {
                    string numChapa = IncrementaSequencia();
                    if (numChapa != null)
                    {
                        string materialName = _currentPreset?.Nome ?? "DSC";
                        fileName = $"{materialName}_{numChapa}{ext}";
                    }
                    else
                    {
                        fileName = $"DSC_{DateTime.Now:yyyyMMdd_HHmmss}{ext}";
                    }
                }
                else
                {
                    fileName = $"DSC_{DateTime.Now:yyyyMMdd_HHmmss}{ext}";
                }
                string fullPath = Path.Combine(_savePath, fileName);
                bool isJpg = e.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase);
                bool hasPP = isJpg && _currentPreset != null && PostProduction.HasPostProduction(_currentPreset);

                if (isJpg)
                {
                    // Carregar original em memória para preview/reprocessamento
                    _originalImage?.Dispose();
                    using (var ms = new MemoryStream(e.ImageData))
                    using (var temp = Image.FromStream(ms, useEmbeddedColorManagement: true))
                        _originalImage = new Bitmap(temp);
                }

                if (hasPP)
                {
                    // Original fica como temporário na pasta da aplicação (só a última, sobrescreve anterior)
                    string tempPath = Path.Combine(
                        Path.GetDirectoryName(Application.ExecutablePath) ?? ".", "_temp_original.jpg");
                    File.WriteAllBytes(tempPath, e.ImageData);

                    // Arquivo principal já sai com pós-produção aplicada
                    using (Bitmap processed = PostProduction.Apply(_originalImage, _currentPreset))
                    {
                        var jpegCodec = ImageCodecInfo.GetImageEncoders()
                            .FirstOrDefault(c => c.MimeType == "image/jpeg");
                        if (jpegCodec != null)
                        {
                            var encParams = new EncoderParameters(1);
                            encParams.Param[0] = new EncoderParameter(Encoder.Quality, 95L);
                            processed.Save(fullPath, jpegCodec, encParams);
                        }
                        else
                        {
                            processed.Save(fullPath, ImageFormat.Jpeg);
                        }
                    }
                    _lastSavedFileName = fileName;
                    SetStatus($"Imagem salva: {fileName} (p\u00F3s-produ\u00E7\u00E3o aplicada)");
                }
                else
                {
                    // Sem pós-produção: salvar original como arquivo principal
                    File.WriteAllBytes(fullPath, e.ImageData);
                    _lastSavedFileName = fileName;
                    SetStatus($"Imagem salva: {fileName}");
                }

                if (isJpg)
                {
                    // Atualizar preview (mostra processada ou original, com crop se ativo)
                    UpdatePostProductionPreview();

                    // Auto-salvar crop se máscara ativa
                    if (_cropMaskActive)
                    {
                        Bitmap cropSource = _processedImage ?? _originalImage;
                        Rectangle cropRect = GetCropRectFromMask(cropSource);
                        if (cropRect.Width > 0 && cropRect.Height > 0)
                            SaveCropFile(cropSource, cropRect);
                    }
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
            _presets.Sort((a, b) => string.Compare(a.Nome, b.Nome, StringComparison.OrdinalIgnoreCase));
            RefreshPresetCombo();

            // Selecionar primeiro preset (ordem alfabética) se existir
            if (_presets.Count > 0)
            {
                _isUpdatingControls = true;
                cboPreset.SelectedIndex = 1;
                _isUpdatingControls = false;
                _currentPreset = _presets[0];
            }
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

        /// <summary>
        /// Busca o valor textual no enum da câmera e seta o índice raw correto.
        /// Se o texto não for encontrado, usa o índice fallback do preset.
        /// Isso permite que presets funcionem entre D7200 e D7500 (índices diferentes)
        /// e também com listas filtradas (ex: PictureControl sem Monochrome/Portrait).
        /// </summary>
        private void SetEnumByText(uint capId, string text, int fallbackIndex)
        {
            if (!_nikonManager.SetEnumByText(capId, text))
            {
                // Fallback: usar índice original (câmera pode ser a mesma)
                if (fallbackIndex >= 0)
                    _nikonManager.SetEnumCapability(capId, fallbackIndex);
            }
        }

        private async Task ApplyPresetAsync(MaterialPreset preset)
        {
            SetStatus($"Aplicando preset: {preset.Nome}...");

            try
            {
                await Task.Run(() =>
                {
                    // ISO - buscar por valor textual (índices diferem entre D7200 e D7500)
                    if (!string.IsNullOrEmpty(preset.ISOText))
                        SetEnumByText((uint)eNkMAIDCapability.kNkMAIDCapability_Sensitivity, preset.ISOText, preset.ISOIndex);

                    // Abertura
                    if (!string.IsNullOrEmpty(preset.ApertureText))
                        SetEnumByText((uint)eNkMAIDCapability.kNkMAIDCapability_Aperture, preset.ApertureText, preset.ApertureIndex);

                    // Velocidade
                    if (!string.IsNullOrEmpty(preset.ShutterSpeedText))
                        SetEnumByText((uint)eNkMAIDCapability.kNkMAIDCapability_ShutterSpeed, preset.ShutterSpeedText, preset.ShutterSpeedIndex);

                    // PictureControl
                    if (!string.IsNullOrEmpty(preset.PictureControlText))
                        SetEnumByText((uint)eNkMAIDCapability.kNkMAIDCapability_PictureControl, preset.PictureControlText, preset.PictureControlIndex);
                    else if (preset.PictureControlIndex >= 0)
                        _nikonManager.SetEnumCapability((uint)eNkMAIDCapability.kNkMAIDCapability_PictureControl, preset.PictureControlIndex);

                    // Temperatura de cor (conforme PicStone)
                    if (preset.Temperatura == 0)
                    {
                        // WBMode = Auto (index 0)
                        _nikonManager.SetEnumCapability((uint)eNkMAIDCapability.kNkMAIDCapability_WBMode, 0);
                    }
                    else
                    {
                        // WBMode = Kelvin/Custom (index 13, conforme PicStone)
                        _nikonManager.SetEnumCapability((uint)eNkMAIDCapability.kNkMAIDCapability_WBMode, 13);

                        // D7100: WBTuneColorTemp como Enum (index-based)
                        // D7500/D7200: WBTuneColorTempEx como Range (valor direto em Kelvin)
                        string cameraName = _nikonManager.GetCameraName();
                        if (cameraName != null && cameraName.Contains("D7100"))
                        {
                            _nikonManager.SetEnumCapability(
                                (uint)eNkMAIDCapability.kNkMAIDCapability_WBTuneColorTemp,
                                (int)preset.Temperatura - 1);
                        }
                        else
                        {
                            _nikonManager.SetRangeCapability(
                                (uint)eNkMAIDCapability.kNkMAIDCapability_WBTuneColorTempEx,
                                preset.Temperatura);
                        }
                    }

                    // Auto Foco / Manual
                    if (preset.AutoFoco)
                        _nikonManager.EnableAutoFocus();
                    else
                        _nikonManager.EnableManualFocus();
                });

                // Atualizar checkbox AF na UI
                _isUpdatingControls = true;
                chkAF.Checked = preset.AutoFoco;
                chkAF.Text = preset.AutoFoco ? "AF" : "MF";
                _isUpdatingControls = false;

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

            // Aplicar crop se m\u00E1scara ativa
            if (_cropMaskActive)
                ApplyCropToDisplay();
        }

        #endregion

        #region Comparação Antes/Depois

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Não interceptar setas em controles que precisam delas
            if (ActiveControl is ComboBox || ActiveControl is TextBox || ActiveControl is RichTextBox)
                return base.ProcessCmdKey(ref msg, keyData);

            if (keyData == Keys.Left && _beforeImage != null)
            {
                // Mostrar como estava ANTES da última mudança de PP
                var oldImage = picLiveView.Image;
                Bitmap display = new Bitmap(_beforeImage);
                if (_cropMaskActive)
                {
                    Rectangle rect = GetCropRectFromMask(_beforeImage);
                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        Bitmap cropped = CropBitmap(_beforeImage, rect);
                        display.Dispose();
                        display = cropped;
                    }
                }
                picLiveView.Image = display;
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
                    Bitmap display = new Bitmap(currentImage);
                    if (_cropMaskActive)
                    {
                        Rectangle rect = GetCropRectFromMask(currentImage);
                        if (rect.Width > 0 && rect.Height > 0)
                        {
                            Bitmap cropped = CropBitmap(currentImage, rect);
                            display.Dispose();
                            display = cropped;
                        }
                    }
                    picLiveView.Image = display;
                    oldImage?.Dispose();
                    lblLiveViewInfo.Text = "[DEPOIS \u25B6]  (\u2190 antes | depois \u2192)";
                }
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region Crop

        private void picLiveView_MouseDown(object sender, MouseEventArgs e)
        {
            if (_originalImage == null || _isLiveViewActive) return;

            // Botão direito: remover crop e mostrar imagem completa
            if (e.Button == MouseButtons.Right && _cropMaskActive)
            {
                _cropMaskActive = false;
                ShowFullImage();
                return;
            }

            if (e.Button != MouseButtons.Left) return;

            _isCropping = true;
            _cropStart = e.Location;
            _cropRect = Rectangle.Empty;
            picLiveView.Cursor = Cursors.Cross;
        }

        private void picLiveView_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isCropping) return;

            int x = Math.Min(_cropStart.X, e.X);
            int y = Math.Min(_cropStart.Y, e.Y);
            int w = Math.Abs(e.X - _cropStart.X);
            int h = Math.Abs(e.Y - _cropStart.Y);
            _cropRect = new Rectangle(x, y, w, h);
            picLiveView.Invalidate();
        }

        private void picLiveView_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_isCropping) return;

            _isCropping = false;
            picLiveView.Cursor = Cursors.Default;

            if (_cropRect.Width > 10 && _cropRect.Height > 10)
                ApplyCrop();

            _cropRect = Rectangle.Empty;
            picLiveView.Invalidate();
        }

        private void picLiveView_Paint(object sender, PaintEventArgs e)
        {
            if (_cropRect.Width > 0 && _cropRect.Height > 0)
            {
                using (var pen = new Pen(Color.Red, 2f) { DashStyle = DashStyle.Dash })
                    e.Graphics.DrawRectangle(pen, _cropRect);
            }
        }

        /// <summary>
        /// Converte coordenadas do PictureBox (SizeMode=Zoom) para coordenadas da imagem real.
        /// </summary>
        private Rectangle TranslateToImageCoords(Rectangle picRect, Image img)
        {
            float picW = picLiveView.ClientSize.Width;
            float picH = picLiveView.ClientSize.Height;
            float imgW = img.Width;
            float imgH = img.Height;

            float zoom = Math.Min(picW / imgW, picH / imgH);
            float dispW = imgW * zoom;
            float dispH = imgH * zoom;
            float offX = (picW - dispW) / 2f;
            float offY = (picH - dispH) / 2f;

            int x = (int)((picRect.X - offX) / zoom);
            int y = (int)((picRect.Y - offY) / zoom);
            int w = (int)(picRect.Width / zoom);
            int h = (int)(picRect.Height / zoom);

            // Clamp nos limites da imagem
            x = Math.Max(0, Math.Min(x, (int)imgW - 1));
            y = Math.Max(0, Math.Min(y, (int)imgH - 1));
            w = Math.Min(w, (int)imgW - x);
            h = Math.Min(h, (int)imgH - y);

            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        /// Converte a máscara de ratios (0-1) para retângulo em pixels da imagem.
        /// </summary>
        private Rectangle GetCropRectFromMask(Bitmap img)
        {
            int x = (int)(_cropMask.X * img.Width);
            int y = (int)(_cropMask.Y * img.Height);
            int w = (int)(_cropMask.Width * img.Width);
            int h = (int)(_cropMask.Height * img.Height);

            x = Math.Max(0, Math.Min(x, img.Width - 1));
            y = Math.Max(0, Math.Min(y, img.Height - 1));
            w = Math.Min(w, img.Width - x);
            h = Math.Min(h, img.Height - y);

            return new Rectangle(x, y, w, h);
        }

        private Bitmap CropBitmap(Bitmap source, Rectangle rect)
        {
            var cropped = new Bitmap(rect.Width, rect.Height);
            using (var g = Graphics.FromImage(cropped))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(source,
                    new Rectangle(0, 0, rect.Width, rect.Height),
                    rect,
                    GraphicsUnit.Pixel);
            }
            return cropped;
        }

        /// <summary>
        /// Aplica o crop desenhado pelo usuário: salva arquivo, armazena máscara e exibe no PictureBox.
        /// </summary>
        private void ApplyCrop()
        {
            Bitmap source = _processedImage ?? _originalImage;
            if (source == null) return;

            Rectangle imgRect = TranslateToImageCoords(_cropRect, source);
            if (imgRect.Width < 1 || imgRect.Height < 1) return;

            // Armazenar máscara como ratios (0-1) para reutilizar em novas fotos
            _cropMask = new RectangleF(
                (float)imgRect.X / source.Width,
                (float)imgRect.Y / source.Height,
                (float)imgRect.Width / source.Width,
                (float)imgRect.Height / source.Height);
            _cropMaskActive = true;

            // Salvar crop em disco
            SaveCropFile(source, imgRect);

            // Exibir imagem cropada no PictureBox
            using (var cropped = CropBitmap(source, imgRect))
            {
                var old = picLiveView.Image;
                picLiveView.Image = new Bitmap(cropped);
                old?.Dispose();
            }

            lblLiveViewInfo.Text = "Crop ativo - bot\u00E3o direito para voltar";
        }

        /// <summary>
        /// Aplica a máscara de crop ao que está exibido no PictureBox.
        /// </summary>
        private void ApplyCropToDisplay()
        {
            Bitmap source = _processedImage ?? _originalImage;
            if (source == null) return;

            Rectangle rect = GetCropRectFromMask(source);
            if (rect.Width < 1 || rect.Height < 1) return;

            using (var cropped = CropBitmap(source, rect))
            {
                var old = picLiveView.Image;
                picLiveView.Image = new Bitmap(cropped);
                old?.Dispose();
            }

            lblLiveViewInfo.Text = "Crop ativo - bot\u00E3o direito para voltar";
        }

        /// <summary>
        /// Mostra a imagem completa (sem crop) no PictureBox.
        /// </summary>
        private void ShowFullImage()
        {
            Bitmap source = _processedImage ?? _originalImage;
            if (source == null) return;

            var old = picLiveView.Image;
            picLiveView.Image = new Bitmap(source);
            old?.Dispose();

            lblLiveViewInfo.Text = "Foto capturada  (\u2190 antes | depois \u2192)";
        }

        /// <summary>
        /// Salva o crop como JPEG qualidade 95 na pasta CROP.
        /// </summary>
        private void SaveCropFile(Bitmap source, Rectangle imgRect)
        {
            string cropDir = Path.Combine(_savePath, "CROP");
            if (!Directory.Exists(cropDir))
                Directory.CreateDirectory(cropDir);

            string baseName = !string.IsNullOrEmpty(_lastSavedFileName)
                ? Path.GetFileNameWithoutExtension(_lastSavedFileName)
                : $"CROP_{DateTime.Now:yyyyMMdd_HHmmss}";

            string cropPath = Path.Combine(cropDir, baseName + "_crop.jpg");

            using (var cropped = CropBitmap(source, imgRect))
            {
                var jpegCodec = ImageCodecInfo.GetImageEncoders()
                    .FirstOrDefault(c => c.MimeType == "image/jpeg");
                if (jpegCodec != null)
                {
                    var encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 95L);
                    cropped.Save(cropPath, jpegCodec, encoderParams);
                }
                else
                {
                    cropped.Save(cropPath, ImageFormat.Jpeg);
                }
            }

            SetStatus($"Crop salvo: {Path.GetFileName(cropPath)}");
        }

        #endregion

        #region Sequência de Chapas

        private List<string> _seqNumeros;  // números expandidos ("1","2","3"...)
        private List<Color> _seqCores;     // verde=pendente, vermelho=feito
        private int _seqIndice = -1;       // posição atual (-1 = não iniciado)

        private void btnSeqOk_Click(object sender, EventArgs e)
        {
            string input = txtSequencia.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                // Limpar sequência
                _seqNumeros = null;
                _seqCores = null;
                _seqIndice = -1;
                rtbSequencia.Clear();
                lblSeqInfo.Text = "";
                btnSeqUndo.Enabled = false;
                return;
            }

            var numeros = ParseSequencia(input);
            if (numeros == null || numeros.Count == 0)
            {
                MessageBox.Show("Formato inválido.\n\nExemplos:\n  1-20\n  1,3,5-10,15\n  10-5 (reverso)",
                    "Sequência", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verificar duplicatas - bloqueia até o usuário corrigir
            var duplicatas = numeros.GroupBy(n => n).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicatas.Count > 0)
            {
                MessageBox.Show(
                    $"Números repetidos: {string.Join(", ", duplicatas)}\n\n" +
                    "Corrija a sequência antes de continuar.",
                    "Duplicatas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSequencia.Focus();
                return;
            }

            _seqNumeros = numeros;
            _seqCores = new List<Color>();
            for (int i = 0; i < numeros.Count; i++)
                _seqCores.Add(Color.Green);
            _seqIndice = -1;

            AtualizarRichText();
            btnSeqUndo.Enabled = false;
        }

        /// <summary>
        /// Parseia string de sequência suportando ranges (1-20, 10-5) e separadores (,;/)
        /// </summary>
        private List<string> ParseSequencia(string input)
        {
            var result = new List<string>();

            // Substituir separadores por vírgula
            string normalized = input.Replace(";", ",").Replace("/", ",");
            string[] parts = normalized.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string part in parts)
            {
                string trimmed = part.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                // Verificar se é range (contém "-" mas não é número negativo)
                int dashIndex = trimmed.IndexOf('-', 1); // pular possível sinal negativo
                if (dashIndex > 0)
                {
                    string leftStr = trimmed.Substring(0, dashIndex).Trim();
                    string rightStr = trimmed.Substring(dashIndex + 1).Trim();

                    int left, right;
                    if (!int.TryParse(leftStr, out left) || !int.TryParse(rightStr, out right))
                        return null; // formato inválido

                    if (left <= right)
                    {
                        for (int i = left; i <= right; i++)
                            result.Add(i.ToString());
                    }
                    else
                    {
                        // Range reverso (ex: 10-5)
                        for (int i = left; i >= right; i--)
                            result.Add(i.ToString());
                    }
                }
                else
                {
                    int num;
                    if (!int.TryParse(trimmed, out num))
                        return null; // formato inválido
                    result.Add(num.ToString());
                }
            }

            return result;
        }

        /// <summary>
        /// Atualiza o RichTextBox com os números coloridos (verde=pendente, vermelho=feito).
        /// </summary>
        private void AtualizarRichText()
        {
            if (_seqNumeros == null) return;

            rtbSequencia.Clear();
            for (int i = 0; i < _seqNumeros.Count; i++)
            {
                if (i > 0)
                    rtbSequencia.AppendText("  ");

                rtbSequencia.SelectionStart = rtbSequencia.TextLength;
                rtbSequencia.SelectionLength = 0;
                rtbSequencia.SelectionColor = _seqCores[i];
                rtbSequencia.SelectionFont = new Font("Consolas", 9.75f,
                    _seqCores[i] == Color.Red ? FontStyle.Bold : FontStyle.Regular);
                rtbSequencia.AppendText(_seqNumeros[i].PadLeft(3, '0'));
            }

            int total = _seqNumeros.Count;
            int restantes = _seqCores.Count(c => c == Color.Green);
            lblSeqInfo.Text = $"Total: {total} | Restantes: {restantes}";
        }

        /// <summary>
        /// Avança para o próximo número da sequência. Retorna o número formatado (3 dígitos) ou null se acabou.
        /// </summary>
        private string IncrementaSequencia()
        {
            if (_seqNumeros == null) return null;

            _seqIndice++;
            if (_seqIndice >= _seqNumeros.Count)
            {
                _seqIndice = _seqNumeros.Count; // manter no fim
                MessageBox.Show("Todas as chapas foram fotografadas!",
                    "Sequência Completa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            // Marcar vermelho
            _seqCores[_seqIndice] = Color.Red;
            AtualizarRichText();
            btnSeqUndo.Enabled = true;

            // Retornar número formatado com 3 dígitos
            int num;
            if (int.TryParse(_seqNumeros[_seqIndice], out num))
                return num.ToString("D3");
            return _seqNumeros[_seqIndice].PadLeft(3, '0');
        }

        private void btnSeqUndo_Click(object sender, EventArgs e)
        {
            if (_seqNumeros == null || _seqIndice < 0) return;

            if (_seqIndice < _seqNumeros.Count)
            {
                // Voltar último vermelho → verde
                _seqCores[_seqIndice] = Color.Green;
            }
            _seqIndice--;
            AtualizarRichText();

            btnSeqUndo.Enabled = (_seqIndice >= 0);
            SetStatus("Última chapa desfeita");
        }

        /// <summary>
        /// Verifica se a sequência está ativa e ainda tem chapas pendentes.
        /// </summary>
        private bool SequenciaAtiva
        {
            get { return _seqNumeros != null && (_seqIndice + 1) < _seqNumeros.Count; }
        }

        #endregion

        #region Helpers

        private void LoadIcon()
        {
            string iconPath = Path.Combine(
                Path.GetDirectoryName(Application.ExecutablePath) ?? ".", "PicStone.ico");
            if (File.Exists(iconPath))
                Icon = new System.Drawing.Icon(iconPath);
        }

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
