namespace PicStonePlus.Forms
{
    partial class FrmConfig
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabGeral = new System.Windows.Forms.TabPage();
            this.tabNomeArquivo = new System.Windows.Forms.TabPage();
            this.tabSubpastas = new System.Windows.Forms.TabPage();
            this.tabCaminhosExtras = new System.Windows.Forms.TabPage();

            // Tab Geral
            this.lblPastaBase = new System.Windows.Forms.Label();
            this.txtPastaBase = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblDigitoChapa = new System.Windows.Forms.Label();
            this.numDigitoChapa = new System.Windows.Forms.NumericUpDown();
            this.lblDigitoBloco = new System.Windows.Forms.Label();
            this.numDigitoBloco = new System.Windows.Forms.NumericUpDown();
            this.lblEstagios = new System.Windows.Forms.Label();
            this.lstEstagios = new System.Windows.Forms.ListBox();
            this.txtNovoEstagio = new System.Windows.Forms.TextBox();
            this.btnEstagioAdd = new System.Windows.Forms.Button();
            this.btnEstagioRemove = new System.Windows.Forms.Button();

            // Tab Nome Arquivo
            this.lblNomeTemplate = new System.Windows.Forms.Label();
            this.txtNomeTemplate = new System.Windows.Forms.TextBox();
            this.lblNomeTokens = new System.Windows.Forms.Label();
            this.lblNomePreview = new System.Windows.Forms.Label();
            this.lblNomePreviewValue = new System.Windows.Forms.Label();

            // Tab Subpastas
            this.lblPastaTemplate = new System.Windows.Forms.Label();
            this.txtPastaTemplate = new System.Windows.Forms.TextBox();
            this.lblPastaTokens = new System.Windows.Forms.Label();
            this.lblPastaPreview = new System.Windows.Forms.Label();
            this.lblPastaPreviewValue = new System.Windows.Forms.Label();

            // Tab Caminhos Extras
            this.grpExtra1 = new System.Windows.Forms.GroupBox();
            this.chkExtra1Ativo = new System.Windows.Forms.CheckBox();
            this.btnExtra1Copiar = new System.Windows.Forms.Button();
            this.lblExtra1Pasta = new System.Windows.Forms.Label();
            this.txtExtra1Pasta = new System.Windows.Forms.TextBox();
            this.btnExtra1Browse = new System.Windows.Forms.Button();
            this.lblExtra1Reducao = new System.Windows.Forms.Label();
            this.numExtra1Reducao = new System.Windows.Forms.NumericUpDown();
            this.lblExtra1ReducaoSuffix = new System.Windows.Forms.Label();
            this.lblExtra1Nome = new System.Windows.Forms.Label();
            this.txtExtra1Nome = new System.Windows.Forms.TextBox();
            this.lblExtra1Subpasta = new System.Windows.Forms.Label();
            this.txtExtra1Subpasta = new System.Windows.Forms.TextBox();
            this.lblExtra1Preview = new System.Windows.Forms.Label();

            this.grpExtra2 = new System.Windows.Forms.GroupBox();
            this.chkExtra2Ativo = new System.Windows.Forms.CheckBox();
            this.btnExtra2Copiar = new System.Windows.Forms.Button();
            this.lblExtra2Pasta = new System.Windows.Forms.Label();
            this.txtExtra2Pasta = new System.Windows.Forms.TextBox();
            this.btnExtra2Browse = new System.Windows.Forms.Button();
            this.lblExtra2Reducao = new System.Windows.Forms.Label();
            this.numExtra2Reducao = new System.Windows.Forms.NumericUpDown();
            this.lblExtra2ReducaoSuffix = new System.Windows.Forms.Label();
            this.lblExtra2Nome = new System.Windows.Forms.Label();
            this.txtExtra2Nome = new System.Windows.Forms.TextBox();
            this.lblExtra2Subpasta = new System.Windows.Forms.Label();
            this.txtExtra2Subpasta = new System.Windows.Forms.TextBox();
            this.lblExtra2Preview = new System.Windows.Forms.Label();

            // Bot\u00F5es rodap\u00E9
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();

