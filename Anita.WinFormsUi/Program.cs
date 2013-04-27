using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SachsenCoder.Anita.Contracts.Data;
using SachsenCoder.Anita.Core.Compositions;
using SachsenCoder.Anita.Core.Leafs;
using SachsenCoder.Anita.Core;

namespace ImageGrabber
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var ui = new Form1();
            var coreComposition = new CoreComposition(ui);

            Application.Run(ui);
        }
    }
}
