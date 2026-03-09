using System;
using Microsoft.Win32;

namespace KioskLauncher
{
    internal static class WindowsUpdateHelper
    {
        private const string AuPolicyPath =
            @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU";

        private const string WuServicePath =
            @"SYSTEM\CurrentControlSet\Services\wuauserv";

        public static void Enable()
        {
            using (var key = Registry.LocalMachine.CreateSubKey(AuPolicyPath, writable: true))
            {
                if (key == null)
                    throw new InvalidOperationException(
                        "Cannot open Windows Update policy key. Ensure the app runs as Administrator.");

                key.SetValue("NoAutoUpdate",         0, RegistryValueKind.DWord);
                key.SetValue("AUOptions",            4, RegistryValueKind.DWord);
                key.SetValue("ScheduledInstallDay",  0, RegistryValueKind.DWord);
                key.SetValue("ScheduledInstallTime", 3, RegistryValueKind.DWord);
            }

            using (var key = Registry.LocalMachine.OpenSubKey(WuServicePath, writable: true))
            {
                key?.SetValue("Start", 2, RegistryValueKind.DWord);
            }
        }

        public static void Disable()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(AuPolicyPath, writable: true))
            {
                if (key == null) return;
                key.DeleteValue("NoAutoUpdate",         throwOnMissingValue: false);
                key.DeleteValue("AUOptions",            throwOnMissingValue: false);
                key.DeleteValue("ScheduledInstallDay",  throwOnMissingValue: false);
                key.DeleteValue("ScheduledInstallTime", throwOnMissingValue: false);
            }
        }
    }
}
