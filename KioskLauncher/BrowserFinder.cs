using System;
using System.IO;
using Microsoft.Win32;

namespace KioskLauncher
{
    internal static class BrowserFinder
    {
        /// <summary>
        /// Locates a Chromium-based browser (Chrome preferred, Edge as fallback).
        /// Both support identical --kiosk flags.
        /// Returns the full path to the executable, or null if neither is found.
        /// </summary>
        public static string? Find()
        {
            // ── Google Chrome ──────────────────────────────────────────────
            var chromePaths = new[]
            {
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    @"Google\Chrome\Application\chrome.exe"),
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                    @"Google\Chrome\Application\chrome.exe"),
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    @"Google\Chrome\Application\chrome.exe"),
            };

            foreach (var path in chromePaths)
                if (File.Exists(path)) return path;

            // Chrome via registry
            using (var regKey = Registry.LocalMachine.OpenSubKey(
                       @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe"))
            {
                var regPath = regKey?.GetValue(string.Empty) as string;
                if (regPath != null && File.Exists(regPath)) return regPath;
            }

            // ── Microsoft Edge (pre-installed on Windows 10/11) ────────────
            var edgePaths = new[]
            {
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                    @"Microsoft\Edge\Application\msedge.exe"),
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    @"Microsoft\Edge\Application\msedge.exe"),
            };

            foreach (var path in edgePaths)
                if (File.Exists(path)) return path;

            return null;
        }
    }
}
