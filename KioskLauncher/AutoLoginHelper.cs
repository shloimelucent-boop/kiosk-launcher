using System;
using Microsoft.Win32;

namespace KioskLauncher
{
    internal static class AutoLoginHelper
    {
        private const string WinlogonPath =
            @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon";

        /// <summary>
        /// Enables Windows auto-login by writing credentials to HKLM\Winlogon.
        /// Requires the app to be running as Administrator.
        /// </summary>
        public static void Enable(string username, string password)
        {
            using (var key = Registry.LocalMachine.OpenSubKey(WinlogonPath, writable: true))
            {
                if (key == null)
                    throw new InvalidOperationException(
                        "Cannot open Winlogon registry key. Make sure the app is running as Administrator.");

                key.SetValue("AutoAdminLogon", "1");
                key.SetValue("DefaultUserName", username);
                key.SetValue("DefaultPassword", password);
                key.SetValue("DefaultDomainName", Environment.MachineName);
            }
        }

        /// <summary>Disables Windows auto-login and clears the stored password.</summary>
        public static void Disable()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(WinlogonPath, writable: true))
            {
                if (key == null) return;
                key.SetValue("AutoAdminLogon", "0");
                key.DeleteValue("DefaultPassword", throwOnMissingValue: false);
            }
        }

        /// <summary>Reads current auto-login credentials from the registry.</summary>
        public static (string Username, string Password) GetCurrent()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(WinlogonPath))
            {
                if (key == null) return (string.Empty, string.Empty);
                var user = key.GetValue("DefaultUserName") as string ?? string.Empty;
                var pass = key.GetValue("DefaultPassword") as string ?? string.Empty;
                return (user, pass);
            }
        }
    }
}
