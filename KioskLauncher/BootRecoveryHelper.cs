using System.Diagnostics;

namespace KioskLauncher
{
    internal static class BootRecoveryHelper
    {
        /// <summary>
        /// Configures Windows to reboot automatically after a BSOD instead of
        /// stopping on the "Windows failed to start" / WinRE recovery screen.
        /// </summary>
        public static void Enable()
        {
            RunBcdedit("/set {current} bootstatuspolicy ignoreallfailures");
            RunBcdedit("/set {current} recoveryenabled no");
        }

        /// <summary>Restores default Windows recovery boot behaviour.</summary>
        public static void Disable()
        {
            RunBcdedit("/set {current} bootstatuspolicy DisplayAllFailures");
            RunBcdedit("/set {current} recoveryenabled yes");
        }

        private static void RunBcdedit(string args)
        {
            var psi = new ProcessStartInfo("bcdedit.exe", args)
            {
                CreateNoWindow         = true,
                UseShellExecute        = false,
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
