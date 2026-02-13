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
            this.menuPresets = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.picLiveView = new System.Windows.Forms.PictureBox();
            this.lblLiveViewInfo = new System.Windows.Forms.Label();
            this.panelRight = new System.Windows.Forms.Panel();
            this.grpCameraInfo = new System.Windows.Forms.GroupBox();
            this.lblCameraName = new System.Windows.Forms.Label();
            this.lblBattery = new System.Windows.Forms.Label();
            this.progressBattery = new System.Windows.Forms.ProgressBar();
            this.lblLens = new System.Windows.Forms.Label();
            this.grpPreset = new System.Windows.Forms.GroupBox();
            this.lblPreset = new System.Windows.Forms.Label();
            this.cboPreset = new System.Windows.Forms.ComboBox();
            this.btnPresets = new System.Windows.Forms.Button();
            this.panelActions = new System.Windows.Forms.Panel();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnCapture = new System.Windows.Forms.Button();
            this.btnLiveView = new System.Windows.Forms.Button();
            this.chkAF = new System.Windows.Forms.CheckBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.grpSequencia = new System.Windows.Forms.GroupBox();
            this.lblSeqInput = new System.Windows.Forms.Label();
            this.txtSequencia = new System.Windows.Forms.TextBox();
            this.btnSeqOk = new System.Windows.Forms.Button();
            this.rtbSequencia = new System.Windows.Forms.RichTextBox();
            this.lblSeqInfo = new System.Windows.Forms.Label();
            this.btnSeqUndo = new System.Windows.Forms.Button();
            this.timerLiveView = new System.Windows.Forms.Timer(this.components);
            this.timerStatus = new System.Windows.Forms.Timer(this.components);
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.panelLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLiveView)).BeginInit();
            this.panelRight.SuspendLayout();
            this.grpCameraInfo.SuspendLayout();
            this.grpPreset.SuspendLayout();
            this.grpSequencia.SuspendLayout();
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
            this.menuConectar.Text = "&Conectar C\u00E2mera";
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
            this.menuCamera.Text = "&C\u00E2mera";
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
            // menuPresets
            //
            this.menuPresets.Name = "menuPresets";
            this.menuPresets.Size = new System.Drawing.Size(189, 22);
            this.menuPresets.Text = "&Presets de Material";
            this.menuPresets.Click += new System.EventHandler(this.btnPresets_Click);
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
            this.lblStatus.Size = new System.Drawing.Size(985, 17);
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
            // panelLeft
            //
            this.panelLeft.BackColor = System.Drawing.Color.Black;
            this.panelLeft.Controls.Add(this.picLiveView);
            this.panelLeft.Controls.Add(this.lblLiveViewInfo);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLeft.Location = new System.Drawing.Point(0, 24);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(1425, 704);
            this.panelLeft.TabIndex = 2;
            //
            // picLiveView
            //
            this.picLiveView.BackColor = System.Drawing.Color.Black;
            this.picLiveView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picLiveView.Location = new System.Drawing.Point(0, 0);
            this.picLiveView.Name = "picLiveView";
            this.picLiveView.Size = new System.Drawing.Size(1425, 684);
            this.picLiveView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLiveView.TabIndex = 0;
            this.picLiveView.TabStop = false;
            this.picLiveView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picLiveView_MouseDown);
            this.picLiveView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picLiveView_MouseMove);
            this.picLiveView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picLiveView_MouseUp);
            this.picLiveView.Paint += new System.Windows.Forms.PaintEventHandler(this.picLiveView_Paint);
            //
            // lblLiveViewInfo
            //
            this.lblLiveViewInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblLiveViewInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblLiveViewInfo.ForeColor = System.Drawing.Color.White;
            this.lblLiveViewInfo.Location = new System.Drawing.Point(0, 684);
            this.lblLiveViewInfo.Name = "lblLiveViewInfo";
            this.lblLiveViewInfo.Size = new System.Drawing.Size(1425, 20);
            this.lblLiveViewInfo.TabIndex = 1;
            this.lblLiveViewInfo.Text = "Live View desativado";
            this.lblLiveViewInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // panelRight
            //
            this.panelRight.AutoScroll = true;
            this.panelRight.Controls.Add(this.grpSequencia);
            this.panelRight.Controls.Add(this.grpPreset);
            this.panelRight.Controls.Add(this.grpCameraInfo);
            this.panelRight.Controls.Add(this.panelActions);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(1425, 24);
            this.panelRight.MinimumSize = new System.Drawing.Size(380, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Padding = new System.Windows.Forms.Padding(8);
            this.panelRight.Size = new System.Drawing.Size(495, 704);
            this.panelRight.TabIndex = 3;
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
            this.grpCameraInfo.Size = new System.Drawing.Size(479, 100);
            this.grpCameraInfo.TabIndex = 0;
            this.grpCameraInfo.TabStop = false;
            this.grpCameraInfo.Text = "Informa\u00E7\u00F5es da C\u00E2mera";
            //
            // lblCameraName
            //
            this.lblCameraName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCameraName.Location = new System.Drawing.Point(12, 22);
            this.lblCameraName.Name = "lblCameraName";
            this.lblCameraName.Size = new System.Drawing.Size(300, 18);
            this.lblCameraName.TabIndex = 0;
            this.lblCameraName.Text = "C\u00E2mera: N\u00E3o conectada";
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
            // grpPreset
            //
            this.grpPreset.Controls.Add(this.lblPreset);
            this.grpPreset.Controls.Add(this.cboPreset);
            this.grpPreset.Controls.Add(this.btnPresets);
            this.grpPreset.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpPreset.Location = new System.Drawing.Point(8, 108);
            this.grpPreset.Name = "grpPreset";
            this.grpPreset.Size = new System.Drawing.Size(479, 55);
            this.grpPreset.TabIndex = 1;
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
            // grpSequencia
            //
            this.grpSequencia.Controls.Add(this.btnSeqUndo);
            this.grpSequencia.Controls.Add(this.lblSeqInfo);
            this.grpSequencia.Controls.Add(this.rtbSequencia);
            this.grpSequencia.Controls.Add(this.btnSeqOk);
            this.grpSequencia.Controls.Add(this.txtSequencia);
            this.grpSequencia.Controls.Add(this.lblSeqInput);
            this.grpSequencia.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpSequencia.Location = new System.Drawing.Point(8, 163);
            this.grpSequencia.Name = "grpSequencia";
            this.grpSequencia.Padding = new System.Windows.Forms.Padding(8);
            this.grpSequencia.Size = new System.Drawing.Size(479, 200);
            this.grpSequencia.TabIndex = 3;
            this.grpSequencia.TabStop = false;
            this.grpSequencia.Text = "Sequ\u00EAncia de Chapas";
            //
            // lblSeqInput
            //
            this.lblSeqInput.Location = new System.Drawing.Point(12, 22);
            this.lblSeqInput.Name = "lblSeqInput";
            this.lblSeqInput.Size = new System.Drawing.Size(55, 20);
            this.lblSeqInput.TabIndex = 0;
            this.lblSeqInput.Text = "Chapas:";
            //
            // txtSequencia
            //
            this.txtSequencia.Location = new System.Drawing.Point(70, 20);
            this.txtSequencia.Name = "txtSequencia";
            this.txtSequencia.Size = new System.Drawing.Size(280, 23);
            this.txtSequencia.TabIndex = 1;
            //
            // btnSeqOk
            //
            this.btnSeqOk.Location = new System.Drawing.Point(358, 19);
            this.btnSeqOk.Name = "btnSeqOk";
            this.btnSeqOk.Size = new System.Drawing.Size(50, 25);
            this.btnSeqOk.TabIndex = 2;
            this.btnSeqOk.Text = "OK";
            this.btnSeqOk.UseVisualStyleBackColor = true;
            this.btnSeqOk.Click += new System.EventHandler(this.btnSeqOk_Click);
            //
            // rtbSequencia
            //
            this.rtbSequencia.BackColor = System.Drawing.Color.White;
            this.rtbSequencia.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.rtbSequencia.Location = new System.Drawing.Point(12, 50);
            this.rtbSequencia.Name = "rtbSequencia";
            this.rtbSequencia.ReadOnly = true;
            this.rtbSequencia.Size = new System.Drawing.Size(455, 100);
            this.rtbSequencia.TabIndex = 3;
            this.rtbSequencia.Text = "";
            //
            // lblSeqInfo
            //
            this.lblSeqInfo.Location = new System.Drawing.Point(12, 155);
            this.lblSeqInfo.Name = "lblSeqInfo";
            this.lblSeqInfo.Size = new System.Drawing.Size(300, 20);
            this.lblSeqInfo.TabIndex = 4;
            this.lblSeqInfo.Text = "";
            //
            // btnSeqUndo
            //
            this.btnSeqUndo.Enabled = false;
            this.btnSeqUndo.Location = new System.Drawing.Point(358, 152);
            this.btnSeqUndo.Name = "btnSeqUndo";
            this.btnSeqUndo.Size = new System.Drawing.Size(75, 25);
            this.btnSeqUndo.TabIndex = 5;
            this.btnSeqUndo.Text = "Desfazer";
            this.btnSeqUndo.UseVisualStyleBackColor = true;
            this.btnSeqUndo.Click += new System.EventHandler(this.btnSeqUndo_Click);
            //
            // panelActions
            //
            this.panelActions.Controls.Add(this.btnConnect);
            this.panelActions.Controls.Add(this.btnCapture);
            this.panelActions.Controls.Add(this.btnLiveView);
            this.panelActions.Controls.Add(this.chkAF);
            this.panelActions.Controls.Add(this.btnRefresh);
            this.panelActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelActions.Location = new System.Drawing.Point(8, 586);
            this.panelActions.Name = "panelActions";
            this.panelActions.Padding = new System.Windows.Forms.Padding(8);
            this.panelActions.Size = new System.Drawing.Size(479, 110);
            this.panelActions.TabIndex = 2;
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
            this.ClientSize = new System.Drawing.Size(1920, 750);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PicStone+";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.panelLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picLiveView)).EndInit();
            this.panelRight.ResumeLayout(false);
            this.grpCameraInfo.ResumeLayout(false);
            this.grpPreset.ResumeLayout(false);
            this.grpSequencia.ResumeLayout(false);
            this.grpSequencia.PerformLayout();
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
        private System.Windows.Forms.ToolStripMenuItem menuPresets;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.PictureBox picLiveView;
        private System.Windows.Forms.Label lblLiveViewInfo;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.GroupBox grpCameraInfo;
        private System.Windows.Forms.Label lblCameraName;
        private System.Windows.Forms.Label lblBattery;
        private System.Windows.Forms.ProgressBar progressBattery;
        private System.Windows.Forms.Label lblLens;
        private System.Windows.Forms.GroupBox grpPreset;
        private System.Windows.Forms.Label lblPreset;
        private System.Windows.Forms.ComboBox cboPreset;
        private System.Windows.Forms.Button btnPresets;
        private System.Windows.Forms.Panel panelActions;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnCapture;
        private System.Windows.Forms.Button btnLiveView;
        private System.Windows.Forms.CheckBox chkAF;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Timer timerLiveView;
        private System.Windows.Forms.Timer timerStatus;
        private System.Windows.Forms.GroupBox grpSequencia;
        private System.Windows.Forms.Label lblSeqInput;
        private System.Windows.Forms.TextBox txtSequencia;
        private System.Windows.Forms.Button btnSeqOk;
        private System.Windows.Forms.RichTextBox rtbSequencia;
        private System.Windows.Forms.Label lblSeqInfo;
        private System.Windows.Forms.Button btnSeqUndo;
    }
}
