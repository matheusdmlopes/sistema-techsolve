using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PIM.Desktop.Forms;

namespace PIM.Desktop
{
    internal static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo TechSolve Desktop.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }
}