            this.tabControl.SuspendLayout();
            this.tabGeral.SuspendLayout();
            this.tabNomeArquivo.SuspendLayout();
            this.tabSubpastas.SuspendLayout();
            this.tabCaminhosExtras.SuspendLayout();
            this.grpExtra1.SuspendLayout();
            this.grpExtra2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDigitoChapa)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDigitoBloco)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numExtra1Reducao)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numExtra2Reducao)).BeginInit();
            this.SuspendLayout();

            // ============================
            // tabControl
            // ============================
            this.tabControl.Controls.Add(this.tabGeral);
            this.tabControl.Controls.Add(this.tabNomeArquivo);
            this.tabControl.Controls.Add(this.tabSubpastas);
            this.tabControl.Controls.Add(this.tabCaminhosExtras);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(510, 460);
            this.tabControl.TabIndex = 0;

            // ============================
            // tabGeral
            // ============================
            this.tabGeral.Controls.Add(this.lblPastaBase);
            this.tabGeral.Controls.Add(this.txtPastaBase);
            this.tabGeral.Controls.Add(this.btnBrowse);
            this.tabGeral.Controls.Add(this.lblDigitoChapa);
            this.tabGeral.Controls.Add(this.numDigitoChapa);
            this.tabGeral.Controls.Add(this.lblDigitoBloco);
            this.tabGeral.Controls.Add(this.numDigitoBloco);
            this.tabGeral.Controls.Add(this.lblEstagios);
            this.tabGeral.Controls.Add(this.lstEstagios);
            this.tabGeral.Controls.Add(this.txtNovoEstagio);
            this.tabGeral.Controls.Add(this.btnEstagioAdd);
            this.tabGeral.Controls.Add(this.btnEstagioRemove);
            this.tabGeral.Location = new System.Drawing.Point(4, 24);
            this.tabGeral.Name = "tabGeral";
            this.tabGeral.Padding = new System.Windows.Forms.Padding(12);
            this.tabGeral.Size = new System.Drawing.Size(502, 362);
            this.tabGeral.TabIndex = 0;
            this.tabGeral.Text = "Geral";
            this.tabGeral.UseVisualStyleBackColor = true;

            // lblPastaBase
            this.lblPastaBase.Location = new System.Drawing.Point(15, 20);
            this.lblPastaBase.Name = "lblPastaBase";
            this.lblPastaBase.Size = new System.Drawing.Size(120, 20);
            this.lblPastaBase.Text = "Pasta Base:";

            // txtPastaBase
            this.txtPastaBase.Location = new System.Drawing.Point(15, 42);
            this.txtPastaBase.Name = "txtPastaBase";
            this.txtPastaBase.Size = new System.Drawing.Size(420, 23);
            this.txtPastaBase.TabIndex = 1;

            // btnBrowse
            this.btnBrowse.Location = new System.Drawing.Point(440, 41);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(40, 25);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);

            // lblDigitoChapa
            this.lblDigitoChapa.Location = new System.Drawing.Point(15, 85);
            this.lblDigitoChapa.Name = "lblDigitoChapa";
            this.lblDigitoChapa.Size = new System.Drawing.Size(170, 20);
            this.lblDigitoChapa.Text = "D\u00EDgitos da Chapa (ex: 3 = 001):";

            // numDigitoChapa
            this.numDigitoChapa.Location = new System.Drawing.Point(190, 83);
            this.numDigitoChapa.Minimum = 1;
            this.numDigitoChapa.Maximum = 6;
            this.numDigitoChapa.Name = "numDigitoChapa";
            this.numDigitoChapa.Size = new System.Drawing.Size(60, 23);
            this.numDigitoChapa.TabIndex = 3;
            this.numDigitoChapa.Value = 3;

            // lblDigitoBloco
            this.lblDigitoBloco.Location = new System.Drawing.Point(270, 85);
            this.lblDigitoBloco.Name = "lblDigitoBloco";
            this.lblDigitoBloco.Size = new System.Drawing.Size(155, 20);
            this.lblDigitoBloco.Text = "D\u00EDgitos do Bloco (0 = sem):";

            // numDigitoBloco
            this.numDigitoBloco.Location = new System.Drawing.Point(430, 83);
            this.numDigitoBloco.Minimum = 0;
            this.numDigitoBloco.Maximum = 8;
            this.numDigitoBloco.Name = "numDigitoBloco";
            this.numDigitoBloco.Size = new System.Drawing.Size(50, 23);
            this.numDigitoBloco.TabIndex = 4;
            this.numDigitoBloco.Value = 0;

            // lblEstagios
            this.lblEstagios.Location = new System.Drawing.Point(15, 125);
            this.lblEstagios.Name = "lblEstagios";
            this.lblEstagios.Size = new System.Drawing.Size(200, 20);
            this.lblEstagios.Text = "Est\u00E1gios cadastrados:";
            this.lblEstagios.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // lstEstagios
            this.lstEstagios.Location = new System.Drawing.Point(15, 147);
            this.lstEstagios.Name = "lstEstagios";
            this.lstEstagios.Size = new System.Drawing.Size(200, 186);
            this.lstEstagios.TabIndex = 4;

            // txtNovoEstagio
            this.txtNovoEstagio.Location = new System.Drawing.Point(225, 147);
            this.txtNovoEstagio.Name = "txtNovoEstagio";
            this.txtNovoEstagio.Size = new System.Drawing.Size(150, 23);
            this.txtNovoEstagio.TabIndex = 5;

            // btnEstagioAdd
            this.btnEstagioAdd.Location = new System.Drawing.Point(380, 146);
            this.btnEstagioAdd.Name = "btnEstagioAdd";
            this.btnEstagioAdd.Size = new System.Drawing.Size(100, 25);
            this.btnEstagioAdd.TabIndex = 6;
            this.btnEstagioAdd.Text = "Adicionar";
            this.btnEstagioAdd.UseVisualStyleBackColor = true;
            this.btnEstagioAdd.Click += new System.EventHandler(this.btnEstagioAdd_Click);

            // btnEstagioRemove
            this.btnEstagioRemove.Location = new System.Drawing.Point(225, 178);
            this.btnEstagioRemove.Name = "btnEstagioRemove";
            this.btnEstagioRemove.Size = new System.Drawing.Size(100, 25);
            this.btnEstagioRemove.TabIndex = 7;
            this.btnEstagioRemove.Text = "Remover";
            this.btnEstagioRemove.UseVisualStyleBackColor = true;
            this.btnEstagioRemove.Click += new System.EventHandler(this.btnEstagioRemove_Click);

            // ============================
            // tabNomeArquivo
            // ============================
            this.tabNomeArquivo.Controls.Add(this.lblNomeTemplate);
            this.tabNomeArquivo.Controls.Add(this.txtNomeTemplate);
            this.tabNomeArquivo.Controls.Add(this.lblNomeTokens);
            this.tabNomeArquivo.Controls.Add(this.lblNomePreview);
            this.tabNomeArquivo.Controls.Add(this.lblNomePreviewValue);
            this.tabNomeArquivo.Location = new System.Drawing.Point(4, 24);
            this.tabNomeArquivo.Name = "tabNomeArquivo";
            this.tabNomeArquivo.Padding = new System.Windows.Forms.Padding(12);
            this.tabNomeArquivo.Size = new System.Drawing.Size(502, 362);
            this.tabNomeArquivo.TabIndex = 1;
            this.tabNomeArquivo.Text = "Nome do Arquivo";
            this.tabNomeArquivo.UseVisualStyleBackColor = true;

            // lblNomeTemplate
            this.lblNomeTemplate.Location = new System.Drawing.Point(15, 15);
            this.lblNomeTemplate.Name = "lblNomeTemplate";
            this.lblNomeTemplate.Size = new System.Drawing.Size(460, 20);
            this.lblNomeTemplate.Text = "Template do nome do arquivo (sem extens\u00E3o):";
            this.lblNomeTemplate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // txtNomeTemplate
            this.txtNomeTemplate.Location = new System.Drawing.Point(15, 38);
            this.txtNomeTemplate.Name = "txtNomeTemplate";
            this.txtNomeTemplate.Size = new System.Drawing.Size(460, 23);
            this.txtNomeTemplate.TabIndex = 0;
            this.txtNomeTemplate.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtNomeTemplate.TextChanged += new System.EventHandler(this.txtNomeTemplate_TextChanged);

            // lblNomeTokens
            this.lblNomeTokens.Location = new System.Drawing.Point(15, 75);
            this.lblNomeTokens.Name = "lblNomeTokens";
            this.lblNomeTokens.Size = new System.Drawing.Size(460, 120);
            this.lblNomeTokens.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
            this.lblNomeTokens.Text = "Tokens dispon\u00EDveis (use dentro do template):\n\n" +
                "{Material}  \u2192  Nome do preset ativo\n" +
                "{Espessura}  \u2192  Valor do campo Espessura\n" +
                "{Bloco}  \u2192  N\u00FAmero do bloco\n" +
                "{Bundle}  \u2192  Identificador do bundle\n" +
                "{Estagio}  \u2192  Est\u00E1gio selecionado\n" +
                "{Chapa}  \u2192  N\u00FAmero da chapa (com d\u00EDgitos)\n" +
                "{Data}  \u2192  Data (yyyyMMdd)    {Hora}  \u2192  Hora (HHmmss)";

            // lblNomePreview
            this.lblNomePreview.Location = new System.Drawing.Point(15, 210);
            this.lblNomePreview.Name = "lblNomePreview";
            this.lblNomePreview.Size = new System.Drawing.Size(460, 20);
            this.lblNomePreview.Text = "Preview:";
            this.lblNomePreview.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // lblNomePreviewValue
            this.lblNomePreviewValue.Location = new System.Drawing.Point(15, 233);
            this.lblNomePreviewValue.Name = "lblNomePreviewValue";
            this.lblNomePreviewValue.Size = new System.Drawing.Size(460, 40);
            this.lblNomePreviewValue.Font = new System.Drawing.Font("Consolas", 10F);
            this.lblNomePreviewValue.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblNomePreviewValue.Text = "";

            // ============================
            // tabSubpastas
            // ============================
            this.tabSubpastas.Controls.Add(this.lblPastaTemplate);
            this.tabSubpastas.Controls.Add(this.txtPastaTemplate);
            this.tabSubpastas.Controls.Add(this.lblPastaTokens);
            this.tabSubpastas.Controls.Add(this.lblPastaPreview);
            this.tabSubpastas.Controls.Add(this.lblPastaPreviewValue);
            this.tabSubpastas.Location = new System.Drawing.Point(4, 24);
            this.tabSubpastas.Name = "tabSubpastas";
            this.tabSubpastas.Padding = new System.Windows.Forms.Padding(12);
            this.tabSubpastas.Size = new System.Drawing.Size(502, 362);
            this.tabSubpastas.TabIndex = 2;
            this.tabSubpastas.Text = "Estrutura de Pastas";
            this.tabSubpastas.UseVisualStyleBackColor = true;

            // lblPastaTemplate
            this.lblPastaTemplate.Location = new System.Drawing.Point(15, 15);
            this.lblPastaTemplate.Name = "lblPastaTemplate";
            this.lblPastaTemplate.Size = new System.Drawing.Size(460, 20);
            this.lblPastaTemplate.Text = "Template de subpastas (use \\\\ para separar n\u00EDveis):";
            this.lblPastaTemplate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // txtPastaTemplate
            this.txtPastaTemplate.Location = new System.Drawing.Point(15, 38);
            this.txtPastaTemplate.Name = "txtPastaTemplate";
            this.txtPastaTemplate.Size = new System.Drawing.Size(460, 23);
            this.txtPastaTemplate.TabIndex = 0;
            this.txtPastaTemplate.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtPastaTemplate.TextChanged += new System.EventHandler(this.txtPastaTemplate_TextChanged);

            // lblPastaTokens
            this.lblPastaTokens.Location = new System.Drawing.Point(15, 75);
            this.lblPastaTokens.Name = "lblPastaTokens";
            this.lblPastaTokens.Size = new System.Drawing.Size(460, 110);
            this.lblPastaTokens.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
            this.lblPastaTokens.Text = "Tokens dispon\u00EDveis:\n\n" +
                "{Ano}  \u2192  Ano (2026)\n" +
                "{Mes}  \u2192  M\u00EAs (02 - FEVEREIRO)\n" +
                "{Material}  \u2192  Nome do preset\n" +
                "{Bloco}  \u2192  N\u00FAmero do bloco\n" +
                "{Espessura}  \u2192  Valor da espessura\n" +
                "{Estagio}  \u2192  Est\u00E1gio selecionado";

            // lblPastaPreview
            this.lblPastaPreview.Location = new System.Drawing.Point(15, 200);
            this.lblPastaPreview.Name = "lblPastaPreview";
            this.lblPastaPreview.Size = new System.Drawing.Size(460, 20);
            this.lblPastaPreview.Text = "Preview:";
            this.lblPastaPreview.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // lblPastaPreviewValue
            this.lblPastaPreviewValue.Location = new System.Drawing.Point(15, 223);
            this.lblPastaPreviewValue.Name = "lblPastaPreviewValue";
            this.lblPastaPreviewValue.Size = new System.Drawing.Size(460, 40);
            this.lblPastaPreviewValue.Font = new System.Drawing.Font("Consolas", 10F);
            this.lblPastaPreviewValue.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblPastaPreviewValue.Text = "";

            // ============================
            // tabCaminhosExtras
            // ============================
            this.tabCaminhosExtras.Controls.Add(this.grpExtra1);
            this.tabCaminhosExtras.Controls.Add(this.grpExtra2);
            this.tabCaminhosExtras.Location = new System.Drawing.Point(4, 24);
            this.tabCaminhosExtras.Name = "tabCaminhosExtras";
            this.tabCaminhosExtras.Padding = new System.Windows.Forms.Padding(8);
            this.tabCaminhosExtras.Size = new System.Drawing.Size(502, 432);
            this.tabCaminhosExtras.TabIndex = 3;
            this.tabCaminhosExtras.Text = "Caminhos Extras";
            this.tabCaminhosExtras.UseVisualStyleBackColor = true;

            // ============================
            // grpExtra1 - Caminho Extra 1
            // ============================
            this.grpExtra1.Controls.Add(this.chkExtra1Ativo);
            this.grpExtra1.Controls.Add(this.btnExtra1Copiar);
            this.grpExtra1.Controls.Add(this.lblExtra1Pasta);
            this.grpExtra1.Controls.Add(this.txtExtra1Pasta);
            this.grpExtra1.Controls.Add(this.btnExtra1Browse);
            this.grpExtra1.Controls.Add(this.lblExtra1Reducao);
            this.grpExtra1.Controls.Add(this.numExtra1Reducao);
            this.grpExtra1.Controls.Add(this.lblExtra1ReducaoSuffix);
            this.grpExtra1.Controls.Add(this.lblExtra1Nome);
            this.grpExtra1.Controls.Add(this.txtExtra1Nome);
            this.grpExtra1.Controls.Add(this.lblExtra1Subpasta);
            this.grpExtra1.Controls.Add(this.txtExtra1Subpasta);
            this.grpExtra1.Controls.Add(this.lblExtra1Preview);
            this.grpExtra1.Location = new System.Drawing.Point(8, 8);
            this.grpExtra1.Name = "grpExtra1";
            this.grpExtra1.Size = new System.Drawing.Size(484, 205);
            this.grpExtra1.TabIndex = 0;
            this.grpExtra1.TabStop = false;
            this.grpExtra1.Text = "Caminho Extra 1";

            // chkExtra1Ativo
            this.chkExtra1Ativo.Location = new System.Drawing.Point(15, 22);
            this.chkExtra1Ativo.Name = "chkExtra1Ativo";
            this.chkExtra1Ativo.Size = new System.Drawing.Size(60, 20);
            this.chkExtra1Ativo.Text = "Ativo";
            this.chkExtra1Ativo.TabIndex = 0;
            this.chkExtra1Ativo.CheckedChanged += new System.EventHandler(this.chkExtra1Ativo_CheckedChanged);

            // btnExtra1Copiar
            this.btnExtra1Copiar.Location = new System.Drawing.Point(330, 18);
            this.btnExtra1Copiar.Name = "btnExtra1Copiar";
            this.btnExtra1Copiar.Size = new System.Drawing.Size(140, 25);
            this.btnExtra1Copiar.TabIndex = 6;
            this.btnExtra1Copiar.Text = "Copiar do Principal";
            this.btnExtra1Copiar.UseVisualStyleBackColor = true;
            this.btnExtra1Copiar.Click += new System.EventHandler(this.btnExtra1Copiar_Click);

            // lblExtra1Pasta
            this.lblExtra1Pasta.Location = new System.Drawing.Point(15, 48);
            this.lblExtra1Pasta.Name = "lblExtra1Pasta";
            this.lblExtra1Pasta.Size = new System.Drawing.Size(80, 20);
            this.lblExtra1Pasta.Text = "Pasta:";

            // txtExtra1Pasta
            this.txtExtra1Pasta.Location = new System.Drawing.Point(15, 68);
            this.txtExtra1Pasta.Name = "txtExtra1Pasta";
            this.txtExtra1Pasta.Size = new System.Drawing.Size(410, 23);
            this.txtExtra1Pasta.TabIndex = 1;
            this.txtExtra1Pasta.TextChanged += new System.EventHandler(this.ExtraPreview_Changed);

            // btnExtra1Browse
            this.btnExtra1Browse.Location = new System.Drawing.Point(430, 67);
            this.btnExtra1Browse.Name = "btnExtra1Browse";
            this.btnExtra1Browse.Size = new System.Drawing.Size(40, 25);
            this.btnExtra1Browse.TabIndex = 2;
            this.btnExtra1Browse.Text = "...";
            this.btnExtra1Browse.UseVisualStyleBackColor = true;
            this.btnExtra1Browse.Click += new System.EventHandler(this.btnExtra1Browse_Click);

            // lblExtra1Reducao
            this.lblExtra1Reducao.Location = new System.Drawing.Point(15, 98);
            this.lblExtra1Reducao.Name = "lblExtra1Reducao";
            this.lblExtra1Reducao.Size = new System.Drawing.Size(70, 20);
            this.lblExtra1Reducao.Text = "Redu\u00E7\u00E3o:";

            // numExtra1Reducao
            this.numExtra1Reducao.Location = new System.Drawing.Point(85, 96);
            this.numExtra1Reducao.Minimum = 10;
            this.numExtra1Reducao.Maximum = 100;
            this.numExtra1Reducao.Increment = 5;
            this.numExtra1Reducao.Name = "numExtra1Reducao";
            this.numExtra1Reducao.Size = new System.Drawing.Size(55, 23);
            this.numExtra1Reducao.TabIndex = 3;
            this.numExtra1Reducao.Value = 50;

            // lblExtra1ReducaoSuffix
            this.lblExtra1ReducaoSuffix.Location = new System.Drawing.Point(143, 98);
            this.lblExtra1ReducaoSuffix.Name = "lblExtra1ReducaoSuffix";
            this.lblExtra1ReducaoSuffix.Size = new System.Drawing.Size(300, 20);
            this.lblExtra1ReducaoSuffix.Text = "% da dimens\u00E3o original (ex: 50% de 6000x4000 = 3000x2000)";
            this.lblExtra1ReducaoSuffix.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);

            // lblExtra1Nome
            this.lblExtra1Nome.Location = new System.Drawing.Point(15, 125);
            this.lblExtra1Nome.Name = "lblExtra1Nome";
            this.lblExtra1Nome.Size = new System.Drawing.Size(120, 20);
            this.lblExtra1Nome.Text = "Template nome:";

            // txtExtra1Nome
            this.txtExtra1Nome.Location = new System.Drawing.Point(135, 123);
            this.txtExtra1Nome.Name = "txtExtra1Nome";
            this.txtExtra1Nome.Size = new System.Drawing.Size(335, 23);
            this.txtExtra1Nome.TabIndex = 4;
            this.txtExtra1Nome.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtExtra1Nome.TextChanged += new System.EventHandler(this.ExtraPreview_Changed);

            // lblExtra1Subpasta
            this.lblExtra1Subpasta.Location = new System.Drawing.Point(15, 153);
            this.lblExtra1Subpasta.Name = "lblExtra1Subpasta";
            this.lblExtra1Subpasta.Size = new System.Drawing.Size(120, 20);
            this.lblExtra1Subpasta.Text = "Template subpastas:";

            // txtExtra1Subpasta
            this.txtExtra1Subpasta.Location = new System.Drawing.Point(135, 151);
            this.txtExtra1Subpasta.Name = "txtExtra1Subpasta";
            this.txtExtra1Subpasta.Size = new System.Drawing.Size(335, 23);
            this.txtExtra1Subpasta.TabIndex = 5;
            this.txtExtra1Subpasta.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtExtra1Subpasta.TextChanged += new System.EventHandler(this.ExtraPreview_Changed);

            // lblExtra1Preview
            this.lblExtra1Preview.Location = new System.Drawing.Point(15, 180);
            this.lblExtra1Preview.Name = "lblExtra1Preview";
            this.lblExtra1Preview.Size = new System.Drawing.Size(455, 20);
            this.lblExtra1Preview.Font = new System.Drawing.Font("Consolas", 8F);
            this.lblExtra1Preview.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblExtra1Preview.Text = "";

            // ============================
            // grpExtra2 - Caminho Extra 2
            // ============================
            this.grpExtra2.Controls.Add(this.chkExtra2Ativo);
            this.grpExtra2.Controls.Add(this.btnExtra2Copiar);
            this.grpExtra2.Controls.Add(this.lblExtra2Pasta);
            this.grpExtra2.Controls.Add(this.txtExtra2Pasta);
            this.grpExtra2.Controls.Add(this.btnExtra2Browse);
            this.grpExtra2.Controls.Add(this.lblExtra2Reducao);
            this.grpExtra2.Controls.Add(this.numExtra2Reducao);
            this.grpExtra2.Controls.Add(this.lblExtra2ReducaoSuffix);
            this.grpExtra2.Controls.Add(this.lblExtra2Nome);
            this.grpExtra2.Controls.Add(this.txtExtra2Nome);
            this.grpExtra2.Controls.Add(this.lblExtra2Subpasta);
            this.grpExtra2.Controls.Add(this.txtExtra2Subpasta);
            this.grpExtra2.Controls.Add(this.lblExtra2Preview);
            this.grpExtra2.Location = new System.Drawing.Point(8, 218);
            this.grpExtra2.Name = "grpExtra2";
            this.grpExtra2.Size = new System.Drawing.Size(484, 205);
            this.grpExtra2.TabIndex = 1;
            this.grpExtra2.TabStop = false;
            this.grpExtra2.Text = "Caminho Extra 2";

            // chkExtra2Ativo
            this.chkExtra2Ativo.Location = new System.Drawing.Point(15, 22);
            this.chkExtra2Ativo.Name = "chkExtra2Ativo";
            this.chkExtra2Ativo.Size = new System.Drawing.Size(60, 20);
            this.chkExtra2Ativo.Text = "Ativo";
            this.chkExtra2Ativo.TabIndex = 0;
            this.chkExtra2Ativo.CheckedChanged += new System.EventHandler(this.chkExtra2Ativo_CheckedChanged);

            // btnExtra2Copiar
            this.btnExtra2Copiar.Location = new System.Drawing.Point(330, 18);
            this.btnExtra2Copiar.Name = "btnExtra2Copiar";
            this.btnExtra2Copiar.Size = new System.Drawing.Size(140, 25);
            this.btnExtra2Copiar.TabIndex = 6;
            this.btnExtra2Copiar.Text = "Copiar do Principal";
            this.btnExtra2Copiar.UseVisualStyleBackColor = true;
            this.btnExtra2Copiar.Click += new System.EventHandler(this.btnExtra2Copiar_Click);

            // lblExtra2Pasta
            this.lblExtra2Pasta.Location = new System.Drawing.Point(15, 48);
            this.lblExtra2Pasta.Name = "lblExtra2Pasta";
            this.lblExtra2Pasta.Size = new System.Drawing.Size(80, 20);
            this.lblExtra2Pasta.Text = "Pasta:";

            // txtExtra2Pasta
            this.txtExtra2Pasta.Location = new System.Drawing.Point(15, 68);
            this.txtExtra2Pasta.Name = "txtExtra2Pasta";
            this.txtExtra2Pasta.Size = new System.Drawing.Size(410, 23);
            this.txtExtra2Pasta.TabIndex = 1;
            this.txtExtra2Pasta.TextChanged += new System.EventHandler(this.ExtraPreview_Changed);

            // btnExtra2Browse
            this.btnExtra2Browse.Location = new System.Drawing.Point(430, 67);
            this.btnExtra2Browse.Name = "btnExtra2Browse";
            this.btnExtra2Browse.Size = new System.Drawing.Size(40, 25);
            this.btnExtra2Browse.TabIndex = 2;
            this.btnExtra2Browse.Text = "...";
            this.btnExtra2Browse.UseVisualStyleBackColor = true;
            this.btnExtra2Browse.Click += new System.EventHandler(this.btnExtra2Browse_Click);

            // lblExtra2Reducao
            this.lblExtra2Reducao.Location = new System.Drawing.Point(15, 98);
            this.lblExtra2Reducao.Name = "lblExtra2Reducao";
            this.lblExtra2Reducao.Size = new System.Drawing.Size(70, 20);
            this.lblExtra2Reducao.Text = "Redu\u00E7\u00E3o:";

            // numExtra2Reducao
            this.numExtra2Reducao.Location = new System.Drawing.Point(85, 96);
            this.numExtra2Reducao.Minimum = 10;
            this.numExtra2Reducao.Maximum = 100;
            this.numExtra2Reducao.Increment = 5;
            this.numExtra2Reducao.Name = "numExtra2Reducao";
            this.numExtra2Reducao.Size = new System.Drawing.Size(55, 23);
            this.numExtra2Reducao.TabIndex = 3;
            this.numExtra2Reducao.Value = 50;

            // lblExtra2ReducaoSuffix
            this.lblExtra2ReducaoSuffix.Location = new System.Drawing.Point(143, 98);
            this.lblExtra2ReducaoSuffix.Name = "lblExtra2ReducaoSuffix";
            this.lblExtra2ReducaoSuffix.Size = new System.Drawing.Size(300, 20);
            this.lblExtra2ReducaoSuffix.Text = "% da dimens\u00E3o original (ex: 50% de 6000x4000 = 3000x2000)";
            this.lblExtra2ReducaoSuffix.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);

            // lblExtra2Nome
            this.lblExtra2Nome.Location = new System.Drawing.Point(15, 125);
            this.lblExtra2Nome.Name = "lblExtra2Nome";
            this.lblExtra2Nome.Size = new System.Drawing.Size(120, 20);
            this.lblExtra2Nome.Text = "Template nome:";

            // txtExtra2Nome
            this.txtExtra2Nome.Location = new System.Drawing.Point(135, 123);
            this.txtExtra2Nome.Name = "txtExtra2Nome";
            this.txtExtra2Nome.Size = new System.Drawing.Size(335, 23);
            this.txtExtra2Nome.TabIndex = 4;
            this.txtExtra2Nome.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtExtra2Nome.TextChanged += new System.EventHandler(this.ExtraPreview_Changed);

            // lblExtra2Subpasta
            this.lblExtra2Subpasta.Location = new System.Drawing.Point(15, 153);
            this.lblExtra2Subpasta.Name = "lblExtra2Subpasta";
            this.lblExtra2Subpasta.Size = new System.Drawing.Size(120, 20);
            this.lblExtra2Subpasta.Text = "Template subpastas:";

            // txtExtra2Subpasta
            this.txtExtra2Subpasta.Location = new System.Drawing.Point(135, 151);
            this.txtExtra2Subpasta.Name = "txtExtra2Subpasta";
            this.txtExtra2Subpasta.Size = new System.Drawing.Size(335, 23);
            this.txtExtra2Subpasta.TabIndex = 5;
            this.txtExtra2Subpasta.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtExtra2Subpasta.TextChanged += new System.EventHandler(this.ExtraPreview_Changed);

            // lblExtra2Preview
            this.lblExtra2Preview.Location = new System.Drawing.Point(15, 180);
            this.lblExtra2Preview.Name = "lblExtra2Preview";
            this.lblExtra2Preview.Size = new System.Drawing.Size(455, 20);
            this.lblExtra2Preview.Font = new System.Drawing.Font("Consolas", 8F);
            this.lblExtra2Preview.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblExtra2Preview.Text = "";

            // ============================
            // Bot\u00F5es OK / Cancelar
            // ============================
            // btnOk
            this.btnOk.Location = new System.Drawing.Point(340, 485);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(85, 30);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);

            // btnCancelar
            this.btnCancelar.Location = new System.Drawing.Point(435, 485);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(85, 30);
            this.btnCancelar.TabIndex = 2;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

            // ============================
            // FrmConfig
            // ============================
            this.AcceptButton = this.btnOk;
            this.CancelButton = this.btnCancelar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 530);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancelar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmConfig";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configura\u00E7\u00F5es - PicStone+";

            this.tabControl.ResumeLayout(false);
            this.tabGeral.ResumeLayout(false);
            this.tabGeral.PerformLayout();
            this.tabNomeArquivo.ResumeLayout(false);
            this.tabNomeArquivo.PerformLayout();
            this.tabSubpastas.ResumeLayout(false);
            this.tabSubpastas.PerformLayout();
            this.tabCaminhosExtras.ResumeLayout(false);
            this.grpExtra1.ResumeLayout(false);
            this.grpExtra1.PerformLayout();
            this.grpExtra2.ResumeLayout(false);
            this.grpExtra2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDigitoChapa)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDigitoBloco)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numExtra1Reducao)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numExtra2Reducao)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabGeral;
        private System.Windows.Forms.TabPage tabNomeArquivo;
        private System.Windows.Forms.TabPage tabSubpastas;

        // Tab Geral
        private System.Windows.Forms.Label lblPastaBase;
        private System.Windows.Forms.TextBox txtPastaBase;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label lblDigitoChapa;
        private System.Windows.Forms.NumericUpDown numDigitoChapa;
        private System.Windows.Forms.Label lblDigitoBloco;
        private System.Windows.Forms.NumericUpDown numDigitoBloco;
        private System.Windows.Forms.Label lblEstagios;
        private System.Windows.Forms.ListBox lstEstagios;
        private System.Windows.Forms.TextBox txtNovoEstagio;
        private System.Windows.Forms.Button btnEstagioAdd;
        private System.Windows.Forms.Button btnEstagioRemove;

        // Tab Nome Arquivo
        private System.Windows.Forms.Label lblNomeTemplate;
        private System.Windows.Forms.TextBox txtNomeTemplate;
        private System.Windows.Forms.Label lblNomeTokens;
        private System.Windows.Forms.Label lblNomePreview;
        private System.Windows.Forms.Label lblNomePreviewValue;

        // Tab Subpastas
        private System.Windows.Forms.Label lblPastaTemplate;
        private System.Windows.Forms.TextBox txtPastaTemplate;
        private System.Windows.Forms.Label lblPastaTokens;
        private System.Windows.Forms.Label lblPastaPreview;
        private System.Windows.Forms.Label lblPastaPreviewValue;

        // Tab Caminhos Extras
        private System.Windows.Forms.TabPage tabCaminhosExtras;
        private System.Windows.Forms.GroupBox grpExtra1;
        private System.Windows.Forms.CheckBox chkExtra1Ativo;
        private System.Windows.Forms.Button btnExtra1Copiar;
        private System.Windows.Forms.Label lblExtra1Pasta;
        private System.Windows.Forms.TextBox txtExtra1Pasta;
        private System.Windows.Forms.Button btnExtra1Browse;
        private System.Windows.Forms.Label lblExtra1Reducao;
        private System.Windows.Forms.NumericUpDown numExtra1Reducao;
        private System.Windows.Forms.Label lblExtra1ReducaoSuffix;
        private System.Windows.Forms.Label lblExtra1Nome;
        private System.Windows.Forms.TextBox txtExtra1Nome;
        private System.Windows.Forms.Label lblExtra1Subpasta;
        private System.Windows.Forms.TextBox txtExtra1Subpasta;
        private System.Windows.Forms.Label lblExtra1Preview;

        private System.Windows.Forms.GroupBox grpExtra2;
        private System.Windows.Forms.CheckBox chkExtra2Ativo;
        private System.Windows.Forms.Button btnExtra2Copiar;
        private System.Windows.Forms.Label lblExtra2Pasta;
        private System.Windows.Forms.TextBox txtExtra2Pasta;
        private System.Windows.Forms.Button btnExtra2Browse;
        private System.Windows.Forms.Label lblExtra2Reducao;
        private System.Windows.Forms.NumericUpDown numExtra2Reducao;
        private System.Windows.Forms.Label lblExtra2ReducaoSuffix;
        private System.Windows.Forms.Label lblExtra2Nome;
        private System.Windows.Forms.TextBox txtExtra2Nome;
        private System.Windows.Forms.Label lblExtra2Subpasta;
        private System.Windows.Forms.TextBox txtExtra2Subpasta;
        private System.Windows.Forms.Label lblExtra2Preview;

        // Rodap\u00E9
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancelar;
    }
}
