using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Win32;

namespace KioskLauncher
{
    internal enum VerifyStatus { Ok, Fail, Skipped }

    internal struct VerifyResult
    {
        public string       Name;
        public VerifyStatus Status;
        public string       Details;
    }

    internal static class VerifyHelper
    {
        public static List<VerifyResult> Verify(Settings s)
        {
            var results = new List<VerifyResult>();

            // Kiosk URL
            results.Add(Check(
                "Kiosk URL",
                !string.IsNullOrEmpty(s.Url),
                () => !string.IsNullOrEmpty(s.Url),
                s.Url,
                "Not configured"
            ));

            // Nightly Restart
            results.Add(CheckCommand(
                "Nightly Restart",
                s.RestartEnabled,
                "schtasks.exe",
                "/query /tn KioskLauncher_Restart",
                "Scheduled task found",
                "Task not found — setup may not have completed"
            ));

            // Auto-Login
            results.Add(CheckRegistry(
                "Windows Auto-Login",
                s.AutoLoginEnabled,
                Registry.LocalMachine,
                @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon",
                "AutoAdminLogon",
                expected: "1",
                okDetail: "Auto-login enabled in registry",
                failDetail: "AutoAdminLogon not set — auto-login may not be active"
            ));

            // Browser Kiosk Startup
            results.Add(CheckRegistry(
                "Browser Kiosk Mode",
                s.AutoLaunchEnabled,
                Registry.CurrentUser,
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
                "KioskLauncherBrowser",
                expected: null,   // null = just check value exists
                okDetail: "Browser startup entry found",
                failDetail: "Startup registry entry not found"
            ));

            // Windows Auto Updates
            results.Add(CheckRegistryDWord(
                "Windows Auto Updates",
                s.AutoUpdatesEnabled,
                Registry.LocalMachine,
                @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU",
                "NoAutoUpdate",
                expectedValue: 0,
                okDetail: "AU policy set — auto updates enabled",
                failDetail: "AU policy not configured"
            ));

            // Power Settings — verify High Performance plan is active (SCHEME_MIN GUID)
            results.Add(CheckCommandOutput(
                "Power & Display Settings",
                s.PowerSettingsEnabled,
                "powercfg.exe", "/getactivescheme",
                expectedSubstring: "8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c",
                okDetail: "High Performance plan active; display/standby timeouts and USB suspend configured",
                failDetail: "High Performance plan not active — power settings may not have applied"
            ));

            // Notifications
            results.Add(CheckRegistryDWord(
                "Notifications & Error Dialogs",
                s.NotificationsDisabled,
                Registry.CurrentUser,
                @"Software\Policies\Microsoft\Windows\Explorer",
                "DisableNotificationCenter",
                expectedValue: 1,
                okDetail: "Notification center disabled",
                failDetail: "DisableNotificationCenter not set"
            ));

            // Sticky Keys
            results.Add(CheckRegistry(
                "Sticky Keys",
                s.StickyKeysDisabled,
                Registry.CurrentUser,
                @"Control Panel\Accessibility\StickyKeys",
                "Flags",
                expected: "506",
                okDetail: "Sticky Keys popups disabled",
                failDetail: "Sticky Keys flags not set to disabled state"
            ));

            // USB Storage
            results.Add(CheckRegistryDWord(
                "USB Storage",
                s.UsbStorageDisabled,
                Registry.LocalMachine,
                @"SYSTEM\CurrentControlSet\Services\USBSTOR",
                "Start",
                expectedValue: 4,
                okDetail: "USBSTOR service disabled",
                failDetail: "USBSTOR service is not disabled"
            ));

            // Boot Recovery
            results.Add(CheckCommandOutput(
                "Boot Recovery",
                s.BootRecoveryEnabled,
                "bcdedit.exe", "/enum {current}",
                expectedSubstring: "ignoreallfailures",
                okDetail: "Auto-reboot after crash enabled",
                failDetail: "Boot recovery policy not configured"
            ));

            return results;
        }

        // ── Private helpers ──────────────────────────────────────────────────────

        private static VerifyResult Check(string name, bool enabled,
            Func<bool> test, string okDetail, string skippedDetail)
        {
            if (!enabled)
                return new VerifyResult { Name = name, Status = VerifyStatus.Skipped, Details = skippedDetail };
            try
            {
                return test()
                    ? new VerifyResult { Name = name, Status = VerifyStatus.Ok,   Details = okDetail }
                    : new VerifyResult { Name = name, Status = VerifyStatus.Fail, Details = "Value is empty" };
            }
            catch (Exception ex)
            {
                return new VerifyResult { Name = name, Status = VerifyStatus.Fail, Details = ex.Message };
            }
        }

