using System;
using System.Windows.Forms;
using SachsenCoder.Anita.Core;

namespace SachsenCoder.Anita.WinFormsUi
{
    internal static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var ui = new MainWindow();
            var coreComposition = new CoreComposition(ui);

            coreComposition.LoadSettings();
            Application.Run(ui);
        }
    }
}