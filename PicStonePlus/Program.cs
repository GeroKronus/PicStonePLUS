using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Windows.Forms;

namespace PicStonePlus
{
    static class Program
    {
        static string _logPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "nikon_debug.log");

        [STAThread]
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                string msg = ex != null
                    ? $"CRASH: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}"
                    : $"CRASH: {e.ExceptionObject}";
                try { File.AppendAllText(_logPath, $"[{DateTime.Now:HH:mm:ss.fff}] {msg}\r\n"); } catch { }
                MessageBox.Show(msg, "Erro Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            Application.ThreadException += (s, e) =>
            {
                string msg = $"THREAD CRASH: {e.Exception.GetType().Name}: {e.Exception.Message}\n{e.Exception.StackTrace}";
                try { File.AppendAllText(_logPath, $"[{DateTime.Now:HH:mm:ss.fff}] {msg}\r\n"); } catch { }
                MessageBox.Show(msg, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            Application.Run(new Forms.MainForm());
        }
    }
}
