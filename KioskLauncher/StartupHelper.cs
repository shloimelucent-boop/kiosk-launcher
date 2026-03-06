using Microsoft.Win32;

namespace KioskLauncher
{
    internal static class StartupHelper
    {
        private const string RunKeyPath =
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        private const string ValueName = "KioskLauncherBrowser";

        /// <summary>
        /// Adds a registry entry under HKCU\Run so the browser opens in kiosk
        /// mode each time the current user logs in.
        /// </summary>
        public static void Enable(string browserPath, string url)
        {
            // Standard command-line format for HKCU\Run:
            //   "C:\path\to\browser.exe" --kiosk --no-first-run --new-window "url"
            var command = $"\"{browserPath}\" --kiosk --no-first-run --new-window \"{url}\"";

            using (var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true))
            {
                key?.SetValue(ValueName, command);
            }
        }

        /// <summary>Removes the kiosk browser auto-launch from HKCU\Run.</summary>
        public static void Disable()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true))
            {
                key?.DeleteValue(ValueName, throwOnMissingValue: false);
            }
        }
    }
}
