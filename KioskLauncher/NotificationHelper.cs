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
        }
    }
}
