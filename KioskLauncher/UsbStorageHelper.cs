using Microsoft.Win32;

namespace KioskLauncher
{
    internal static class UsbStorageHelper
    {
        private const string UsbStorPath =
            @"SYSTEM\CurrentControlSet\Services\USBSTOR";

        public static void Disable()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(UsbStorPath, writable: true))
            {
                key?.SetValue("Start", 4, RegistryValueKind.DWord);
            }
        }

        public static void Enable()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(UsbStorPath, writable: true))
            {
                key?.SetValue("Start", 3, RegistryValueKind.DWord);
            }
        }
    }
}
