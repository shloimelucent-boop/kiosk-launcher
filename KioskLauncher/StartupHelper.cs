using Microsoft.Win32;

namespace KioskLauncher
{
    internal static class StartupHelper
    {
        private const string ValueName  = "KioskLauncherBrowser";
        private const string RunKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        /// <summary>
        /// Adds a HKCU Run registry entry that launches the browser in kiosk mode
        /// on every Windows login.
        /// </summary>
        public static void Enable(string browserPath, string url)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true))
            {
                key?.SetValue(
                    ValueName,
                    $"\"{browserPath}\" --kiosk --no-first-run --no-restore-last-session " +
                    $"--disable-session-crashed-bubble --new-window \"{url}\"",
                    RegistryValueKind.String);
            }
        }

        /// <summary>
        /// Removes the kiosk browser Run entry.
        /// </summary>
        public static void Disable()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true))
                key?.DeleteValue(ValueName, throwOnMissingValue: false);
        }
    }
}