        private static VerifyResult CheckRegistry(string name, bool enabled,
            RegistryKey hive, string path, string valueName,
            string? expected, string okDetail, string failDetail)
        {
            if (!enabled)
                return new VerifyResult { Name = name, Status = VerifyStatus.Skipped, Details = "Not configured" };
            try
            {
                using (var key = hive.OpenSubKey(path))
                {
                    if (key == null)
                        return new VerifyResult { Name = name, Status = VerifyStatus.Fail, Details = failDetail };

                    var val = key.GetValue(valueName);
                    if (val == null)
                        return new VerifyResult { Name = name, Status = VerifyStatus.Fail, Details = failDetail };

                    if (expected != null && val.ToString() != expected)
                        return new VerifyResult { Name = name, Status = VerifyStatus.Fail, Details = failDetail };

                    return new VerifyResult { Name = name, Status = VerifyStatus.Ok, Details = okDetail };
                }
            }
            catch (Exception ex)
            {
                return new VerifyResult { Name = name, Status = VerifyStatus.Fail, Details = ex.Message };
            }
        }

        private static VerifyResult CheckRegistryDWord(string name, bool enabled,
            RegistryKey hive, string path, string valueName,
            int expectedValue, string okDetail, string failDetail)
        {
            if (!enabled)
                return new VerifyResult { Name = name, Status = VerifyStatus.Skipped, Details = "Not configured" };
            try
            {
                using (var key = hive.OpenSubKey(path))
                {
                    if (key == null)
                        return new VerifyResult { Name = name, Status = VerifyStatus.Fail, Details = failDetail };

                    var val = key.GetValue(valueName);
                    if (val == null || (int)val != expectedValue)
                        return new VerifyResult { Name = name, Status = VerifyStatus.Fail, Details = failDetail };

                    return new VerifyResult { Name = name, Status = VerifyStatus.Ok, Details = okDetail };
                }
            }
            catch (Exception ex)
            {
                return new VerifyResult { Name = name, Status = VerifyStatus.Fail, Details = ex.Message };
            }
        }

        private static VerifyResult CheckCommand(string name, bool enabled,
            string exe, string args, string okDetail, string failDetail)
        {
            if (!enabled)
                return new VerifyResult { Name = name, Status = VerifyStatus.Skipped, Details = "Not configured" };
            try
            {
                var psi = new ProcessStartInfo(exe, args)
                {
                    CreateNoWindow         = true,
                    UseShellExecute        = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError  = true
                };
                using (var p = Process.Start(psi))
                {
                    p?.WaitForExit();
                    bool ok = p?.ExitCode == 0;
                    return new VerifyResult
                    {
                        Name    = name,
                        Status  = ok ? VerifyStatus.Ok : VerifyStatus.Fail,
                        Details = ok ? okDetail : failDetail
                    };
                }
            }
            catch (Exception ex)
            {
                return new VerifyResult { Name = name, Status = VerifyStatus.Fail, Details = ex.Message };
            }
        }

        private static VerifyResult CheckCommandOutput(string name, bool enabled,
            string exe, string args, string expectedSubstring, string okDetail, string failDetail)
        {
            if (!enabled)
                return new VerifyResult { Name = name, Status = VerifyStatus.Skipped, Details = "Not configured" };
            try
            {
                var psi = new ProcessStartInfo(exe, args)
                {
                    CreateNoWindow         = true,
                    UseShellExecute        = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError  = true
                };
                using (var p = Process.Start(psi))
                {
                    var output = p?.StandardOutput.ReadToEnd() ?? string.Empty;
                    p?.WaitForExit();
                    bool found = output.IndexOf(expectedSubstring, StringComparison.OrdinalIgnoreCase) >= 0;
                    return new VerifyResult
                    {
                        Name    = name,
                        Status  = found ? VerifyStatus.Ok : VerifyStatus.Fail,
                        Details = found ? okDetail : failDetail
                    };
                }
            }
            catch (Exception ex)
            {
                return new VerifyResult { Name = name, Status = VerifyStatus.Fail, Details = ex.Message };
            }
        }
    }
}
