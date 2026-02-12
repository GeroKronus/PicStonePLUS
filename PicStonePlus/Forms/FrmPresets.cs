using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PicStonePlus.Models;
using PicStonePlus.SDK;

namespace PicStonePlus.Forms
{
    public partial class FrmPresets : Form
    {
        private List<MaterialPreset> _presets;
        private readonly NikonManager _nikonManager;

        // Temperaturas de cor disponíveis (D7200/D7500): 2500K-10000K, step 50K
        private static readonly int[] TemperaturasKelvin;

        static FrmPresets()
        {
            // 2500 a 10000 em passos de 50 = 151 valores
            TemperaturasKelvin = new int[(10000 - 2500) / 50 + 1];
            for (int i = 0; i < TemperaturasKelvin.Length; i++)
                TemperaturasKelvin[i] = 2500 + i * 50;
        }

        /// <summary>Nome do preset que foi salvo por último (null se nenhum foi salvo).</summary>
        public string LastSavedPresetName { get; private set; }

        public FrmPresets(NikonManager nikonManager, string selectPresetName = null)
        {
            InitializeComponent();
            _nikonManager = nikonManager;
            _presets = PresetManager.Load();

            PopulateTemperaturaCombo();
            PopulateCameraCombos();
            RefreshList();

            // Selecionar preset inicial, se informado
            if (!string.IsNullOrEmpty(selectPresetName))
            {
                for (int i = 0; i < _presets.Count; i++)
                {
                    if (_presets[i].Nome == selectPresetName)
                    {
                        lstPresets.SelectedIndex = i;
                        break;
                    }
                }
            }

            if (lstPresets.SelectedIndex < 0)
                EnableDetails(false);

            btnCapturarCamera.Enabled = _nikonManager != null && _nikonManager.IsConnected;
        }

        #region Lista de Presets

        private void RefreshList()
        {
            lstPresets.Items.Clear();
            foreach (var p in _presets)
                lstPresets.Items.Add(p.Nome);
        }

        private void lstPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPresets.SelectedIndex < 0)
            {
                EnableDetails(false);
                return;
            }

