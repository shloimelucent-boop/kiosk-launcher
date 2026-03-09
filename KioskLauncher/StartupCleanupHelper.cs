using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace KioskLauncher
{
    /// <summary>
    /// Scans HKCU\Run for known apps that disrupt kiosk mode and can non-destructively
    /// disable them via StartupApproved\Run (the entry stays in Run, only the approval
    /// flag changes — same mechanism Task Manager uses).
    /// </summary>
    internal static class StartupCleanupHelper
    {
        private const string RunKeyPath =
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        private const string ApprovedKeyPath =
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";

        // Apps known to open windows or notifications that disrupt kiosk mode.
        private static readonly string[] KnownDisruptive =
        {
            "OneDrive",
            "GoogleDriveFS",
            "Google Drive",
            "Dropbox",
            "Teams",
            "Slack",
            "Discord",
            "Zoom",
            "Skype",
            "Steam",
            "AdobeAAMUpdater-1.0",
            "Spotify"
        };

        /// <summary>
        /// Returns the value names of disruptive apps currently registered in HKCU\Run.
        /// Only returns entries that are actually present in the Run key.
        /// </summary>
        public static List<string> FindDisruptiveEntries()
        {
            var found = new List<string>();
            using (var key = Registry.CurrentUser.OpenSubKey(RunKeyPath))
            {
                if (key == null) return found;

                foreach (var name in key.GetValueNames())
                {
                    foreach (var known in KnownDisruptive)
                    {
                        if (string.Equals(name, known, StringComparison.OrdinalIgnoreCase))
                        {
                            found.Add(name);
                            break;
                        }
                    }
                }
            }
            return found;
        }

        /// <summary>
        /// Non-destructively disables a startup entry via StartupApproved\Run.
        /// The HKCU\Run value is preserved; only the approval flag is set to disabled.
        /// </summary>
        public static void DisableEntry(string valueName)
        {
            SetApprovedFlag(valueName, disabled: true);
        }

        /// <summary>
        /// Re-enables a previously disabled startup entry via StartupApproved\Run.
        /// </summary>
        public static void EnableEntry(string valueName)
        {
            SetApprovedFlag(valueName, disabled: false);
        }

        // ── Private helpers ──────────────────────────────────────────────────────

        /// <summary>
        /// Writes a 12-byte binary value to StartupApproved\Run, which is the same
        /// mechanism Task Manager uses to enable/disable startup entries:
        ///   Byte  0   : 0x02 = enabled, 0x03 = disabled
        ///   Bytes 1–3 : 0x00 (reserved)
        ///   Bytes 4–11: FILETIME of current UTC time (8 bytes, little-endian)
        /// </summary>
        private static void SetApprovedFlag(string valueName, bool disabled)
        {
            var data = new byte[12];
            data[0] = disabled ? (byte)0x03 : (byte)0x02;
            // Bytes 1–3 remain 0x00.
            byte[] timeBytes = BitConverter.GetBytes(DateTime.UtcNow.ToFileTimeUtc());
            Array.Copy(timeBytes, 0, data, 4, 8);

            using (var key = Registry.CurrentUser.CreateSubKey(ApprovedKeyPath))
            {
                key?.SetValue(valueName, data, RegistryValueKind.Binary);
            }
        }
    }
}
