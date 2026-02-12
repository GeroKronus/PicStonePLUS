namespace PicStonePlus.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
                if (_nikonManager != null)
                    _nikonManager.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuArquivo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuConectar = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDesconectar = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSair = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCamera = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCapturar = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAutoFocus = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLiveView = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.picLiveView = new System.Windows.Forms.PictureBox();
            this.lblLiveViewInfo = new System.Windows.Forms.Label();
            this.panelRight = new System.Windows.Forms.Panel();
            this.grpPictureControl = new System.Windows.Forms.GroupBox();
            this.lblPicCtrl = new System.Windows.Forms.Label();
            this.cboPictureControl = new System.Windows.Forms.ComboBox();
            this.grpFocus = new System.Windows.Forms.GroupBox();
            this.lblFocusMode = new System.Windows.Forms.Label();
            this.cboFocusMode = new System.Windows.Forms.ComboBox();
            this.lblAFArea = new System.Windows.Forms.Label();
            this.cboAFArea = new System.Windows.Forms.ComboBox();
            this.btnAutoFocus = new System.Windows.Forms.Button();
            this.grpWhiteBalance = new System.Windows.Forms.GroupBox();
            this.lblWBMode = new System.Windows.Forms.Label();
            this.cboWBMode = new System.Windows.Forms.ComboBox();
            this.grpExposure = new System.Windows.Forms.GroupBox();
            this.lblExposureMode = new System.Windows.Forms.Label();
            this.cboExposureMode = new System.Windows.Forms.ComboBox();
            this.lblShutter = new System.Windows.Forms.Label();
            this.cboShutterSpeed = new System.Windows.Forms.ComboBox();
            this.lblAperture = new System.Windows.Forms.Label();
            this.cboAperture = new System.Windows.Forms.ComboBox();
            this.lblISO = new System.Windows.Forms.Label();
            this.cboISO = new System.Windows.Forms.ComboBox();
            this.lblExpComp = new System.Windows.Forms.Label();
            this.trackExpComp = new System.Windows.Forms.TrackBar();
            this.lblExpCompValue = new System.Windows.Forms.Label();
            this.lblMetering = new System.Windows.Forms.Label();
            this.cboMetering = new System.Windows.Forms.ComboBox();
            this.grpCameraInfo = new System.Windows.Forms.GroupBox();
            this.lblCameraName = new System.Windows.Forms.Label();
            this.lblBattery = new System.Windows.Forms.Label();
            this.progressBattery = new System.Windows.Forms.ProgressBar();
            this.lblLens = new System.Windows.Forms.Label();
            this.panelActions = new System.Windows.Forms.Panel();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnCapture = new System.Windows.Forms.Button();
            this.btnLiveView = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.chkAF = new System.Windows.Forms.CheckBox();
            this.grpPreset = new System.Windows.Forms.GroupBox();
            this.lblPreset = new System.Windows.Forms.Label();
            this.cboPreset = new System.Windows.Forms.ComboBox();
            this.btnPresets = new System.Windows.Forms.Button();
            this.menuPresets = new System.Windows.Forms.ToolStripMenuItem();
            this.timerLiveView = new System.Windows.Forms.Timer(this.components);
            this.timerStatus = new System.Windows.Forms.Timer(this.components);
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLiveView)).BeginInit();
            this.panelRight.SuspendLayout();
            this.grpPreset.SuspendLayout();
            this.grpPictureControl.SuspendLayout();
            this.grpFocus.SuspendLayout();
            this.grpWhiteBalance.SuspendLayout();
            this.grpExposure.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackExpComp)).BeginInit();
            this.grpCameraInfo.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuArquivo,
            this.menuCamera});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1200, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";
            // 
            // menuArquivo
            // 
            this.menuArquivo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuConectar,
            this.menuDesconectar,
            this.menuSep1,
            this.menuSair});
            this.menuArquivo.Name = "menuArquivo";
            this.menuArquivo.Size = new System.Drawing.Size(61, 20);
            this.menuArquivo.Text = "&Arquivo";
            // 
            // menuConectar
            // 
            this.menuConectar.Name = "menuConectar";
            this.menuConectar.Size = new System.Drawing.Size(166, 22);
            this.menuConectar.Text = "&Conectar Câmera";
            this.menuConectar.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // menuDesconectar
            // 
            this.menuDesconectar.Enabled = false;
            this.menuDesconectar.Name = "menuDesconectar";
            this.menuDesconectar.Size = new System.Drawing.Size(166, 22);
            this.menuDesconectar.Text = "&Desconectar";
            this.menuDesconectar.Click += new System.EventHandler(this.menuDesconectar_Click);
            // 
            // menuSep1
            // 
            this.menuSep1.Name = "menuSep1";
            this.menuSep1.Size = new System.Drawing.Size(163, 6);
            // 
            // menuSair
            // 
            this.menuSair.Name = "menuSair";
            this.menuSair.Size = new System.Drawing.Size(166, 22);
            this.menuSair.Text = "&Sair";
            this.menuSair.Click += new System.EventHandler(this.menuSair_Click);
            // 
            // menuCamera
            // 
            this.menuCamera.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuCapturar,
            this.menuAutoFocus,
            this.menuLiveView,
            this.menuPresets});
            this.menuCamera.Name = "menuCamera";
            this.menuCamera.Size = new System.Drawing.Size(60, 20);
            this.menuCamera.Text = "&Câmera";
            //
            // menuPresets
            //
            this.menuPresets.Name = "menuPresets";
            this.menuPresets.Size = new System.Drawing.Size(189, 22);
            this.menuPresets.Text = "&Presets de Material";
            this.menuPresets.Click += new System.EventHandler(this.btnPresets_Click);
            // 
            // menuCapturar
            // 
            this.menuCapturar.Name = "menuCapturar";
            this.menuCapturar.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.menuCapturar.Size = new System.Drawing.Size(189, 22);
            this.menuCapturar.Text = "&Capturar";
            this.menuCapturar.Click += new System.EventHandler(this.btnCapture_Click);
            // 
            // menuAutoFocus
            // 
            this.menuAutoFocus.Name = "menuAutoFocus";
            this.menuAutoFocus.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.menuAutoFocus.Size = new System.Drawing.Size(189, 22);
            this.menuAutoFocus.Text = "&AutoFocus";
            this.menuAutoFocus.Click += new System.EventHandler(this.btnAutoFocus_Click);
            // 
            // menuLiveView
            // 
            this.menuLiveView.Name = "menuLiveView";
            this.menuLiveView.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.menuLiveView.Size = new System.Drawing.Size(189, 22);
            this.menuLiveView.Text = "&Live View ON/OFF";
            this.menuLiveView.Click += new System.EventHandler(this.btnLiveView_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.progressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 728);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1200, 22);
            this.statusStrip.TabIndex = 1;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(1185, 17);
            this.lblStatus.Spring = true;
            this.lblStatus.Text = "Desconectado";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(200, 18);
            this.progressBar.Visible = false;
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 24);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.BackColor = System.Drawing.Color.Black;
            this.splitMain.Panel1.Controls.Add(this.picLiveView);
            this.splitMain.Panel1.Controls.Add(this.lblLiveViewInfo);
            this.splitMain.Panel1MinSize = 320;
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.panelRight);
            this.splitMain.Panel2.Controls.Add(this.panelActions);
            this.splitMain.Panel2MinSize = 320;
            this.splitMain.Size = new System.Drawing.Size(1200, 704);
            this.splitMain.SplitterDistance = 640;
            this.splitMain.TabIndex = 2;
            // 
            // picLiveView
            // 
            this.picLiveView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.picLiveView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picLiveView.Location = new System.Drawing.Point(0, 0);
            this.picLiveView.Name = "picLiveView";
            this.picLiveView.Size = new System.Drawing.Size(640, 427);
            this.picLiveView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLiveView.TabIndex = 0;
            this.picLiveView.TabStop = false;
            // 
            // lblLiveViewInfo
            // 
            this.lblLiveViewInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblLiveViewInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblLiveViewInfo.ForeColor = System.Drawing.Color.White;
            this.lblLiveViewInfo.Location = new System.Drawing.Point(0, 684);
            this.lblLiveViewInfo.Name = "lblLiveViewInfo";
            this.lblLiveViewInfo.Size = new System.Drawing.Size(640, 20);
            this.lblLiveViewInfo.TabIndex = 1;
            this.lblLiveViewInfo.Text = "Live View desativado";
            this.lblLiveViewInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelRight
            // 
            this.panelRight.AutoScroll = true;
            this.panelRight.Controls.Add(this.grpPictureControl);
            this.panelRight.Controls.Add(this.grpFocus);
            this.panelRight.Controls.Add(this.grpWhiteBalance);
            this.panelRight.Controls.Add(this.grpExposure);
            this.panelRight.Controls.Add(this.grpPreset);
            this.panelRight.Controls.Add(this.grpCameraInfo);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(0, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Padding = new System.Windows.Forms.Padding(8);
            this.panelRight.Size = new System.Drawing.Size(556, 594);
            this.panelRight.TabIndex = 0;
            // 
            // grpPictureControl
            // 
            this.grpPictureControl.Controls.Add(this.lblPicCtrl);
            this.grpPictureControl.Controls.Add(this.cboPictureControl);
            this.grpPictureControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpPictureControl.Location = new System.Drawing.Point(8, 493);
            this.grpPictureControl.Name = "grpPictureControl";
            this.grpPictureControl.Size = new System.Drawing.Size(540, 55);
            this.grpPictureControl.TabIndex = 4;
            this.grpPictureControl.TabStop = false;
            this.grpPictureControl.Text = "Picture Control";
            // 
            // lblPicCtrl
            // 
            this.lblPicCtrl.Location = new System.Drawing.Point(12, 22);
            this.lblPicCtrl.Name = "lblPicCtrl";
            this.lblPicCtrl.Size = new System.Drawing.Size(80, 20);
            this.lblPicCtrl.TabIndex = 0;
            this.lblPicCtrl.Text = "Preset:";
            // 
            // cboPictureControl
            // 
            this.cboPictureControl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPictureControl.FormattingEnabled = true;
            this.cboPictureControl.Location = new System.Drawing.Point(95, 20);
            this.cboPictureControl.Name = "cboPictureControl";
            this.cboPictureControl.Size = new System.Drawing.Size(230, 23);
            this.cboPictureControl.TabIndex = 1;
            this.cboPictureControl.SelectedIndexChanged += new System.EventHandler(this.cboPictureControl_SelectedIndexChanged);
            // 
            // grpFocus
            // 
            this.grpFocus.Controls.Add(this.lblFocusMode);
            this.grpFocus.Controls.Add(this.cboFocusMode);
            this.grpFocus.Controls.Add(this.lblAFArea);
            this.grpFocus.Controls.Add(this.cboAFArea);
            this.grpFocus.Controls.Add(this.btnAutoFocus);
            this.grpFocus.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpFocus.Location = new System.Drawing.Point(8, 393);
            this.grpFocus.Name = "grpFocus";
            this.grpFocus.Size = new System.Drawing.Size(540, 100);
            this.grpFocus.TabIndex = 3;
            this.grpFocus.TabStop = false;
            this.grpFocus.Text = "Foco";
            // 
            // lblFocusMode
            // 
            this.lblFocusMode.Location = new System.Drawing.Point(12, 22);
            this.lblFocusMode.Name = "lblFocusMode";
            this.lblFocusMode.Size = new System.Drawing.Size(80, 20);
            this.lblFocusMode.TabIndex = 0;
            this.lblFocusMode.Text = "Modo:";
            // 
            // cboFocusMode
            // 
            this.cboFocusMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFocusMode.FormattingEnabled = true;
            this.cboFocusMode.Location = new System.Drawing.Point(95, 20);
            this.cboFocusMode.Name = "cboFocusMode";
            this.cboFocusMode.Size = new System.Drawing.Size(230, 23);
            this.cboFocusMode.TabIndex = 1;
            this.cboFocusMode.SelectedIndexChanged += new System.EventHandler(this.cboFocusMode_SelectedIndexChanged);
            // 
            // lblAFArea
            // 
            this.lblAFArea.Location = new System.Drawing.Point(12, 52);
            this.lblAFArea.Name = "lblAFArea";
            this.lblAFArea.Size = new System.Drawing.Size(80, 20);
            this.lblAFArea.TabIndex = 2;
            this.lblAFArea.Text = "Área AF:";
            // 
            // cboAFArea
            // 
            this.cboAFArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAFArea.FormattingEnabled = true;
            this.cboAFArea.Location = new System.Drawing.Point(95, 50);
            this.cboAFArea.Name = "cboAFArea";
            this.cboAFArea.Size = new System.Drawing.Size(140, 23);
            this.cboAFArea.TabIndex = 3;
            this.cboAFArea.SelectedIndexChanged += new System.EventHandler(this.cboAFArea_SelectedIndexChanged);
            // 
            // btnAutoFocus
            // 
            this.btnAutoFocus.Location = new System.Drawing.Point(245, 48);
            this.btnAutoFocus.Name = "btnAutoFocus";
            this.btnAutoFocus.Size = new System.Drawing.Size(80, 28);
            this.btnAutoFocus.TabIndex = 4;
            this.btnAutoFocus.Text = "AF";
            this.btnAutoFocus.UseVisualStyleBackColor = true;
            this.btnAutoFocus.Click += new System.EventHandler(this.btnAutoFocus_Click);
            // 
            // grpWhiteBalance
            // 
            this.grpWhiteBalance.Controls.Add(this.lblWBMode);
            this.grpWhiteBalance.Controls.Add(this.cboWBMode);
            this.grpWhiteBalance.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpWhiteBalance.Location = new System.Drawing.Point(8, 338);
            this.grpWhiteBalance.Name = "grpWhiteBalance";
            this.grpWhiteBalance.Size = new System.Drawing.Size(540, 55);
            this.grpWhiteBalance.TabIndex = 2;
            this.grpWhiteBalance.TabStop = false;
            this.grpWhiteBalance.Text = "White Balance";
            // 
            // lblWBMode
            // 
            this.lblWBMode.Location = new System.Drawing.Point(12, 22);
            this.lblWBMode.Name = "lblWBMode";
            this.lblWBMode.Size = new System.Drawing.Size(80, 20);
            this.lblWBMode.TabIndex = 0;
            this.lblWBMode.Text = "Modo:";
            // 
            // cboWBMode
            // 
            this.cboWBMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWBMode.FormattingEnabled = true;
            this.cboWBMode.Location = new System.Drawing.Point(95, 20);
            this.cboWBMode.Name = "cboWBMode";
            this.cboWBMode.Size = new System.Drawing.Size(230, 23);
            this.cboWBMode.TabIndex = 1;
            this.cboWBMode.SelectedIndexChanged += new System.EventHandler(this.cboWBMode_SelectedIndexChanged);
            //
            // grpPreset
            //
            this.grpPreset.Controls.Add(this.lblPreset);
            this.grpPreset.Controls.Add(this.cboPreset);
            this.grpPreset.Controls.Add(this.btnPresets);
            this.grpPreset.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpPreset.Location = new System.Drawing.Point(8, 108);
            this.grpPreset.Name = "grpPreset";
            this.grpPreset.Size = new System.Drawing.Size(540, 55);
            this.grpPreset.TabIndex = 10;
            this.grpPreset.TabStop = false;
            this.grpPreset.Text = "Preset de Material";
            //
            // lblPreset
            //
            this.lblPreset.Location = new System.Drawing.Point(12, 22);
            this.lblPreset.Name = "lblPreset";
            this.lblPreset.Size = new System.Drawing.Size(80, 20);
            this.lblPreset.TabIndex = 0;
            this.lblPreset.Text = "Material:";
            //
            // cboPreset
            //
            this.cboPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPreset.FormattingEnabled = true;
            this.cboPreset.Location = new System.Drawing.Point(95, 20);
            this.cboPreset.Name = "cboPreset";
            this.cboPreset.Size = new System.Drawing.Size(230, 23);
            this.cboPreset.TabIndex = 1;
            this.cboPreset.SelectedIndexChanged += new System.EventHandler(this.cboPreset_SelectedIndexChanged);
            //
            // btnPresets
            //
            this.btnPresets.Location = new System.Drawing.Point(335, 19);
            this.btnPresets.Name = "btnPresets";
            this.btnPresets.Size = new System.Drawing.Size(90, 25);
            this.btnPresets.TabIndex = 2;
            this.btnPresets.Text = "Editar...";
            this.btnPresets.UseVisualStyleBackColor = true;
            this.btnPresets.Click += new System.EventHandler(this.btnPresets_Click);
            //
            // grpExposure
            //
            this.grpExposure.Controls.Add(this.lblExposureMode);
            this.grpExposure.Controls.Add(this.cboExposureMode);
            this.grpExposure.Controls.Add(this.lblShutter);
            this.grpExposure.Controls.Add(this.cboShutterSpeed);
            this.grpExposure.Controls.Add(this.lblAperture);
            this.grpExposure.Controls.Add(this.cboAperture);
            this.grpExposure.Controls.Add(this.lblISO);
            this.grpExposure.Controls.Add(this.cboISO);
            this.grpExposure.Controls.Add(this.lblExpComp);
            this.grpExposure.Controls.Add(this.trackExpComp);
            this.grpExposure.Controls.Add(this.lblExpCompValue);
            this.grpExposure.Controls.Add(this.lblMetering);
            this.grpExposure.Controls.Add(this.cboMetering);
            this.grpExposure.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpExposure.Location = new System.Drawing.Point(8, 108);
            this.grpExposure.Name = "grpExposure";
            this.grpExposure.Padding = new System.Windows.Forms.Padding(8);
            this.grpExposure.Size = new System.Drawing.Size(540, 230);
            this.grpExposure.TabIndex = 1;
            this.grpExposure.TabStop = false;
            this.grpExposure.Text = "Controles de Exposição";
            // 
            // lblExposureMode
            // 
            this.lblExposureMode.Location = new System.Drawing.Point(12, 22);
            this.lblExposureMode.Name = "lblExposureMode";
            this.lblExposureMode.Size = new System.Drawing.Size(80, 20);
            this.lblExposureMode.TabIndex = 0;
            this.lblExposureMode.Text = "Modo:";
            // 
            // cboExposureMode
            // 
            this.cboExposureMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboExposureMode.FormattingEnabled = true;
            this.cboExposureMode.Location = new System.Drawing.Point(95, 20);
            this.cboExposureMode.Name = "cboExposureMode";
            this.cboExposureMode.Size = new System.Drawing.Size(230, 23);
            this.cboExposureMode.TabIndex = 1;
            this.cboExposureMode.SelectedIndexChanged += new System.EventHandler(this.cboExposureMode_SelectedIndexChanged);
            // 
            // lblShutter
            // 
            this.lblShutter.Location = new System.Drawing.Point(12, 52);
            this.lblShutter.Name = "lblShutter";
            this.lblShutter.Size = new System.Drawing.Size(80, 20);
            this.lblShutter.TabIndex = 2;
            this.lblShutter.Text = "Velocidade:";
            // 
            // cboShutterSpeed
            // 
            this.cboShutterSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboShutterSpeed.FormattingEnabled = true;
            this.cboShutterSpeed.Location = new System.Drawing.Point(95, 50);
            this.cboShutterSpeed.Name = "cboShutterSpeed";
            this.cboShutterSpeed.Size = new System.Drawing.Size(230, 23);
            this.cboShutterSpeed.TabIndex = 3;
            this.cboShutterSpeed.SelectedIndexChanged += new System.EventHandler(this.cboShutterSpeed_SelectedIndexChanged);
            // 
            // lblAperture
            // 
            this.lblAperture.Location = new System.Drawing.Point(12, 82);
            this.lblAperture.Name = "lblAperture";
            this.lblAperture.Size = new System.Drawing.Size(80, 20);
            this.lblAperture.TabIndex = 4;
            this.lblAperture.Text = "Abertura:";
            // 
            // cboAperture
            // 
            this.cboAperture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAperture.FormattingEnabled = true;
            this.cboAperture.Location = new System.Drawing.Point(95, 80);
            this.cboAperture.Name = "cboAperture";
            this.cboAperture.Size = new System.Drawing.Size(230, 23);
            this.cboAperture.TabIndex = 5;
            this.cboAperture.SelectedIndexChanged += new System.EventHandler(this.cboAperture_SelectedIndexChanged);
            // 
            // lblISO
            // 
            this.lblISO.Location = new System.Drawing.Point(12, 112);
            this.lblISO.Name = "lblISO";
            this.lblISO.Size = new System.Drawing.Size(80, 20);
            this.lblISO.TabIndex = 6;
            this.lblISO.Text = "ISO:";
            // 
            // cboISO
            // 
            this.cboISO.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboISO.FormattingEnabled = true;
            this.cboISO.Location = new System.Drawing.Point(95, 110);
            this.cboISO.Name = "cboISO";
            this.cboISO.Size = new System.Drawing.Size(230, 23);
            this.cboISO.TabIndex = 7;
            this.cboISO.SelectedIndexChanged += new System.EventHandler(this.cboISO_SelectedIndexChanged);
            // 
            // lblExpComp
            // 
            this.lblExpComp.Location = new System.Drawing.Point(12, 142);
            this.lblExpComp.Name = "lblExpComp";
            this.lblExpComp.Size = new System.Drawing.Size(80, 20);
            this.lblExpComp.TabIndex = 8;
            this.lblExpComp.Text = "Comp. Exp:";
            // 
            // trackExpComp
            // 
            this.trackExpComp.Location = new System.Drawing.Point(95, 138);
            this.trackExpComp.Maximum = 30;
            this.trackExpComp.Minimum = -30;
            this.trackExpComp.Name = "trackExpComp";
            this.trackExpComp.Size = new System.Drawing.Size(180, 45);
            this.trackExpComp.TabIndex = 9;
            this.trackExpComp.TickFrequency = 5;
            this.trackExpComp.Scroll += new System.EventHandler(this.trackExpComp_Scroll);
            // 
            // lblExpCompValue
            // 
            this.lblExpCompValue.Location = new System.Drawing.Point(280, 142);
            this.lblExpCompValue.Name = "lblExpCompValue";
            this.lblExpCompValue.Size = new System.Drawing.Size(60, 20);
            this.lblExpCompValue.TabIndex = 10;
            this.lblExpCompValue.Text = "0.0 EV";
            // 
            // lblMetering
            // 
            this.lblMetering.Location = new System.Drawing.Point(12, 172);
            this.lblMetering.Name = "lblMetering";
            this.lblMetering.Size = new System.Drawing.Size(80, 20);
            this.lblMetering.TabIndex = 11;
            this.lblMetering.Text = "Medição:";
            // 
            // cboMetering
            // 
            this.cboMetering.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMetering.FormattingEnabled = true;
            this.cboMetering.Location = new System.Drawing.Point(95, 170);
            this.cboMetering.Name = "cboMetering";
            this.cboMetering.Size = new System.Drawing.Size(230, 23);
            this.cboMetering.TabIndex = 12;
            this.cboMetering.SelectedIndexChanged += new System.EventHandler(this.cboMetering_SelectedIndexChanged);
            // 
            // grpCameraInfo
            // 
            this.grpCameraInfo.Controls.Add(this.lblCameraName);
            this.grpCameraInfo.Controls.Add(this.lblBattery);
            this.grpCameraInfo.Controls.Add(this.progressBattery);
            this.grpCameraInfo.Controls.Add(this.lblLens);
            this.grpCameraInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpCameraInfo.Location = new System.Drawing.Point(8, 8);
            this.grpCameraInfo.Name = "grpCameraInfo";
            this.grpCameraInfo.Padding = new System.Windows.Forms.Padding(8);
            this.grpCameraInfo.Size = new System.Drawing.Size(540, 100);
            this.grpCameraInfo.TabIndex = 0;
            this.grpCameraInfo.TabStop = false;
            this.grpCameraInfo.Text = "Informações da Câmera";
            // 
            // lblCameraName
            // 
            this.lblCameraName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCameraName.Location = new System.Drawing.Point(12, 22);
            this.lblCameraName.Name = "lblCameraName";
            this.lblCameraName.Size = new System.Drawing.Size(300, 18);
            this.lblCameraName.TabIndex = 0;
            this.lblCameraName.Text = "Câmera: Não conectada";
            // 
            // lblBattery
            // 
            this.lblBattery.Location = new System.Drawing.Point(12, 44);
            this.lblBattery.Name = "lblBattery";
            this.lblBattery.Size = new System.Drawing.Size(60, 18);
            this.lblBattery.TabIndex = 1;
            this.lblBattery.Text = "Bateria:";
            // 
            // progressBattery
            // 
            this.progressBattery.Location = new System.Drawing.Point(72, 44);
            this.progressBattery.Name = "progressBattery";
            this.progressBattery.Size = new System.Drawing.Size(150, 18);
            this.progressBattery.TabIndex = 2;
            // 
            // lblLens
            // 
            this.lblLens.Location = new System.Drawing.Point(12, 66);
            this.lblLens.Name = "lblLens";
            this.lblLens.Size = new System.Drawing.Size(350, 18);
            this.lblLens.TabIndex = 3;
            this.lblLens.Text = "Lente: --";
            // 
            // panelActions
            // 
            this.panelActions.Controls.Add(this.btnConnect);
            this.panelActions.Controls.Add(this.btnCapture);
            this.panelActions.Controls.Add(this.btnLiveView);
            this.panelActions.Controls.Add(this.chkAF);
            this.panelActions.Controls.Add(this.btnRefresh);
            this.panelActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelActions.Location = new System.Drawing.Point(0, 594);
            this.panelActions.Name = "panelActions";
            this.panelActions.Padding = new System.Windows.Forms.Padding(8);
            this.panelActions.Size = new System.Drawing.Size(556, 110);
            this.panelActions.TabIndex = 1;
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConnect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnConnect.ForeColor = System.Drawing.Color.White;
            this.btnConnect.Location = new System.Drawing.Point(12, 8);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(100, 35);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Conectar";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnCapture
            // 
            this.btnCapture.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnCapture.Enabled = false;
            this.btnCapture.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCapture.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCapture.ForeColor = System.Drawing.Color.White;
            this.btnCapture.Location = new System.Drawing.Point(120, 8);
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new System.Drawing.Size(100, 35);
            this.btnCapture.TabIndex = 1;
            this.btnCapture.Text = "Capturar";
            this.btnCapture.UseVisualStyleBackColor = false;
            this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
            // 
            // btnLiveView
            // 
            this.btnLiveView.Enabled = false;
            this.btnLiveView.Location = new System.Drawing.Point(228, 8);
            this.btnLiveView.Name = "btnLiveView";
            this.btnLiveView.Size = new System.Drawing.Size(100, 35);
            this.btnLiveView.TabIndex = 2;
            this.btnLiveView.Text = "Live View ON";
            this.btnLiveView.UseVisualStyleBackColor = true;
            this.btnLiveView.Click += new System.EventHandler(this.btnLiveView_Click);
            //
            // chkAF
            //
            this.chkAF.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkAF.Checked = true;
            this.chkAF.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAF.Enabled = false;
            this.chkAF.Location = new System.Drawing.Point(336, 8);
            this.chkAF.Name = "chkAF";
            this.chkAF.Size = new System.Drawing.Size(60, 35);
            this.chkAF.TabIndex = 20;
            this.chkAF.Text = "AF";
            this.chkAF.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkAF.UseVisualStyleBackColor = true;
            this.chkAF.CheckedChanged += new System.EventHandler(this.chkAF_CheckedChanged);
            //
            // btnRefresh
            // 
            this.btnRefresh.Enabled = false;
            this.btnRefresh.Location = new System.Drawing.Point(12, 50);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 28);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Atualizar";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // 
            // timerLiveView
            // 
            this.timerLiveView.Interval = 33;
            this.timerLiveView.Tick += new System.EventHandler(this.timerLiveView_Tick);
            // 
            // timerStatus
            // 
            this.timerStatus.Interval = 2000;
            this.timerStatus.Tick += new System.EventHandler(this.timerStatus_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 750);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PicStone+";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picLiveView)).EndInit();
            this.panelRight.ResumeLayout(false);
            this.grpPictureControl.ResumeLayout(false);
            this.grpFocus.ResumeLayout(false);
            this.grpWhiteBalance.ResumeLayout(false);
            this.grpExposure.ResumeLayout(false);
            this.grpExposure.PerformLayout();
            this.grpPreset.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackExpComp)).EndInit();
            this.grpCameraInfo.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuArquivo;
        private System.Windows.Forms.ToolStripMenuItem menuConectar;
        private System.Windows.Forms.ToolStripMenuItem menuDesconectar;
        private System.Windows.Forms.ToolStripSeparator menuSep1;
        private System.Windows.Forms.ToolStripMenuItem menuSair;
        private System.Windows.Forms.ToolStripMenuItem menuCamera;
        private System.Windows.Forms.ToolStripMenuItem menuCapturar;
        private System.Windows.Forms.ToolStripMenuItem menuAutoFocus;
        private System.Windows.Forms.ToolStripMenuItem menuLiveView;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.PictureBox picLiveView;
        private System.Windows.Forms.Label lblLiveViewInfo;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.GroupBox grpCameraInfo;
        private System.Windows.Forms.Label lblCameraName;
        private System.Windows.Forms.Label lblBattery;
        private System.Windows.Forms.ProgressBar progressBattery;
        private System.Windows.Forms.Label lblLens;
        private System.Windows.Forms.GroupBox grpExposure;
        private System.Windows.Forms.Label lblExposureMode;
        private System.Windows.Forms.ComboBox cboExposureMode;
        private System.Windows.Forms.Label lblShutter;
        private System.Windows.Forms.ComboBox cboShutterSpeed;
        private System.Windows.Forms.Label lblAperture;
        private System.Windows.Forms.ComboBox cboAperture;
        private System.Windows.Forms.Label lblISO;
        private System.Windows.Forms.ComboBox cboISO;
        private System.Windows.Forms.Label lblExpComp;
        private System.Windows.Forms.TrackBar trackExpComp;
        private System.Windows.Forms.Label lblExpCompValue;
        private System.Windows.Forms.Label lblMetering;
        private System.Windows.Forms.ComboBox cboMetering;
        private System.Windows.Forms.GroupBox grpWhiteBalance;
        private System.Windows.Forms.Label lblWBMode;
        private System.Windows.Forms.ComboBox cboWBMode;
        private System.Windows.Forms.GroupBox grpFocus;
        private System.Windows.Forms.Label lblFocusMode;
        private System.Windows.Forms.ComboBox cboFocusMode;
        private System.Windows.Forms.Label lblAFArea;
        private System.Windows.Forms.ComboBox cboAFArea;
        private System.Windows.Forms.Button btnAutoFocus;
        private System.Windows.Forms.GroupBox grpPictureControl;
        private System.Windows.Forms.Label lblPicCtrl;
        private System.Windows.Forms.ComboBox cboPictureControl;
        private System.Windows.Forms.Panel panelActions;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnCapture;
        private System.Windows.Forms.Button btnLiveView;
        private System.Windows.Forms.CheckBox chkAF;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Timer timerLiveView;
        private System.Windows.Forms.Timer timerStatus;
        private System.Windows.Forms.GroupBox grpPreset;
        private System.Windows.Forms.Label lblPreset;
        private System.Windows.Forms.ComboBox cboPreset;
        private System.Windows.Forms.Button btnPresets;
        private System.Windows.Forms.ToolStripMenuItem menuPresets;
    }
}
