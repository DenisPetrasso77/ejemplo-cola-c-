using System;
using System.Windows.Forms;

namespace ColaAnimadaCool
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ColaForm()); // tu formulario principal
        }
    }
}