            EnableDetails(true);
            LoadPresetToFields(_presets[lstPresets.SelectedIndex]);
        }

        private void EnableDetails(bool enabled)
        {
            grpCamera.Enabled = enabled;
            grpPosProducao.Enabled = enabled;
            btnSalvar.Enabled = enabled;
            btnCapturarCamera.Enabled = enabled && _nikonManager != null && _nikonManager.IsConnected;
        }

        #endregion

        #region Novo / Excluir

        private void btnNovo_Click(object sender, EventArgs e)
        {
            string nome = PromptNome("Novo Preset", "Nome do material:");
            if (string.IsNullOrWhiteSpace(nome))
                return;

            var preset = new MaterialPreset { Nome = nome };
            _presets.Add(preset);
            PresetManager.Save(_presets);
            RefreshList();
            lstPresets.SelectedIndex = _presets.Count - 1;
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (lstPresets.SelectedIndex < 0) return;

            string nome = _presets[lstPresets.SelectedIndex].Nome;
            var result = MessageBox.Show(
                $"Excluir o preset \"{nome}\"?",
                "Confirmar Exclusão",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            _presets.RemoveAt(lstPresets.SelectedIndex);
            PresetManager.Save(_presets);
            RefreshList();
            EnableDetails(false);
        }

        #endregion

        #region Carregar / Salvar Campos

        private void LoadPresetToFields(MaterialPreset preset)
        {
            txtNome.Text = preset.Nome;

            SafeSelectIndex(cboISO, preset.ISOIndex);
            SafeSelectIndex(cboAbertura, preset.ApertureIndex);
            SafeSelectIndex(cboVelocidade, preset.ShutterSpeedIndex);
            SafeSelectIndex(cboPictureControl, preset.PictureControlIndex);

            SelectTemperatura(preset.Temperatura);

            nudBrilho.Value = Clamp(preset.Brilho);
            nudContraste.Value = Clamp(preset.Contraste);
            nudSombras.Value = Clamp(preset.Sombras);
            nudVermelho.Value = Clamp(preset.Vermelho);
            nudVerde.Value = Clamp(preset.Verde);
            nudAzul.Value = Clamp(preset.Azul);
            nudSaturacao.Value = Clamp(preset.Saturacao);
            nudMatiz.Value = Clamp(preset.Matiz);
            nudGama.Value = Clamp(preset.Gama);
            nudTonalidade.Value = Clamp(preset.Tonalidade);
        }

        private MaterialPreset ReadFieldsToPreset()
        {
            return new MaterialPreset
            {
                Nome = txtNome.Text.Trim(),

                ISOIndex = cboISO.SelectedIndex,
                ApertureIndex = cboAbertura.SelectedIndex,
                ShutterSpeedIndex = cboVelocidade.SelectedIndex,
                PictureControlIndex = cboPictureControl.SelectedIndex,
                Temperatura = GetSelectedTemperatura(),

                ISOText = cboISO.SelectedIndex >= 0 ? cboISO.Text : "",
                ApertureText = cboAbertura.SelectedIndex >= 0 ? cboAbertura.Text : "",
                ShutterSpeedText = cboVelocidade.SelectedIndex >= 0 ? cboVelocidade.Text : "",

                Brilho = (int)nudBrilho.Value,
                Contraste = (int)nudContraste.Value,
                Sombras = (int)nudSombras.Value,
                Vermelho = (int)nudVermelho.Value,
                Verde = (int)nudVerde.Value,
                Azul = (int)nudAzul.Value,
                Saturacao = (int)nudSaturacao.Value,
                Matiz = (int)nudMatiz.Value,
                Gama = (int)nudGama.Value,
                Tonalidade = (int)nudTonalidade.Value
            };
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (lstPresets.SelectedIndex < 0) return;

            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("O nome do preset não pode ser vazio.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var preset = ReadFieldsToPreset();
            _presets[lstPresets.SelectedIndex] = preset;
            PresetManager.Save(_presets);
            LastSavedPresetName = preset.Nome;

            // Fechar e voltar ao MainForm (que aplica automaticamente)
            Close();
        }

        #endregion

        #region Capturar da Câmera

        private void btnCapturarCamera_Click(object sender, EventArgs e)
        {
            if (_nikonManager == null || !_nikonManager.IsConnected)
            {
                MessageBox.Show("Câmera não conectada.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Ler configurações atuais da câmera e popular os combos locais
            CaptureEnumFromCamera(cboISO, (uint)eNkMAIDCapability.kNkMAIDCapability_Sensitivity);
            CaptureEnumFromCamera(cboAbertura, (uint)eNkMAIDCapability.kNkMAIDCapability_Aperture);
            CaptureEnumFromCamera(cboVelocidade, (uint)eNkMAIDCapability.kNkMAIDCapability_ShutterSpeed);
            CaptureEnumFromCamera(cboPictureControl, (uint)eNkMAIDCapability.kNkMAIDCapability_PictureControl);

            // Temperatura: ler WBTuneColorTempEx (D7500/D7200 = Range)
            try
            {
                double value, lower, upper;
                uint steps;
                if (_nikonManager.GetRangeCapability(
                    (uint)eNkMAIDCapability.kNkMAIDCapability_WBTuneColorTempEx,
                    out value, out lower, out upper, out steps))
                {
                    SelectTemperatura(value);
                }
            }
            catch { }

            MessageBox.Show("Configurações da câmera capturadas!", "Capturar",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CaptureEnumFromCamera(ComboBox combo, uint capId)
        {
            List<string> options;
            int currentIndex;
            if (_nikonManager.GetEnumCapability(capId, out options, out currentIndex))
            {
                combo.Items.Clear();
                foreach (string opt in options)
                    combo.Items.Add(opt);
                if (currentIndex >= 0 && currentIndex < combo.Items.Count)
                    combo.SelectedIndex = currentIndex;
            }
        }

        #endregion

        #region Popular Combos (a partir da câmera conectada)

        private void PopulateCameraCombos()
        {
            if (_nikonManager == null || !_nikonManager.IsConnected)
                return;

            PopulateComboFromCamera(cboISO, (uint)eNkMAIDCapability.kNkMAIDCapability_Sensitivity);
            PopulateComboFromCamera(cboAbertura, (uint)eNkMAIDCapability.kNkMAIDCapability_Aperture);
            PopulateComboFromCamera(cboVelocidade, (uint)eNkMAIDCapability.kNkMAIDCapability_ShutterSpeed);
            PopulateComboFromCamera(cboPictureControl, (uint)eNkMAIDCapability.kNkMAIDCapability_PictureControl);
        }

        private void PopulateComboFromCamera(ComboBox combo, uint capId)
        {
            List<string> options;
            int currentIndex;
            if (_nikonManager.GetEnumCapability(capId, out options, out currentIndex))
            {
                combo.Items.Clear();
                foreach (string opt in options)
                    combo.Items.Add(opt);
            }
        }

        #endregion

        #region Temperatura de Cor

        private void PopulateTemperaturaCombo()
        {
            cboTemperatura.Items.Clear();
            cboTemperatura.Items.Add("Auto (WB)");
            foreach (int k in TemperaturasKelvin)
                cboTemperatura.Items.Add($"{k} K");
            cboTemperatura.SelectedIndex = 0;
        }

        private void SelectTemperatura(double kelvin)
        {
            if (kelvin <= 0)
            {
                cboTemperatura.SelectedIndex = 0; // Auto
                return;
            }

            // Encontrar valor mais próximo
            int closest = 0;
            double minDiff = double.MaxValue;
            for (int i = 0; i < TemperaturasKelvin.Length; i++)
            {
                double diff = Math.Abs(TemperaturasKelvin[i] - kelvin);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    closest = i;
                }
            }
            cboTemperatura.SelectedIndex = closest + 1; // +1 por causa do "Auto"
        }

        private double GetSelectedTemperatura()
        {
            int idx = cboTemperatura.SelectedIndex;
            if (idx <= 0)
                return 0; // Auto
            return TemperaturasKelvin[idx - 1];
        }

        #endregion

        #region Helpers

        private static void SafeSelectIndex(ComboBox combo, int index)
        {
            if (index >= 0 && index < combo.Items.Count)
                combo.SelectedIndex = index;
            else
                combo.SelectedIndex = -1;
        }

        private decimal Clamp(int value)
        {
            if (value < (int)nudBrilho.Minimum) return nudBrilho.Minimum;
            if (value > (int)nudBrilho.Maximum) return nudBrilho.Maximum;
            return value;
        }

        private string PromptNome(string title, string prompt)
        {
            using (var dlg = new Form())
            {
                dlg.Text = title;
                dlg.Size = new System.Drawing.Size(350, 150);
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;

                var lbl = new Label { Text = prompt, Location = new System.Drawing.Point(12, 15), Size = new System.Drawing.Size(310, 20) };
                var txt = new TextBox { Location = new System.Drawing.Point(12, 40), Size = new System.Drawing.Size(310, 23) };
                var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new System.Drawing.Point(162, 75), Size = new System.Drawing.Size(75, 28) };
                var btnCancel = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new System.Drawing.Point(247, 75), Size = new System.Drawing.Size(75, 28) };

                dlg.Controls.AddRange(new Control[] { lbl, txt, btnOk, btnCancel });
                dlg.AcceptButton = btnOk;
                dlg.CancelButton = btnCancel;

                if (dlg.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(txt.Text))
                    return txt.Text.Trim();

                return null;
            }
        }

        #endregion
    }
}
