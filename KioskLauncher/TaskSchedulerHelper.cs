using System.Diagnostics;
using System.Text;

namespace KioskLauncher
{
    internal static class TaskSchedulerHelper
    {
        private const string TaskName = "KioskLauncher_Restart";

        /// <summary>Creates (or replaces) the nightly restart scheduled task.</summary>
        public static (bool Success, string Message) Enable(string time24)
        {
            // Remove any existing version first (ignore failure)
            Run($"/delete /tn \"{TaskName}\" /f");

            var args = $"/create /tn \"{TaskName}\" " +
                       $"/tr \"shutdown /r /t 60 /c \\\"Nightly kiosk restart\\\"\" " +
                       $"/sc daily /st {time24} /ru SYSTEM /f";

            var (exit, _, err) = Run(args);
            return exit == 0
                ? (true, $"Restart scheduled daily at {time24}.")
                : (false, $"schtasks error: {err.Trim()}");
        }

        /// <summary>Removes the nightly restart scheduled task.</summary>
        public static (bool Success, string Message) Disable()
        {
            var (exit, _, err) = Run($"/delete /tn \"{TaskName}\" /f");
            bool ok = exit == 0
                      || err.Contains("does not exist")
                      || err.Contains("cannot find")
                      || err.Contains("ERROR: The system cannot find");
            return ok
                ? (true, "Nightly restart task removed.")
                : (false, $"schtasks error: {err.Trim()}");
        }

        private static (int Exit, string Out, string Err) Run(string args)
        {
            var psi = new ProcessStartInfo("schtasks.exe", args)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            using (var p = Process.Start(psi)!)
            {
                var output = p.StandardOutput.ReadToEnd();
                var error = p.StandardError.ReadToEnd();
                p.WaitForExit();
                return (p.ExitCode, output, error);
            }
        }
    }
}
