namespace PicStonePlus.Forms
{
    partial class FrmPresets
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.splitPresets = new System.Windows.Forms.SplitContainer();
            this.lstPresets = new System.Windows.Forms.ListBox();
            this.panelListButtons = new System.Windows.Forms.Panel();
            this.btnNovo = new System.Windows.Forms.Button();
            this.btnExcluir = new System.Windows.Forms.Button();
            this.panelDetails = new System.Windows.Forms.Panel();
            this.grpPosProducao = new System.Windows.Forms.GroupBox();
            this.grpCamera = new System.Windows.Forms.GroupBox();
            this.panelSaveButtons = new System.Windows.Forms.Panel();
            this.btnSalvar = new System.Windows.Forms.Button();
            this.btnCapturarCamera = new System.Windows.Forms.Button();

            // Campos de câmera
            this.lblNome = new System.Windows.Forms.Label();
            this.txtNome = new System.Windows.Forms.TextBox();
            this.lblISO = new System.Windows.Forms.Label();
            this.cboISO = new System.Windows.Forms.ComboBox();
            this.lblAbertura = new System.Windows.Forms.Label();
            this.cboAbertura = new System.Windows.Forms.ComboBox();
            this.lblVelocidade = new System.Windows.Forms.Label();
            this.cboVelocidade = new System.Windows.Forms.ComboBox();
            this.lblPictureControl = new System.Windows.Forms.Label();
            this.cboPictureControl = new System.Windows.Forms.ComboBox();
            this.lblTemperatura = new System.Windows.Forms.Label();
            this.cboTemperatura = new System.Windows.Forms.ComboBox();
            this.lblTempInfo = new System.Windows.Forms.Label();
            this.chkAutoFoco = new System.Windows.Forms.CheckBox();

            // Campos de pós-produção
            this.lblBrilho = new System.Windows.Forms.Label();
            this.nudBrilho = new System.Windows.Forms.NumericUpDown();
            this.lblContraste = new System.Windows.Forms.Label();
            this.nudContraste = new System.Windows.Forms.NumericUpDown();
            this.lblSombras = new System.Windows.Forms.Label();
            this.nudSombras = new System.Windows.Forms.NumericUpDown();
            this.lblVermelho = new System.Windows.Forms.Label();
            this.nudVermelho = new System.Windows.Forms.NumericUpDown();
            this.lblVerde = new System.Windows.Forms.Label();
            this.nudVerde = new System.Windows.Forms.NumericUpDown();
            this.lblAzul = new System.Windows.Forms.Label();
            this.nudAzul = new System.Windows.Forms.NumericUpDown();
            this.lblSaturacao = new System.Windows.Forms.Label();
            this.nudSaturacao = new System.Windows.Forms.NumericUpDown();
            this.lblMatiz = new System.Windows.Forms.Label();
            this.nudMatiz = new System.Windows.Forms.NumericUpDown();
            this.lblGama = new System.Windows.Forms.Label();
            this.nudGama = new System.Windows.Forms.NumericUpDown();
            this.lblTonalidade = new System.Windows.Forms.Label();
            this.nudTonalidade = new System.Windows.Forms.NumericUpDown();

