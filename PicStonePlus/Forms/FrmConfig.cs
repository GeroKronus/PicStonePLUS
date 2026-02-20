using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using PicStonePlus.Models;

namespace PicStonePlus.Forms
{
    public partial class FrmConfig : Form
    {
        private AppSettings _settings;

        public FrmConfig()
        {
            InitializeComponent();
            _settings = AppSettingsManager.Load();
            CarregarConfiguracoes();
        }

        private void CarregarConfiguracoes()
        {
            // Tab Geral
            txtPastaBase.Text = _settings.PastaBase;
            numDigitoChapa.Value = Math.Max(1, Math.Min(6, _settings.DigitoChapa));
            numDigitoBloco.Value = Math.Max(0, Math.Min(8, _settings.DigitoBloco));

            // Est\u00E1gios
            lstEstagios.Items.Clear();
            foreach (var est in _settings.Estagios)
                lstEstagios.Items.Add(est);

            // Tab Nome do Arquivo
            txtNomeTemplate.Text = _settings.TemplateNomeArquivo;

            // Tab Subpastas
            txtPastaTemplate.Text = _settings.TemplateSubpastas;

            // Tab Caminhos Extras
            if (_settings.CaminhosExtras != null && _settings.CaminhosExtras.Count >= 2)
            {
                var e1 = _settings.CaminhosExtras[0];
                chkExtra1Ativo.Checked = e1.Ativo;
                txtExtra1Pasta.Text = e1.PastaBase;
                numExtra1Reducao.Value = Math.Max(10, Math.Min(100, e1.Reducao));
                txtExtra1Nome.Text = e1.TemplateNomeArquivo;
                txtExtra1Subpasta.Text = e1.TemplateSubpastas;

                var e2 = _settings.CaminhosExtras[1];
                chkExtra2Ativo.Checked = e2.Ativo;
                txtExtra2Pasta.Text = e2.PastaBase;
                numExtra2Reducao.Value = Math.Max(10, Math.Min(100, e2.Reducao));
                txtExtra2Nome.Text = e2.TemplateNomeArquivo;
                txtExtra2Subpasta.Text = e2.TemplateSubpastas;
            }
            AtualizarExtra1Enabled();
            AtualizarExtra2Enabled();

            // Previews
            AtualizarNomePreview();
            AtualizarPastaPreview();
            AtualizarExtraPreview();
        }

