using System;
using System.Windows.Forms;

namespace Aseguradora.Cotizador.WinForms
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmCotizacionAuto());
        }
    }
}
