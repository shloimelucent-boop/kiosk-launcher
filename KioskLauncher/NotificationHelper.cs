using Microsoft.Win32;

namespace KioskLauncher
{
    internal static class NotificationHelper
    {
        private const string ExplorerPolicyPath =
            @"Software\Policies\Microsoft\Windows\Explorer";

        private const string WerPath =
            @"Software\Microsoft\Windows\Windows Error Reporting";

        public static void Disable()
        {
            using (var key = Registry.CurrentUser.CreateSubKey(ExplorerPolicyPath, writable: true))
            {
                key?.SetValue("DisableNotificationCenter", 1, RegistryValueKind.DWord);
            }

            using (var key = Registry.CurrentUser.CreateSubKey(WerPath, writable: true))
            {
                key?.SetValue("Disabled", 1, RegistryValueKind.DWord);
            }

            // Suppress "Set up your PC" / privacy OOBE dialog that can re-trigger after a BSOD or update.
            using (var key = Registry.LocalMachine.CreateSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\OOBE", writable: true))
            {
                key?.SetValue("SkipUserOOBE",    1, RegistryValueKind.DWord);
                key?.SetValue("SkipMachineOOBE", 1, RegistryValueKind.DWord);
            }
            using (var key = Registry.CurrentUser.CreateSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\OOBE", writable: true))
            {
                key?.SetValue("DisablePrivacyExperience", 1, RegistryValueKind.DWord);
            }
            using (var key = Registry.CurrentUser.CreateSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\UserProfileEngagement", writable: true))
            {
                key?.SetValue("ScoobeSystemSettingEnabled", 0, RegistryValueKind.DWord);
            }
        }

        public static void Enable()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(ExplorerPolicyPath, writable: true))
            {
                key?.DeleteValue("DisableNotificationCenter", throwOnMissingValue: false);
            }

            using (var key = Registry.CurrentUser.OpenSubKey(WerPath, writable: true))
            {
                key?.DeleteValue("Disabled", throwOnMissingValue: false);
            }

            using (var key = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\OOBE", writable: true))
            {
                key?.DeleteValue("SkipUserOOBE",    throwOnMissingValue: false);
                key?.DeleteValue("SkipMachineOOBE", throwOnMissingValue: false);
            }
            using (var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\OOBE", writable: true))
            {
                key?.DeleteValue("DisablePrivacyExperience", throwOnMissingValue: false);
            }
            using (var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\UserProfileEngagement", writable: true))
            {
                key?.DeleteValue("ScoobeSystemSettingEnabled", throwOnMissingValue: false);
            }
        }
    }
}