        #region Tab Geral

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Selecione a pasta base para salvar as fotos";
                dlg.SelectedPath = txtPastaBase.Text;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    txtPastaBase.Text = dlg.SelectedPath;
            }
        }

        private void btnEstagioAdd_Click(object sender, EventArgs e)
        {
            string novo = txtNovoEstagio.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(novo)) return;

            foreach (var item in lstEstagios.Items)
            {
                if (item.ToString().Equals(novo, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Este est\u00E1gio j\u00E1 existe na lista.",
                        "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            lstEstagios.Items.Add(novo);
            txtNovoEstagio.Clear();
            txtNovoEstagio.Focus();
        }

        private void btnEstagioRemove_Click(object sender, EventArgs e)
        {
            if (lstEstagios.SelectedItem == null) return;
            lstEstagios.Items.RemoveAt(lstEstagios.SelectedIndex);
        }

        #endregion

        #region Tab Nome do Arquivo

        private void txtNomeTemplate_TextChanged(object sender, EventArgs e)
        {
            AtualizarNomePreview();
        }

        private void AtualizarNomePreview()
        {
            string template = txtNomeTemplate.Text;
            if (string.IsNullOrWhiteSpace(template))
            {
                lblNomePreviewValue.Text = "(template vazio)";
                return;
            }

            int digitos = (int)numDigitoChapa.Value;
            string resultado = ResolverTemplateNome(template, digitos);
            lblNomePreviewValue.Text = resultado + ".jpg";
        }

        private string ResolverTemplateNome(string template, int digitos)
        {
            int digBloco = (int)numDigitoBloco.Value;
            string blocoEx = digBloco > 0 ? "123".PadLeft(digBloco, '0') : "12345";
            string r = template;
            r = r.Replace("{Material}", "AURORA BOREAL");
            r = r.Replace("{Espessura}", "2");
            r = r.Replace("{Bloco}", blocoEx);
            r = r.Replace("{Bundle}", "A");
            r = r.Replace("{Estagio}", "POLIDA");
            r = r.Replace("{Chapa}", new string('0', Math.Max(digitos - 1, 0)) + "1");
            r = r.Replace("{Data}", DateTime.Now.ToString("yyyyMMdd"));
            r = r.Replace("{Hora}", DateTime.Now.ToString("HHmmss"));
            return r;
        }

        #endregion

        #region Tab Subpastas

        private void txtPastaTemplate_TextChanged(object sender, EventArgs e)
        {
            AtualizarPastaPreview();
        }

        private void AtualizarPastaPreview()
        {
            string template = txtPastaTemplate.Text;
            if (string.IsNullOrWhiteSpace(template))
            {
                lblPastaPreviewValue.Text = txtPastaBase.Text + @"\";
                return;
            }

            string resolved = ResolverTemplatePasta(template);
            string caminho = Path.Combine(txtPastaBase.Text, resolved);
            lblPastaPreviewValue.Text = caminho + @"\";
        }

        private string ResolverTemplatePasta(string template)
        {
            int digBloco = (int)numDigitoBloco.Value;
            string blocoEx = digBloco > 0 ? "123".PadLeft(digBloco, '0') : "12345";
            string r = template;
            r = r.Replace("{Ano}", DateTime.Now.ToString("yyyy"));
            r = r.Replace("{Mes}", DateTime.Now.ToString("MM - MMMM").ToUpper());
            r = r.Replace("{Material}", "AURORA BOREAL");
            r = r.Replace("{Bloco}", blocoEx);
            r = r.Replace("{Espessura}", "2");
            r = r.Replace("{Estagio}", "POLIDA");
            return r;
        }

        #endregion

        #region Tab Caminhos Extras

        private void chkExtra1Ativo_CheckedChanged(object sender, EventArgs e)
        {
            AtualizarExtra1Enabled();
        }

        private void chkExtra2Ativo_CheckedChanged(object sender, EventArgs e)
        {
            AtualizarExtra2Enabled();
        }

        private void AtualizarExtra1Enabled()
        {
            bool ativo = chkExtra1Ativo.Checked;
            txtExtra1Pasta.Enabled = ativo;
            btnExtra1Browse.Enabled = ativo;
            numExtra1Reducao.Enabled = ativo;
            txtExtra1Nome.Enabled = ativo;
            txtExtra1Subpasta.Enabled = ativo;
        }

        private void AtualizarExtra2Enabled()
        {
            bool ativo = chkExtra2Ativo.Checked;
            txtExtra2Pasta.Enabled = ativo;
            btnExtra2Browse.Enabled = ativo;
            numExtra2Reducao.Enabled = ativo;
            txtExtra2Nome.Enabled = ativo;
            txtExtra2Subpasta.Enabled = ativo;
        }

        private void btnExtra1Copiar_Click(object sender, EventArgs e)
        {
            txtExtra1Pasta.Text = txtPastaBase.Text;
            txtExtra1Nome.Text = txtNomeTemplate.Text;
            txtExtra1Subpasta.Text = txtPastaTemplate.Text;
            AtualizarExtraPreview();
        }

        private void btnExtra2Copiar_Click(object sender, EventArgs e)
        {
            txtExtra2Pasta.Text = txtPastaBase.Text;
            txtExtra2Nome.Text = txtNomeTemplate.Text;
            txtExtra2Subpasta.Text = txtPastaTemplate.Text;
            AtualizarExtraPreview();
        }

        private void btnExtra1Browse_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Selecione a pasta para o Caminho Extra 1";
                dlg.SelectedPath = txtExtra1Pasta.Text;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    txtExtra1Pasta.Text = dlg.SelectedPath;
            }
        }

        private void btnExtra2Browse_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Selecione a pasta para o Caminho Extra 2";
                dlg.SelectedPath = txtExtra2Pasta.Text;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    txtExtra2Pasta.Text = dlg.SelectedPath;
            }
        }

        private void ExtraPreview_Changed(object sender, EventArgs e)
        {
            AtualizarExtraPreview();
        }

        private void AtualizarExtraPreview()
        {
            int digitos = (int)numDigitoChapa.Value;

            // Preview Extra 1
            string nome1 = ResolverTemplateNome(txtExtra1Nome.Text, digitos);
            string pasta1 = txtExtra1Pasta.Text;
            if (!string.IsNullOrWhiteSpace(txtExtra1Subpasta.Text))
                pasta1 = Path.Combine(pasta1, ResolverTemplatePasta(txtExtra1Subpasta.Text));
            lblExtra1Preview.Text = string.IsNullOrEmpty(pasta1)
                ? nome1 + ".jpg"
                : Path.Combine(pasta1, nome1 + ".jpg");

            // Preview Extra 2
            string nome2 = ResolverTemplateNome(txtExtra2Nome.Text, digitos);
            string pasta2 = txtExtra2Pasta.Text;
            if (!string.IsNullOrWhiteSpace(txtExtra2Subpasta.Text))
                pasta2 = Path.Combine(pasta2, ResolverTemplatePasta(txtExtra2Subpasta.Text));
            lblExtra2Preview.Text = string.IsNullOrEmpty(pasta2)
                ? nome2 + ".jpg"
                : Path.Combine(pasta2, nome2 + ".jpg");
        }

        #endregion

        #region OK / Cancelar

        private void btnOk_Click(object sender, EventArgs e)
        {
            string pasta = txtPastaBase.Text.Trim();
            if (string.IsNullOrEmpty(pasta))
            {
                MessageBox.Show("A pasta base n\u00E3o pode estar vazia.",
                    "Valida\u00E7\u00E3o", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedTab = tabGeral;
                txtPastaBase.Focus();
                return;
            }

            _settings.PastaBase = pasta;
            _settings.DigitoChapa = (int)numDigitoChapa.Value;
            _settings.DigitoBloco = (int)numDigitoBloco.Value;
            _settings.TemplateNomeArquivo = txtNomeTemplate.Text;
            _settings.TemplateSubpastas = txtPastaTemplate.Text;

            _settings.Estagios = new List<string>();
            foreach (var item in lstEstagios.Items)
                _settings.Estagios.Add(item.ToString());

            // Caminhos Extras
            if (_settings.CaminhosExtras == null || _settings.CaminhosExtras.Count < 2)
                _settings.CaminhosExtras = new List<CaminhoExtra> { new CaminhoExtra(), new CaminhoExtra() };

            _settings.CaminhosExtras[0].Ativo = chkExtra1Ativo.Checked;
            _settings.CaminhosExtras[0].PastaBase = txtExtra1Pasta.Text.Trim();
            _settings.CaminhosExtras[0].Reducao = (int)numExtra1Reducao.Value;
            _settings.CaminhosExtras[0].TemplateNomeArquivo = txtExtra1Nome.Text;
            _settings.CaminhosExtras[0].TemplateSubpastas = txtExtra1Subpasta.Text;

            _settings.CaminhosExtras[1].Ativo = chkExtra2Ativo.Checked;
            _settings.CaminhosExtras[1].PastaBase = txtExtra2Pasta.Text.Trim();
            _settings.CaminhosExtras[1].Reducao = (int)numExtra2Reducao.Value;
            _settings.CaminhosExtras[1].TemplateNomeArquivo = txtExtra2Nome.Text;
            _settings.CaminhosExtras[1].TemplateSubpastas = txtExtra2Subpasta.Text;

            AppSettingsManager.Save(_settings);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion
    }
}
