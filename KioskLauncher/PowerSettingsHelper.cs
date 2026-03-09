using System.Diagnostics;
using Microsoft.Win32;

namespace KioskLauncher
{
    internal static class PowerSettingsHelper
    {
        public static void Enable()
        {
            RunPowercfg("/change monitor-timeout-ac 0");
            RunPowercfg("/change monitor-timeout-dc 0");
            RunPowercfg("/change standby-timeout-ac 0");
            RunPowercfg("/change standby-timeout-dc 0");

            using (var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", writable: true))
            {
                if (key == null) return;
                key.SetValue("ScreenSaveActive",   "0", RegistryValueKind.String);
                key.SetValue("ScreenSaveIsSecure", "0", RegistryValueKind.String);
            }
        }

        public static void Disable()
        {
            RunPowercfg("/change monitor-timeout-ac 15");
            RunPowercfg("/change monitor-timeout-dc 10");
            RunPowercfg("/change standby-timeout-ac 30");
            RunPowercfg("/change standby-timeout-dc 15");

            using (var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", writable: true))
            {
                if (key == null) return;
                key.SetValue("ScreenSaveActive",   "1", RegistryValueKind.String);
                key.SetValue("ScreenSaveIsSecure", "0", RegistryValueKind.String);
            }
        }

        private static void RunPowercfg(string args)
        {
            var psi = new ProcessStartInfo("powercfg.exe", args)
            {
                CreateNoWindow     = true,
                UseShellExecute    = false,
                RedirectStandardOutput = true,
                RedirectStandardError  = true
            };
            using (var p = Process.Start(psi))
            {
                p?.WaitForExit();
            }
        }
    }
}
