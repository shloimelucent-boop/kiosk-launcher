using Microsoft.Win32;

namespace KioskLauncher
{
    internal static class StickyKeysHelper
    {
        private const string AccessBase = @"Control Panel\Accessibility\";

        public static void Disable()
        {
            SetFlag(@"StickyKeys",  "506");
            SetFlag(@"ToggleKeys",  "58");
            SetFlag(@"FilterKeys",  "0");
        }

        public static void Enable()
        {
            // Restore Windows defaults
            SetFlag(@"StickyKeys", "510");
            SetFlag(@"ToggleKeys", "62");
            SetFlag(@"FilterKeys", "62");
        }

        private static void SetFlag(string subKey, string value)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(AccessBase + subKey, writable: true))
            {
                key?.SetValue("Flags", value, RegistryValueKind.String);
            }
        }
    }
}
