using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Talos
{
    internal static class Program
    {
        private static MainForm _mainForm;


        internal static MainForm MainForm
        {
            get
            {
                if (_mainForm == null)
                {
                    _mainForm = (MainForm)Application.OpenForms[0];
                }
                return _mainForm;
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
           
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        internal static void ExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            string text = AppDomain.CurrentDomain.BaseDirectory + "CrashLogs\\";
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            text = text + DateTime.Now.ToString("MM-dd-HH-yyyy h mm tt") + ".log";
            File.WriteAllText(text, (e.ExceptionObject as Exception).ToString());
        }
    }
}