            ((System.ComponentModel.ISupportInitialize)(this.splitPresets)).BeginInit();
            this.splitPresets.Panel1.SuspendLayout();
            this.splitPresets.Panel2.SuspendLayout();
            this.splitPresets.SuspendLayout();
            this.panelListButtons.SuspendLayout();
            this.panelDetails.SuspendLayout();
            this.grpCamera.SuspendLayout();
            this.grpPosProducao.SuspendLayout();
            this.panelSaveButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBrilho)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudContraste)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSombras)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudVermelho)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudVerde)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAzul)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSaturacao)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMatiz)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGama)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTonalidade)).BeginInit();
            this.SuspendLayout();
            //
            // splitPresets
            //
            this.splitPresets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitPresets.Location = new System.Drawing.Point(0, 0);
            this.splitPresets.Name = "splitPresets";
            //
            // splitPresets.Panel1 (Lista)
            //
            this.splitPresets.Panel1.Controls.Add(this.lstPresets);
            this.splitPresets.Panel1.Controls.Add(this.panelListButtons);
            this.splitPresets.Panel1MinSize = 180;
            //
            // splitPresets.Panel2 (Detalhes)
            //
            this.splitPresets.Panel2.Controls.Add(this.panelDetails);
            this.splitPresets.Panel2.Controls.Add(this.panelSaveButtons);
            this.splitPresets.Panel2MinSize = 400;
            this.splitPresets.Size = new System.Drawing.Size(784, 561);
            this.splitPresets.SplitterDistance = 200;
            this.splitPresets.TabIndex = 0;
            //
            // lstPresets
            //
            this.lstPresets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstPresets.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lstPresets.FormattingEnabled = true;
            this.lstPresets.ItemHeight = 17;
            this.lstPresets.Location = new System.Drawing.Point(0, 0);
            this.lstPresets.Name = "lstPresets";
            this.lstPresets.Size = new System.Drawing.Size(200, 521);
            this.lstPresets.TabIndex = 0;
            this.lstPresets.SelectedIndexChanged += new System.EventHandler(this.lstPresets_SelectedIndexChanged);
            //
            // panelListButtons
            //
            this.panelListButtons.Controls.Add(this.btnNovo);
            this.panelListButtons.Controls.Add(this.btnExcluir);
            this.panelListButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelListButtons.Location = new System.Drawing.Point(0, 521);
            this.panelListButtons.Name = "panelListButtons";
            this.panelListButtons.Size = new System.Drawing.Size(200, 40);
            this.panelListButtons.TabIndex = 1;
            //
            // btnNovo
            //
            this.btnNovo.Location = new System.Drawing.Point(6, 6);
            this.btnNovo.Name = "btnNovo";
            this.btnNovo.Size = new System.Drawing.Size(90, 28);
            this.btnNovo.TabIndex = 0;
            this.btnNovo.Text = "Novo";
            this.btnNovo.UseVisualStyleBackColor = true;
            this.btnNovo.Click += new System.EventHandler(this.btnNovo_Click);
            //
            // btnExcluir
            //
            this.btnExcluir.Location = new System.Drawing.Point(102, 6);
            this.btnExcluir.Name = "btnExcluir";
            this.btnExcluir.Size = new System.Drawing.Size(90, 28);
            this.btnExcluir.TabIndex = 1;
            this.btnExcluir.Text = "Excluir";
            this.btnExcluir.UseVisualStyleBackColor = true;
            this.btnExcluir.Click += new System.EventHandler(this.btnExcluir_Click);
            //
            // panelDetails
            //
            this.panelDetails.AutoScroll = true;
            this.panelDetails.Controls.Add(this.grpPosProducao);
            this.panelDetails.Controls.Add(this.grpCamera);
            this.panelDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDetails.Location = new System.Drawing.Point(0, 0);
            this.panelDetails.Name = "panelDetails";
            this.panelDetails.Padding = new System.Windows.Forms.Padding(8);
            this.panelDetails.Size = new System.Drawing.Size(580, 511);
            this.panelDetails.TabIndex = 0;
            //
            // panelSaveButtons
            //
            this.panelSaveButtons.Controls.Add(this.btnSalvar);
            this.panelSaveButtons.Controls.Add(this.btnCapturarCamera);
            this.panelSaveButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelSaveButtons.Location = new System.Drawing.Point(0, 511);
            this.panelSaveButtons.Name = "panelSaveButtons";
            this.panelSaveButtons.Size = new System.Drawing.Size(580, 50);
            this.panelSaveButtons.TabIndex = 1;
            //
            // btnSalvar
            //
            this.btnSalvar.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnSalvar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalvar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSalvar.ForeColor = System.Drawing.Color.White;
            this.btnSalvar.Location = new System.Drawing.Point(8, 10);
            this.btnSalvar.Name = "btnSalvar";
            this.btnSalvar.Size = new System.Drawing.Size(120, 32);
            this.btnSalvar.TabIndex = 0;
            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.UseVisualStyleBackColor = false;
            this.btnSalvar.Click += new System.EventHandler(this.btnSalvar_Click);
            //
            // btnCapturarCamera
            //
            this.btnCapturarCamera.Location = new System.Drawing.Point(140, 10);
            this.btnCapturarCamera.Name = "btnCapturarCamera";
            this.btnCapturarCamera.Size = new System.Drawing.Size(160, 32);
            this.btnCapturarCamera.TabIndex = 1;
            this.btnCapturarCamera.Text = "Capturar da Câmera";
            this.btnCapturarCamera.UseVisualStyleBackColor = true;
            this.btnCapturarCamera.Click += new System.EventHandler(this.btnCapturarCamera_Click);

            // =====================================================
            // grpCamera - Configurações de Câmera (por material)
            // =====================================================
            this.grpCamera.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpCamera.Location = new System.Drawing.Point(8, 8);
            this.grpCamera.Name = "grpCamera";
            this.grpCamera.Padding = new System.Windows.Forms.Padding(8);
            this.grpCamera.Size = new System.Drawing.Size(564, 235);
            this.grpCamera.TabIndex = 0;
            this.grpCamera.TabStop = false;
            this.grpCamera.Text = "Configurações de Câmera (por material)";
            this.grpCamera.Controls.Add(this.lblNome);
            this.grpCamera.Controls.Add(this.txtNome);
            this.grpCamera.Controls.Add(this.lblISO);
            this.grpCamera.Controls.Add(this.cboISO);
            this.grpCamera.Controls.Add(this.lblAbertura);
            this.grpCamera.Controls.Add(this.cboAbertura);
            this.grpCamera.Controls.Add(this.lblVelocidade);
            this.grpCamera.Controls.Add(this.cboVelocidade);
            this.grpCamera.Controls.Add(this.lblPictureControl);
            this.grpCamera.Controls.Add(this.cboPictureControl);
            this.grpCamera.Controls.Add(this.lblTemperatura);
            this.grpCamera.Controls.Add(this.cboTemperatura);
            this.grpCamera.Controls.Add(this.lblTempInfo);
            this.grpCamera.Controls.Add(this.chkAutoFoco);

            // Nome
            this.lblNome.Location = new System.Drawing.Point(12, 24);
            this.lblNome.Name = "lblNome";
            this.lblNome.Size = new System.Drawing.Size(90, 20);
            this.lblNome.Text = "Nome:";
            //
            this.txtNome.Location = new System.Drawing.Point(105, 22);
            this.txtNome.Name = "txtNome";
            this.txtNome.Size = new System.Drawing.Size(230, 23);
            this.txtNome.TabIndex = 1;

            // ISO
            this.lblISO.Location = new System.Drawing.Point(12, 54);
            this.lblISO.Name = "lblISO";
            this.lblISO.Size = new System.Drawing.Size(90, 20);
            this.lblISO.Text = "ISO:";
            //
            this.cboISO.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboISO.Location = new System.Drawing.Point(105, 52);
            this.cboISO.Name = "cboISO";
            this.cboISO.Size = new System.Drawing.Size(160, 23);
            this.cboISO.TabIndex = 3;

            // Abertura
            this.lblAbertura.Location = new System.Drawing.Point(12, 84);
            this.lblAbertura.Name = "lblAbertura";
            this.lblAbertura.Size = new System.Drawing.Size(90, 20);
            this.lblAbertura.Text = "Abertura:";
            //
            this.cboAbertura.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAbertura.Location = new System.Drawing.Point(105, 82);
            this.cboAbertura.Name = "cboAbertura";
            this.cboAbertura.Size = new System.Drawing.Size(160, 23);
            this.cboAbertura.TabIndex = 5;

            // Velocidade
            this.lblVelocidade.Location = new System.Drawing.Point(12, 114);
            this.lblVelocidade.Name = "lblVelocidade";
            this.lblVelocidade.Size = new System.Drawing.Size(90, 20);
            this.lblVelocidade.Text = "Velocidade:";
            //
            this.cboVelocidade.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVelocidade.Location = new System.Drawing.Point(105, 112);
            this.cboVelocidade.Name = "cboVelocidade";
            this.cboVelocidade.Size = new System.Drawing.Size(160, 23);
            this.cboVelocidade.TabIndex = 7;

            // PictureControl
            this.lblPictureControl.Location = new System.Drawing.Point(12, 144);
            this.lblPictureControl.Name = "lblPictureControl";
            this.lblPictureControl.Size = new System.Drawing.Size(90, 20);
            this.lblPictureControl.Text = "Pic. Control:";
            //
            this.cboPictureControl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPictureControl.Location = new System.Drawing.Point(105, 142);
            this.cboPictureControl.Name = "cboPictureControl";
            this.cboPictureControl.Size = new System.Drawing.Size(160, 23);
            this.cboPictureControl.TabIndex = 9;

            // Temperatura (Kelvin)
            this.lblTemperatura.Location = new System.Drawing.Point(12, 174);
            this.lblTemperatura.Name = "lblTemperatura";
            this.lblTemperatura.Size = new System.Drawing.Size(90, 20);
            this.lblTemperatura.Text = "Temp. Cor:";
            //
            this.cboTemperatura.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTemperatura.Location = new System.Drawing.Point(105, 172);
            this.cboTemperatura.Name = "cboTemperatura";
            this.cboTemperatura.Size = new System.Drawing.Size(160, 23);
            this.cboTemperatura.TabIndex = 11;
            //
            this.lblTempInfo.Location = new System.Drawing.Point(272, 174);
            this.lblTempInfo.Name = "lblTempInfo";
            this.lblTempInfo.Size = new System.Drawing.Size(200, 20);
            this.lblTempInfo.Text = "D7200 / D7500";
            this.lblTempInfo.ForeColor = System.Drawing.Color.Gray;

            // Auto Foco
            this.chkAutoFoco.Location = new System.Drawing.Point(105, 202);
            this.chkAutoFoco.Name = "chkAutoFoco";
            this.chkAutoFoco.Size = new System.Drawing.Size(200, 20);
            this.chkAutoFoco.Text = "Auto Foco";
            this.chkAutoFoco.Checked = true;
            this.chkAutoFoco.TabIndex = 12;

            // =====================================================
            // grpPosProducao - Pós-Produção
            // =====================================================
            this.grpPosProducao.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpPosProducao.Location = new System.Drawing.Point(8, 218);
            this.grpPosProducao.Name = "grpPosProducao";
            this.grpPosProducao.Padding = new System.Windows.Forms.Padding(8);
            this.grpPosProducao.Size = new System.Drawing.Size(564, 220);
            this.grpPosProducao.TabIndex = 1;
            this.grpPosProducao.TabStop = false;
            this.grpPosProducao.Text = "Pós-Produção (processamento em software)";

            // Layout: 2 colunas x 5 linhas
            int col1X = 12, col1VX = 105;
            int col2X = 290, col2VX = 380;
            int startY = 24;
            int rowH = 30;

            // Coluna 1
            SetupNudPair(this.grpPosProducao, this.lblBrilho, this.nudBrilho, "Brilho:", col1X, col1VX, startY);
            SetupNudPair(this.grpPosProducao, this.lblContraste, this.nudContraste, "Contraste:", col1X, col1VX, startY + rowH);
            SetupNudPair(this.grpPosProducao, this.lblSombras, this.nudSombras, "Sombras:", col1X, col1VX, startY + rowH * 2);
            SetupNudPair(this.grpPosProducao, this.lblVermelho, this.nudVermelho, "Vermelho:", col1X, col1VX, startY + rowH * 3);
            SetupNudPair(this.grpPosProducao, this.lblVerde, this.nudVerde, "Verde:", col1X, col1VX, startY + rowH * 4);

            // Coluna 2
            SetupNudPair(this.grpPosProducao, this.lblAzul, this.nudAzul, "Azul:", col2X, col2VX, startY);
            SetupNudPair(this.grpPosProducao, this.lblSaturacao, this.nudSaturacao, "Saturação:", col2X, col2VX, startY + rowH);
            SetupNudPair(this.grpPosProducao, this.lblMatiz, this.nudMatiz, "Matiz:", col2X, col2VX, startY + rowH * 2);
            SetupNudPair(this.grpPosProducao, this.lblGama, this.nudGama, "Gama:", col2X, col2VX, startY + rowH * 3);
            SetupNudPair(this.grpPosProducao, this.lblTonalidade, this.nudTonalidade, "Tonalidade:", col2X, col2VX, startY + rowH * 4);

            //
            // FrmPresets
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.splitPresets);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "FrmPresets";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Presets de Material";

            this.splitPresets.Panel1.ResumeLayout(false);
            this.splitPresets.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitPresets)).EndInit();
            this.splitPresets.ResumeLayout(false);
            this.panelListButtons.ResumeLayout(false);
            this.panelDetails.ResumeLayout(false);
            this.grpCamera.ResumeLayout(false);
            this.grpCamera.PerformLayout();
            this.grpPosProducao.ResumeLayout(false);
            this.panelSaveButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudBrilho)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudContraste)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSombras)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudVermelho)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudVerde)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAzul)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSaturacao)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMatiz)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGama)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTonalidade)).EndInit();
            this.ResumeLayout(false);
        }

        private void SetupNudPair(System.Windows.Forms.GroupBox parent,
            System.Windows.Forms.Label lbl, System.Windows.Forms.NumericUpDown nud,
            string text, int lblX, int nudX, int y)
        {
            lbl.Location = new System.Drawing.Point(lblX, y + 2);
            lbl.Size = new System.Drawing.Size(90, 20);
            lbl.Text = text;
            parent.Controls.Add(lbl);

            nud.Location = new System.Drawing.Point(nudX, y);
            nud.Size = new System.Drawing.Size(80, 23);
            nud.Minimum = -1000;
            nud.Maximum = 1000;
            nud.Value = 0;
            parent.Controls.Add(nud);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitPresets;
        private System.Windows.Forms.ListBox lstPresets;
        private System.Windows.Forms.Panel panelListButtons;
        private System.Windows.Forms.Button btnNovo;
        private System.Windows.Forms.Button btnExcluir;
        private System.Windows.Forms.Panel panelDetails;
        private System.Windows.Forms.GroupBox grpCamera;
        private System.Windows.Forms.GroupBox grpPosProducao;
        private System.Windows.Forms.Panel panelSaveButtons;
        private System.Windows.Forms.Button btnSalvar;
        private System.Windows.Forms.Button btnCapturarCamera;

        // Campos de câmera (por material)
        private System.Windows.Forms.Label lblNome;
        private System.Windows.Forms.TextBox txtNome;
        private System.Windows.Forms.Label lblISO;
        private System.Windows.Forms.ComboBox cboISO;
        private System.Windows.Forms.Label lblAbertura;
        private System.Windows.Forms.ComboBox cboAbertura;
        private System.Windows.Forms.Label lblVelocidade;
        private System.Windows.Forms.ComboBox cboVelocidade;
        private System.Windows.Forms.Label lblPictureControl;
        private System.Windows.Forms.ComboBox cboPictureControl;
        private System.Windows.Forms.Label lblTemperatura;
        private System.Windows.Forms.ComboBox cboTemperatura;
        private System.Windows.Forms.Label lblTempInfo;
        private System.Windows.Forms.CheckBox chkAutoFoco;

        // Campos de pós-produção
        private System.Windows.Forms.Label lblBrilho;
        private System.Windows.Forms.NumericUpDown nudBrilho;
        private System.Windows.Forms.Label lblContraste;
        private System.Windows.Forms.NumericUpDown nudContraste;
        private System.Windows.Forms.Label lblSombras;
        private System.Windows.Forms.NumericUpDown nudSombras;
        private System.Windows.Forms.Label lblVermelho;
        private System.Windows.Forms.NumericUpDown nudVermelho;
        private System.Windows.Forms.Label lblVerde;
        private System.Windows.Forms.NumericUpDown nudVerde;
        private System.Windows.Forms.Label lblAzul;
        private System.Windows.Forms.NumericUpDown nudAzul;
        private System.Windows.Forms.Label lblSaturacao;
        private System.Windows.Forms.NumericUpDown nudSaturacao;
        private System.Windows.Forms.Label lblMatiz;
        private System.Windows.Forms.NumericUpDown nudMatiz;
        private System.Windows.Forms.Label lblGama;
        private System.Windows.Forms.NumericUpDown nudGama;
        private System.Windows.Forms.Label lblTonalidade;
        private System.Windows.Forms.NumericUpDown nudTonalidade;
    }
}
