# Kiosk Launcher

A Windows configuration tool that turns any PC into a kiosk display. Configures:

- **Nightly restart** via Windows Task Scheduler (runs as SYSTEM — reliable)
- **Windows auto-login** — no password prompt after restart
- **Chrome/Edge kiosk mode** — auto-launches your URL in full-screen on every login

## Requirements

- Windows 10 or 11
- .NET Framework 4.7.2 (pre-installed on all Windows 10/11 PCs)
- Google Chrome or Microsoft Edge (Edge is the fallback, pre-installed on Win 10/11)

## Usage

1. Run `KioskLauncher.exe` — accept the UAC (Administrator) prompt
2. Enter your kiosk URL
3. Set nightly restart time (default: 02:00)
4. Enter your Windows username and password for auto-login
5. Click **Save & Apply**

The PC will now: restart nightly → auto-log in → open your URL in full-screen kiosk mode.

## Exiting Kiosk Mode

Press `Alt+F4` to close the kiosk browser window.

## Security Note

The auto-login password is stored in the Windows registry under
`HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon`
(same location as the built-in `netplwiz` auto-login feature).
**Only use this on a dedicated, physically secure PC.**

## Build

Requires [.NET SDK](https://dotnet.microsoft.com/download) (for building only — not for running):

```bash
cd KioskLauncher
dotnet build -c Release
# Output: bin/Release/net472/KioskLauncher.exe
```

Copy `KioskLauncher.exe` to any Windows 10/11 PC — no installer needed.
