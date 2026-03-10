# Kiosk Launcher

A Windows configuration tool that turns any PC into a dedicated kiosk display. Walks you through setup step-by-step with a multi-page wizard, then applies all changes in one click.

> ⚠️ **Intended for dedicated kiosk hardware only.** This tool makes system-level changes to the Windows Registry, Task Scheduler, power configuration, and service settings. Test on a non-production machine before deploying.

---

## What it configures

| Feature | Details |
|---|---|
| **Kiosk URL** | The web address displayed full-screen in the browser |
| **Nightly Restart** | Scheduled restart via Task Scheduler (runs as SYSTEM) |
| **Windows Auto-Login** | Logs in automatically after each restart — no password prompt |
| **Browser Kiosk Mode** | Launches Chrome or Edge in full-screen kiosk mode on every login |
| **Windows Auto Updates** | Configures AU Group Policy to install updates daily at 3 AM |
| **Display & Sleep** | Sets monitor and sleep timeouts to Never; activates High Performance power plan; disables USB Selective Suspend and screensaver |
| **Notifications** | Suppresses Windows notification centre, crash/error dialogs, and the Windows setup (OOBE) prompts that can appear after recovery reboots |
| **Boot Recovery** | Configures Windows to reboot automatically after a BSOD instead of stopping on the recovery/error screen (recommended) |
| **Sticky Keys** | Disables the accessibility popup triggered by pressing Shift 5 times |
| **USB Storage** | Blocks USB mass storage devices from mounting (optional) |
| **Startup App Cleanup** | Scans for disruptive startup apps (OneDrive, Google Drive, Teams, etc.) and disables them non-destructively |

---

## Requirements

- Windows 10 or 11
- .NET Framework 4.7.2 (pre-installed on all Windows 10/11 PCs)
- Google Chrome or Microsoft Edge (Edge is the fallback, pre-installed on Win 10/11)
- Administrator privileges (UAC prompt appears on launch)

---

## Usage

1. Copy `KioskLauncher.exe` to the target PC — no installer needed
2. Right-click → **Run as Administrator** (or accept the UAC prompt)
3. Read and accept the disclaimer on the Welcome screen
4. Step through the wizard — each page is explained:
   - **Step 1** — Enter the kiosk URL (`http://` or `https://`)
   - **Step 2** — Optionally schedule a nightly restart
   - **Step 3** — Optionally enable Windows auto-login
   - **Step 4** — Optionally configure browser kiosk mode on startup
   - **Step 5** — Select system hardening options (updates, power, notifications, etc.)
   - **Step 6** — Review and disable any disruptive startup apps found on this PC
5. Review the summary, then click **Finish**

The PC will now: restart nightly → auto-log in → open your URL in full-screen kiosk mode.

### Re-running / Modifying settings

Run the exe again — the Welcome screen will offer two options:
- **Verify Settings** — checks whether all previously configured settings are currently active in Windows and shows a ✓ / ✗ / — report
- **Start / Modify Setup** — re-runs the wizard with your previous settings pre-filled

---

## Exiting Kiosk Mode

Press `Alt+F4` to close the kiosk browser window. The desktop will be accessible until the next restart.

---

## Security Notes

- The auto-login password is stored in the Windows registry under
  `HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon`
  (the same location used by the built-in `netplwiz` auto-login feature).
  **Only use this on a physically secure, dedicated machine.**

- Disruptive startup apps are disabled non-destructively — the registry entry is kept but flagged as disabled via `StartupApproved\Run`, the same mechanism Task Manager uses. They can be re-enabled at any time from Task Manager → Startup Apps.

- USB storage blocking sets the `USBSTOR` service start type to Disabled (value 4). Re-running the wizard with that option unchecked restores it to Manual (value 3).

- Boot recovery uses `bcdedit` to set `bootstatuspolicy ignoreallfailures` and `recoveryenabled no` on the current boot entry. This means Windows will not show the automatic repair / WinRE screen after a crash — the machine reboots silently. Re-running the wizard with that option unchecked restores the defaults (`DisplayAllFailures` / `recoveryenabled yes`).

---

## Build

Requires the [.NET SDK](https://dotnet.microsoft.com/download) (for building only — not needed to run the exe):

```bash
cd KioskLauncher
dotnet build -c Release
# Output: KioskLauncher/bin/Release/net472/KioskLauncher.exe
```

Copy `KioskLauncher.exe` to any Windows 10/11 PC — no installer or runtime install needed.
