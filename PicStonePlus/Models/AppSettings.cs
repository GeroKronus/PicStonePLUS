using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace PicStonePlus.Models
{
    public class AppSettings
    {
        public string PastaBase { get; set; }

        public int DigitoChapa { get; set; } = 3;

        // Zeros à esquerda do bloco (0 = sem zeros, 5 = "00123")
        public int DigitoBloco { get; set; } = 0;

        // Template livre do nome do arquivo
        // Tokens: {Material}, {Espessura}, {Bloco}, {Bundle}, {Estagio}, {Chapa}, {Data}, {Hora}
        // Exemplo: "{Material} {Espessura}CM Block {Bloco} Slab {Chapa}"
        public string TemplateNomeArquivo { get; set; } = "{Material} {Espessura} {Bloco} {Bundle} {Chapa}";

        // Template livre da estrutura de subpastas
        // Use \ para separar níveis de pasta. Tokens e texto livre podem ser combinados.
        // Tokens: {Ano}, {Mes}, {Material}, {Bloco}, {Espessura}, {Estagio}
        // Exemplo: "{Ano}\{Mes}\{Material}_{Bloco}"
        public string TemplateSubpastas { get; set; } = @"{Ano}\{Mes}\{Material}";

        // Lista de estágios cadastrados (editável via Configurações)
        public List<string> Estagios { get; set; } = new List<string>
            { "POLIDA", "LEVIGADA", "RESINADA", "BRUTA", "FLAMEADA", "APIACOADA", "ESCOVADA" };

        // Caminhos extras de salvamento (com redução de dimensão)
        public List<CaminhoExtra> CaminhosExtras { get; set; } = new List<CaminhoExtra>
            { new CaminhoExtra(), new CaminhoExtra() };

        public AppSettings()
        {
            PastaBase = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                "NikonD7500");
        }
    }

    public class CaminhoExtra
    {
        public bool Ativo { get; set; } = false;
        public string PastaBase { get; set; } = "";
        public int Reducao { get; set; } = 50;  // percentual (10-100)
        public string TemplateNomeArquivo { get; set; } = "{Material} {Espessura} {Bloco} {Chapa}";
        public string TemplateSubpastas { get; set; } = @"{Ano}\{Mes}\{Material}";
    }

    public static class AppSettingsManager
    {
        private static string ConfigFilePath
        {
            get
            {
                string dir = Path.GetDirectoryName(Application.ExecutablePath) ?? ".";
                return Path.Combine(dir, "config.json");
            }
        }

        private static AppSettings _cached;

        public static AppSettings Load()
        {
            if (_cached != null)
                return _cached;

            try
            {
                if (!File.Exists(ConfigFilePath))
                {
                    _cached = new AppSettings();
                    return _cached;
                }

                string json = File.ReadAllText(ConfigFilePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    _cached = new AppSettings();
                    return _cached;
                }

                var serializer = new JavaScriptSerializer();
                _cached = serializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                return _cached;
            }
            catch
            {
                _cached = new AppSettings();
                return _cached;
            }
        }

        public static void Save(AppSettings settings)
        {
            _cached = settings;

            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(settings);
            json = FormatJson(json);
            File.WriteAllText(ConfigFilePath, json);
        }

        public static void Invalidate()
        {
            _cached = null;
        }

        private static string FormatJson(string json)
        {
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
