using System.Threading;
using System.Windows.Forms;

namespace KioskLauncher
{
    internal static class Program
    {
        [System.STAThread]
        static void Main()
        {
            bool createdNew;
            using (var mutex = new Mutex(true, "Global\\KioskLauncher_SingleInstance", out createdNew))
            {
                if (!createdNew)
                {
                    MessageBox.Show(
                        "Kiosk Launcher is already running.",
                        "Kiosk Launcher",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }
    }
}
