using System;
using System.Windows.Forms;

namespace LaserSuburbLookup
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(System.Windows.Forms.HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UI.MainForm());
        }
    }
}
