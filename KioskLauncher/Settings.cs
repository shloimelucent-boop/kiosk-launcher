using System;
using System.IO;

namespace KioskLauncher
{
    internal class Settings
    {
        private static readonly string ConfigDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KioskLauncher");

        private static readonly string ConfigPath = Path.Combine(ConfigDir, "config.ini");

        public string Url { get; set; } = string.Empty;
        public string RestartTime { get; set; } = "02:00";
        public bool RestartEnabled { get; set; }
        public bool AutoLaunchEnabled { get; set; }
        public string BrowserPath { get; set; } = string.Empty;
        public bool AutoLoginEnabled { get; set; }
        public string AutoLoginUser { get; set; } = string.Empty;
        public bool AutoUpdatesEnabled    { get; set; }
        public bool PowerSettingsEnabled  { get; set; }
        public bool NotificationsDisabled { get; set; }
        public bool StickyKeysDisabled    { get; set; }
        public bool UsbStorageDisabled    { get; set; }
        public bool BootRecoveryEnabled   { get; set; }

        public static Settings Load()
        {
            var s = new Settings();
            if (!File.Exists(ConfigPath)) return s;

            foreach (var line in File.ReadAllLines(ConfigPath))
            {
                var sep = line.IndexOf('=');
                if (sep < 0) continue;
                var key = line.Substring(0, sep).Trim();
                var val = line.Substring(sep + 1).Trim();

                switch (key)
                {
                    case "Url":               s.Url = val; break;
                    case "RestartTime":       s.RestartTime = val; break;
                    case "RestartEnabled":    s.RestartEnabled = val == "1"; break;
                    case "AutoLaunchEnabled": s.AutoLaunchEnabled = val == "1"; break;
                    case "BrowserPath":       s.BrowserPath = val; break;
                    case "AutoLoginEnabled":    s.AutoLoginEnabled    = val == "1"; break;
                    case "AutoLoginUser":       s.AutoLoginUser       = val; break;
                    case "AutoUpdatesEnabled":    s.AutoUpdatesEnabled    = val == "1"; break;
                    case "PowerSettingsEnabled":  s.PowerSettingsEnabled  = val == "1"; break;
                    case "NotificationsDisabled": s.NotificationsDisabled = val == "1"; break;
                    case "StickyKeysDisabled":    s.StickyKeysDisabled    = val == "1"; break;
                    case "UsbStorageDisabled":    s.UsbStorageDisabled    = val == "1"; break;
                    case "BootRecoveryEnabled":   s.BootRecoveryEnabled   = val == "1"; break;
                }
            }
            return s;
        }

        public void Save()
        {
            Directory.CreateDirectory(ConfigDir);
            File.WriteAllText(ConfigPath,
                "Url=" + Url + "\n" +
                "RestartTime=" + RestartTime + "\n" +
                "RestartEnabled=" + (RestartEnabled ? "1" : "0") + "\n" +
                "AutoLaunchEnabled=" + (AutoLaunchEnabled ? "1" : "0") + "\n" +
                "BrowserPath=" + BrowserPath + "\n" +
                "AutoLoginEnabled=" + (AutoLoginEnabled ? "1" : "0") + "\n" +
                "AutoLoginUser=" + AutoLoginUser + "\n" +
                "AutoUpdatesEnabled=" + (AutoUpdatesEnabled ? "1" : "0") + "\n" +
                "PowerSettingsEnabled=" + (PowerSettingsEnabled ? "1" : "0") + "\n" +
                "NotificationsDisabled=" + (NotificationsDisabled ? "1" : "0") + "\n" +
                "StickyKeysDisabled=" + (StickyKeysDisabled ? "1" : "0") + "\n" +
                "UsbStorageDisabled=" + (UsbStorageDisabled ? "1" : "0") + "\n" +
                "BootRecoveryEnabled=" + (BootRecoveryEnabled ? "1" : "0") + "\n"
            );
        }
    }
}
