using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace PicStonePlus.Models
{
    public class MaterialPreset
    {
        // Identificação
        public string Nome { get; set; }

        // Configurações de câmera - índices são fallback, textos são usados para busca cross-camera
        public int ISOIndex { get; set; }
        public int ApertureIndex { get; set; }
        public int ShutterSpeedIndex { get; set; }
        public int PictureControlIndex { get; set; }
        public double Temperatura { get; set; } // 0 = WB Auto, >0 = Kelvin (D7500/D7200: Range)
        public bool AutoFoco { get; set; } = true; // true = AF, false = MF

        // Valores textuais (usados para aplicar presets entre câmeras diferentes)
        public string ISOText { get; set; }
        public string ApertureText { get; set; }
        public string ShutterSpeedText { get; set; }
        public string PictureControlText { get; set; }

        // Pós-produção (processamento em software, não enviados à câmera)
        public int Brilho { get; set; }
        public int Contraste { get; set; }
        public int Sombras { get; set; }
        public int Vermelho { get; set; }
        public int Verde { get; set; }
        public int Azul { get; set; }
        public int Saturacao { get; set; }
        public int Matiz { get; set; }
        public int Gama { get; set; }
        public int Tonalidade { get; set; }
    }

    public static class PresetManager
    {
        private static string PresetFilePath
        {
            get
            {
                string dir = Path.GetDirectoryName(Application.ExecutablePath) ?? ".";
                return Path.Combine(dir, "presets.json");
            }
        }

        public static List<MaterialPreset> Load()
        {
            try
            {
                if (!File.Exists(PresetFilePath))
                    return new List<MaterialPreset>();

                string json = File.ReadAllText(PresetFilePath);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<MaterialPreset>();

                var serializer = new JavaScriptSerializer();
                return serializer.Deserialize<List<MaterialPreset>>(json) ?? new List<MaterialPreset>();
            }
            catch
            {
                return new List<MaterialPreset>();
            }
        }

        public static void Save(List<MaterialPreset> presets)
        {
            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(presets);

            // Formatar JSON para legibilidade
            json = FormatJson(json);

            File.WriteAllText(PresetFilePath, json);
        }

        private static string FormatJson(string json)
        {
            // Formatação simples sem dependências externas
            var sb = new System.Text.StringBuilder();
            int indent = 0;
            bool inString = false;
            bool escaped = false;

            foreach (char c in json)
            {
                if (escaped)
                {
                    sb.Append(c);
                    escaped = false;
                    continue;
                }

                if (c == '\\' && inString)
                {
                    sb.Append(c);
                    escaped = true;
                    continue;
                }

                if (c == '"')
                {
                    inString = !inString;
                    sb.Append(c);
                    continue;
                }

                if (inString)
                {
                    sb.Append(c);
                    continue;
                }

                switch (c)
                {
                    case '{':
                    case '[':
                        sb.Append(c);
                        sb.AppendLine();
                        indent++;
                        sb.Append(new string(' ', indent * 2));
                        break;
                    case '}':
                    case ']':
                        sb.AppendLine();
                        indent--;
                        sb.Append(new string(' ', indent * 2));
                        sb.Append(c);
                        break;
                    case ',':
                        sb.Append(c);
                        sb.AppendLine();
                        sb.Append(new string(' ', indent * 2));
                        break;
                    case ':':
                        sb.Append(c);
                        sb.Append(' ');
                        break;
                    default:
                        if (!char.IsWhiteSpace(c))
                            sb.Append(c);
                        break;
                }
            }

            return sb.ToString();
        }
    }
}
