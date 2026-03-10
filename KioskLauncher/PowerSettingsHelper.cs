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

            // Disable USB Selective Suspend on High Performance plan, then activate it.
            // This prevents idle USB/CPU state transitions that cause driver crashes on fanless hardware.
            // Subgroup GUID: 2a737441-1930-4402-8d77-b2bebba308a3 (USB settings)
            // Setting GUID:  48e6b7a6-50f5-4782-a5d4-53bb8f07e226 (USB selective suspend timeout; 0=disabled)
            RunPowercfg("/setacvaluesetting SCHEME_MIN 2a737441-1930-4402-8d77-b2bebba308a3 48e6b7a6-50f5-4782-a5d4-53bb8f07e226 0");
            RunPowercfg("/setactive SCHEME_MIN");

            using (var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", writable: true))
            {
                if (key == null) return;
                key.SetValue("ScreenSaveActive",   "0", RegistryValueKind.String);
                key.SetValue("ScreenSaveIsSecure", "0", RegistryValueKind.String);
            }
        }

        public static void Disable()
        {
            RunPowercfg("/setactive SCHEME_BALANCED");

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
