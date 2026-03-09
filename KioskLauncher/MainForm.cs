using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace KioskLauncher
{
    public class MainForm : Form
    {
        // ── Navigation state ────────────────────────────────────────────────────
        private Panel[] _panels  = null!;
        private int     _current = 0;

        // ── Header ──────────────────────────────────────────────────────────────
        private Label lblStepTitle   = null!;
        private Label lblStepCounter = null!;

        // ── Nav bar ─────────────────────────────────────────────────────────────
        private Panel  pnlNavBar = null!;
        private Button btnBack   = null!;
        private Button btnNext   = null!;

        // ── Panel 0: Welcome ────────────────────────────────────────────────────
        private CheckBox chkAgree      = null!;
        private Button   btnVerify     = null!;
        private Button   btnStartSetup = null!;

        // ── Panel 1: Kiosk URL ──────────────────────────────────────────────────
        private TextBox txtUrl = null!;

        // ── Panel 2: Nightly Restart ────────────────────────────────────────────
        private CheckBox       chkRestart     = null!;
        private DateTimePicker dtpRestartTime = null!;

        // ── Panel 3: Windows Auto-Login ─────────────────────────────────────────
        private CheckBox chkAutoLogin = null!;
        private TextBox  txtUsername  = null!;
        private TextBox  txtPassword  = null!;

        // ── Panel 4: Browser Kiosk Mode ─────────────────────────────────────────
        private CheckBox chkAutoLaunch      = null!;
        private TextBox  txtBrowserPath     = null!;
        private Button   btnBrowse          = null!;
        private Label    lblBrowserDetected = null!;

        // ── Panel 5: System Configuration ───────────────────────────────────────
        private CheckBox chkAutoUpdates  = null!;
        private CheckBox chkPowerSettings = null!;
        private CheckBox chkNotifications = null!;
        private CheckBox chkStickyKeys   = null!;
        private CheckBox chkUsbStorage   = null!;

        // ── Panel 6: Startup Apps ────────────────────────────────────────────────
        private Panel        pnlStartupCheckboxes = null!;
        private List<string> _startupToDisable    = new List<string>();

        // ── Panel 7: Summary ────────────────────────────────────────────────────
        // lblSummaryValues[i] = value label for row i (0-based)
        private Label[] lblSummaryValues = null!;

        // ── Panel 8: Verify ─────────────────────────────────────────────────────
        private Panel pnlVerifyResults = null!;

        // ── Layout constants ─────────────────────────────────────────────────────
        private const int ContentLeft   = 20;
        private const int ContentTop    = 70;
        private const int ContentWidth  = 500;
        private const int ContentHeight = 290;

        // ── Constructor ──────────────────────────────────────────────────────────
        public MainForm()
        {
            BuildUI();
            LoadSettings();
            ShowPanel(0);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // UI Construction
        // ═══════════════════════════════════════════════════════════════════════

        private void BuildUI()
        {
            SuspendLayout();

            Text            = "Kiosk Launcher";
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox     = false;
            StartPosition   = FormStartPosition.CenterScreen;
            Font            = new Font("Segoe UI", 9f);
            BackColor       = Color.FromArgb(245, 245, 248);
            ClientSize      = new Size(540, 440);

            // ── Header strip ──────────────────────────────────────────────────
            var pnlHeader = new Panel
            {
                Left = 0, Top = 0, Width = 540, Height = 65,
                BackColor = Color.White
            };

            lblStepTitle = new Label
            {
                Left = ContentLeft, Top = 18, Width = 370,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30)
            };

            lblStepCounter = new Label
            {
                Left = 400, Top = 22, Width = 120,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.Gray
            };

            pnlHeader.Controls.AddRange(new Control[] { lblStepTitle, lblStepCounter });

            var sep1 = new Panel
            {
                Left = 0, Top = 65, Width = 540, Height = 1,
                BackColor = Color.FromArgb(200, 200, 210)
            };

            // ── Content panels ────────────────────────────────────────────────
            var p0 = BuildPanel0_Welcome();
            var p1 = BuildPanel1_Url();
            var p2 = BuildPanel2_Restart();
            var p3 = BuildPanel3_AutoLogin();
            var p4 = BuildPanel4_Browser();
            var p5 = BuildPanel5_SysConfig();
            var p6 = BuildPanel6_StartupApps();
            var p7 = BuildPanel7_Summary();
            var p8 = BuildPanel8_Verify();

            _panels = new Panel[] { p0, p1, p2, p3, p4, p5, p6, p7, p8 };

            // ── Nav bar ───────────────────────────────────────────────────────
            var sep2 = new Panel
            {
                Left = 0, Top = 400, Width = 540, Height = 1,
                BackColor = Color.FromArgb(200, 200, 210)
            };

            pnlNavBar = new Panel
            {
                Left = 0, Top = 401, Width = 540, Height = 39,
                BackColor = Color.FromArgb(245, 245, 248)
            };

            btnBack = new Button
            {
                Left = ContentLeft, Top = 5, Width = 90, Height = 29,
                Text = "< Back",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 220, 225),
                ForeColor = Color.FromArgb(50, 50, 50)
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += (s, e) => ShowPanel(_current - 1);

            btnNext = new Button
            {
                Left = 430, Top = 5, Width = 90, Height = 29,
                Text = "Next >",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 120, 190),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.Click += BtnNext_Click;

            pnlNavBar.Controls.AddRange(new Control[] { btnBack, btnNext });

            Controls.AddRange(new Control[]
            {
                pnlHeader, sep1,
                p0, p1, p2, p3, p4, p5, p6, p7, p8,
                sep2, pnlNavBar
            });

            ResumeLayout(false);
        }

        // ── Panel 0: Welcome ─────────────────────────────────────────────────────
        private Panel BuildPanel0_Welcome()
        {
            var panel = MakeContentPanel();

            var lblTitle = new Label
            {
                Left = 0, Top = 0, Width = ContentWidth,
                Text = "Kiosk Launcher Setup",
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30)
            };

            var rtfDisclaimer = new RichTextBox
            {
                Left = 0, Top = 28, Width = ContentWidth, Height = 138,
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                BackColor = Color.FromArgb(250, 250, 252),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 8f),
                Text =
                    "DISCLAIMER — PLEASE READ BEFORE CONTINUING\r\n\r\n" +
                    "This software (\"Kiosk Launcher\") is provided \"AS IS\", without warranty of any kind, " +
                    "express or implied. The creator(s) of this tool make no representations or warranties " +
                    "regarding the accuracy, completeness, or suitability of this software for any purpose.\r\n\r\n" +
                    "By using this software you acknowledge and agree that:\r\n\r\n" +
                    "  •  This tool makes system-level changes to Windows settings, including modifying the " +
                    "Windows Registry, Task Scheduler, power configuration, and service start-up settings.\r\n\r\n" +
                    "  •  Such changes may affect system stability, security, and performance. Improper use " +
                    "on a non-dedicated or production system may result in loss of functionality, data loss, " +
                    "or other unintended consequences.\r\n\r\n" +
                    "  •  You are solely responsible for understanding the effect of each setting before " +
                    "applying it. It is strongly recommended that you test this tool on a non-production " +
                    "machine before deployment.\r\n\r\n" +
                    "  •  Administrator privileges are required and requested by this application. You are " +
                    "responsible for ensuring that administrator access is appropriate on this machine.\r\n\r\n" +
                    "  •  The creator(s) of this software shall not be held liable for any direct, indirect, " +
                    "incidental, special, or consequential damages arising out of or in connection with the " +
                    "use or inability to use this software.\r\n\r\n" +
                    "  •  This tool is intended for use on dedicated kiosk hardware only. Use on shared, " +
                    "personal, or corporate-managed systems is at the user's own risk.\r\n\r\n" +
                    "By clicking \"Accept & Start Setup\" or \"Accept & Verify Settings\" you confirm that " +
                    "you have read, understood, and agree to these terms."
            };

            chkAgree = new CheckBox
            {
                Left = 0, Top = 176, AutoSize = true,
                Text = "I have read and understand the above disclaimer",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
            };

            btnVerify = new Button
            {
                Left = 0, Top = 210, Width = 190, Height = 29,
                Text = "Accept && Verify Settings",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 220, 225),
                ForeColor = Color.FromArgb(50, 50, 50),
                Enabled = false,
                Visible = false
            };
            btnVerify.FlatAppearance.BorderSize = 0;
            btnVerify.Click += (s, e) => ShowPanel(8);

            btnStartSetup = new Button
            {
                Left = 0, Top = 210, Width = 180, Height = 29,
                Text = "Accept && Start Setup",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 120, 190),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Enabled = false
            };
            btnStartSetup.FlatAppearance.BorderSize = 0;
            btnStartSetup.Click += (s, e) => ShowPanel(1);

            chkAgree.CheckedChanged += (s, e) =>
            {
                bool agreed = chkAgree.Checked;
                btnStartSetup.Enabled = agreed;
                btnVerify.Enabled     = agreed;
            };

            panel.Controls.AddRange(new Control[]
                { lblTitle, rtfDisclaimer, chkAgree, btnVerify, btnStartSetup });
            return panel;
        }

        // ── Panel 1: Kiosk URL ────────────────────────────────────────────────────
        private Panel BuildPanel1_Url()
        {
            var panel = MakeContentPanel();

            AddStepHeading(panel, "Kiosk URL",
                "Enter the web address that will be displayed in the kiosk browser.");

            var lblUrl = new Label
                { Left = 0, Top = 62, AutoSize = true, Text = "URL:" };
            txtUrl = new TextBox
                { Left = 0, Top = 80, Width = ContentWidth };

            var lblHint = new Label
            {
                Left = 0, Top = 106, Width = ContentWidth,
                Text = "Must start with http:// or https://",
                ForeColor = Color.FromArgb(180, 100, 0),
                Font = new Font("Segoe UI", 8f)
            };

            panel.Controls.AddRange(new Control[] { lblUrl, txtUrl, lblHint });
            return panel;
        }

        // ── Panel 2: Nightly Restart ──────────────────────────────────────────────
        private Panel BuildPanel2_Restart()
        {
            var panel = MakeContentPanel();

            AddStepHeading(panel, "Nightly Restart",
                "Schedule an automatic PC restart each night to keep the kiosk fresh.\n" +
                "The PC will warn users 60 seconds before restarting.");

            chkRestart = new CheckBox
            {
                Left = 0, Top = 70, AutoSize = true,
                Text = "Restart PC every night at:"
            };
            dtpRestartTime = new DateTimePicker
            {
                Left = 210, Top = 68, Width = 80,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "HH:mm",
                ShowUpDown = true,
                Enabled = false,
                Value = new DateTime(2000, 1, 1, 2, 0, 0)
            };
            chkRestart.CheckedChanged += (s, e) =>
                dtpRestartTime.Enabled = chkRestart.Checked;

            panel.Controls.AddRange(new Control[] { chkRestart, dtpRestartTime });
            return panel;
        }

        // ── Panel 3: Windows Auto-Login ───────────────────────────────────────────
        private Panel BuildPanel3_AutoLogin()
        {
            var panel = MakeContentPanel();

            AddStepHeading(panel, "Windows Auto-Login",
                "Automatically log in to Windows on startup without a password prompt.");

            var lblWarn = new Label
            {
                Left = 0, Top = 50, Width = ContentWidth, Height = 30,
                Text = "⚠  Password is stored in the registry (same as netplwiz). " +
                       "Only use on a dedicated, physically secure PC.",
                ForeColor = Color.DarkOrange,
                Font = new Font("Segoe UI", 8f)
            };

            chkAutoLogin = new CheckBox
            {
                Left = 0, Top = 84, AutoSize = true,
                Text = "Enable Windows auto-login on startup"
            };

            var lblUser = new Label { Left = 0, Top = 112, AutoSize = true, Text = "Username:" };
            txtUsername = new TextBox { Left = 82, Top = 109, Width = 200, Enabled = false };
            var lblPass = new Label { Left = 0, Top = 140, AutoSize = true, Text = "Password:" };
            txtPassword = new TextBox
            {
                Left = 82, Top = 137, Width = 200,
                UseSystemPasswordChar = true, Enabled = false
            };

            chkAutoLogin.CheckedChanged += (s, e) =>
            {
                txtUsername.Enabled = chkAutoLogin.Checked;
                txtPassword.Enabled = chkAutoLogin.Checked;
            };

            panel.Controls.AddRange(new Control[]
                { lblWarn, chkAutoLogin, lblUser, txtUsername, lblPass, txtPassword });
            return panel;
        }

        // ── Panel 4: Browser Kiosk Mode ───────────────────────────────────────────
        private Panel BuildPanel4_Browser()
        {
            var panel = MakeContentPanel();

            AddStepHeading(panel, "Browser Kiosk Mode",
                "Launch the kiosk URL in full-screen browser kiosk mode on every Windows startup.");

            chkAutoLaunch = new CheckBox
            {
                Left = 0, Top = 62, AutoSize = true,
                Text = "Open URL in kiosk mode on every startup"
            };

            var lblBrow = new Label { Left = 0, Top = 92, AutoSize = true, Text = "Browser:" };
            txtBrowserPath = new TextBox { Left = 68, Top = 89, Width = 314, Enabled = false };
            btnBrowse = new Button
            {
                Left = 388, Top = 88, Width = 80, Height = 24,
                Text = "Browse...", Enabled = false,
                FlatStyle = FlatStyle.Flat
            };
            btnBrowse.FlatAppearance.BorderSize = 1;

            lblBrowserDetected = new Label
            {
                Left = 0, Top = 118, Width = ContentWidth, Height = 16,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 7.5f)
            };

            btnBrowse.Click += BtnBrowse_Click;
            chkAutoLaunch.CheckedChanged += (s, e) =>
            {
                txtBrowserPath.Enabled = chkAutoLaunch.Checked;
                btnBrowse.Enabled      = chkAutoLaunch.Checked;
            };

            panel.Controls.AddRange(new Control[]
                { chkAutoLaunch, lblBrow, txtBrowserPath, btnBrowse, lblBrowserDetected });
            return panel;
        }

        // ── Panel 5: System Configuration ─────────────────────────────────────────
        private Panel BuildPanel5_SysConfig()
        {
            var panel = MakeContentPanel();

            AddStepHeading(panel, "System Configuration",
                "Select the Windows settings to apply for an optimal kiosk experience.");

            int y = 52;

            // Recommended section
            var lblRec = new Label
            {
                Left = 0, Top = y, Width = ContentWidth, AutoSize = true,
                Text = "Recommended",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor = Color.Gray
            };
            y += 18;

            y = AddSysConfigItem(panel, ref y,
                out chkAutoUpdates,
                "Enable automatic Windows Updates",
                "Downloads and installs updates daily at 3 AM via Group Policy.");

            y = AddSysConfigItem(panel, ref y,
                out chkPowerSettings,
                "Prevent display sleep and screensaver",
                "Sets monitor and sleep timeouts to Never; disables screensaver.");

            y = AddSysConfigItem(panel, ref y,
                out chkNotifications,
                "Disable notifications and error dialogs",
                "Suppresses Windows notification popups and crash dialog boxes.");

            // Separator
            var sep = new Panel
            {
                Left = 0, Top = y, Width = ContentWidth, Height = 1,
                BackColor = Color.FromArgb(200, 200, 210)
            };
            y += 8;

            // Optional section
            var lblOpt = new Label
            {
                Left = 0, Top = y, Width = ContentWidth, AutoSize = true,
                Text = "Optional",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor = Color.Gray
            };
            y += 18;

            y = AddSysConfigItem(panel, ref y,
                out chkStickyKeys,
                "Disable Sticky Keys accessibility popups",
                "Prevents the dialog that appears when Shift is pressed 5 times.");

            y = AddSysConfigItem(panel, ref y,
                out chkUsbStorage,
                "Block USB storage devices",
                "Prevents USB drives from mounting. Disable if USB peripherals are needed.");

            panel.Controls.AddRange(new Control[] { lblRec, sep, lblOpt });
            return panel;
        }

        private int AddSysConfigItem(Panel parent, ref int y,
            out CheckBox chk, string checkText, string subText)
        {
            chk = new CheckBox
            {
                Left = 0, Top = y, AutoSize = true,
                Text = checkText
            };
            var lbl = new Label
            {
                Left = 18, Top = y + 18, Width = ContentWidth - 18, Height = 16,
                Text = subText,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 7.5f)
            };
            parent.Controls.AddRange(new Control[] { chk, lbl });
            y += 42;
            return y;
        }

        // ── Panel 6: Startup Apps ─────────────────────────────────────────────────
        private Panel BuildPanel6_StartupApps()
        {
            var panel = MakeContentPanel();

            AddStepHeading(panel, "Startup Apps",
                "These apps were found running at startup and may disrupt kiosk mode.\n" +
                "Uncheck any you wish to keep enabled.");

            // Sub-panel rebuilt each time this step is shown.
            pnlStartupCheckboxes = new Panel
            {
                Left = 0, Top = 60, Width = ContentWidth, Height = 220,
                BackColor = Color.Transparent
            };

            panel.Controls.Add(pnlStartupCheckboxes);
            return panel;
        }

        /// <summary>
        /// Scans for disruptive startup entries and builds checkboxes inside
        /// pnlStartupCheckboxes.  Called every time Panel 6 becomes visible.
        /// </summary>
        private void PopulateStartupApps()
        {
            pnlStartupCheckboxes.Controls.Clear();

            var found = StartupCleanupHelper.FindDisruptiveEntries();

            if (found.Count == 0)
            {
                var lbl = new Label
                {
                    Left = 0, Top = 0, Width = ContentWidth, AutoSize = true,
                    Text = "✓ No disruptive startup apps detected.",
                    ForeColor = Color.DarkGreen,
                    Font = new Font("Segoe UI", 9f)
                };
                pnlStartupCheckboxes.Controls.Add(lbl);
                return;
            }

            int y = 0;
            foreach (var name in found)
            {
                var chk = new CheckBox
                {
                    Left = 0, Top = y, AutoSize = true,
                    Text = name,
                    Checked = true,
                    Tag = name
                };
                pnlStartupCheckboxes.Controls.Add(chk);
                y += 26;
            }
        }

        // ── Panel 7: Summary ──────────────────────────────────────────────────────
        private Panel BuildPanel7_Summary()
        {
            var panel = MakeContentPanel();

            AddStepHeading(panel, "Review & Apply",
                "The following changes will be applied when you click Finish.");

            string[] rowNames =
            {
                "Kiosk URL", "Nightly Restart", "Auto-Login",
                "Browser Kiosk", "Auto Updates", "Power & Display",
                "Notifications", "Sticky Keys", "USB Storage", "Startup Apps"
            };

            lblSummaryValues = new Label[rowNames.Length];

            int y = 56;
            for (int i = 0; i < rowNames.Length; i++)
            {
                var nameLabel = new Label
                {
                    Left = 0, Top = y, Width = 110, Height = 23,
                    Text = rowNames[i] + ":",
                    Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(50, 50, 50)
                };

                lblSummaryValues[i] = new Label
                {
                    Left = 115, Top = y, Width = ContentWidth - 115, Height = 23,
                    Font = new Font("Segoe UI", 8.5f)
                };

                panel.Controls.AddRange(new Control[] { nameLabel, lblSummaryValues[i] });
                y += 23;
            }

            return panel;
        }

        // ── Panel 8: Verify ───────────────────────────────────────────────────────
        private Panel BuildPanel8_Verify()
        {
            var panel = MakeContentPanel();

            var lblTitle = new Label
            {
                Left = 0, Top = 0, Width = ContentWidth,
                Text = "Verify Current Settings",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30)
            };

            var lblDesc = new Label
            {
                Left = 0, Top = 24, Width = ContentWidth, Height = 18,
                Text = "Checking whether previously configured settings are active in Windows...",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 8.5f)
            };

            pnlVerifyResults = new Panel
            {
                Left = 0, Top = 48, Width = ContentWidth, Height = 190,
                BackColor = Color.Transparent
            };

            var btnVerifySetup = new Button
            {
                Left = 0, Top = 245, Width = 190, Height = 29,
                Text = "Start / Modify Setup",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 120, 190),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            btnVerifySetup.FlatAppearance.BorderSize = 0;
            btnVerifySetup.Click += (s, e) =>
            {
                pnlNavBar.Visible = true;
                ShowPanel(1);
            };

            var btnVerifyExit = new Button
            {
                Left = 200, Top = 245, Width = 90, Height = 29,
                Text = "Exit",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 220, 225),
                ForeColor = Color.FromArgb(50, 50, 50)
            };
            btnVerifyExit.FlatAppearance.BorderSize = 0;
            btnVerifyExit.Click += (s, e) => this.Close();

            panel.Controls.AddRange(new Control[]
                { lblTitle, lblDesc, pnlVerifyResults, btnVerifySetup, btnVerifyExit });
            return panel;
        }

        // ── Shared helpers ────────────────────────────────────────────────────────

        private static Panel MakeContentPanel()
        {
            return new Panel
            {
                Left = ContentLeft, Top = ContentTop,
                Width = ContentWidth, Height = ContentHeight,
                BackColor = Color.Transparent,
                Visible = false
            };
        }

        private static void AddStepHeading(Panel panel, string title, string description)
        {
            var lblTitle = new Label
            {
                Left = 0, Top = 0, Width = ContentWidth, AutoSize = true,
                Text = title,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30)
            };
            var lblDesc = new Label
            {
                Left = 0, Top = 22, Width = ContentWidth, Height = 32,
                Text = description,
                ForeColor = Color.FromArgb(80, 80, 80),
                Font = new Font("Segoe UI", 8.5f)
            };
            panel.Controls.AddRange(new Control[] { lblTitle, lblDesc });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // Navigation
        // ═══════════════════════════════════════════════════════════════════════

        private void ShowPanel(int index)
        {
            _current = index;

            foreach (var p in _panels)
                p.Visible = false;
            _panels[_current].Visible = true;

            lblStepTitle.Text   = GetStepTitle(_current);
            lblStepCounter.Text = (_current >= 1 && _current <= 6)
                ? $"Step {_current} of 6"
                : string.Empty;

            // Nav bar: hidden on Welcome (0) and Verify (8)
            pnlNavBar.Visible = (_current >= 1 && _current <= 7);

            if (pnlNavBar.Visible)
            {
                btnBack.Enabled = (_current > 1);
                btnNext.Text    = (_current == 7) ? "Finish" : "Next >";
            }

            if (_current == 6) PopulateStartupApps();
            if (_current == 7) RefreshSummary();
            if (_current == 8) RunVerify();
        }

        private static string GetStepTitle(int index)
        {
            switch (index)
            {
                case 0: return "Welcome";
                case 1: return "Step 1: Kiosk URL";
                case 2: return "Step 2: Nightly Restart";
                case 3: return "Step 3: Windows Auto-Login";
                case 4: return "Step 4: Browser Kiosk Mode";
                case 5: return "Step 5: System Configuration";
                case 6: return "Step 6: Startup Apps";
                case 7: return "Review & Apply";
                case 8: return "Verify Settings";
                default: return string.Empty;
            }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (_current == 7)
                ApplyAll();
            else
                ShowPanel(_current + 1);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // Load Settings
        // ═══════════════════════════════════════════════════════════════════════

        private void LoadSettings()
        {
            var s = Settings.Load();

            // Panel 1
            txtUrl.Text = s.Url;

            // Panel 2
            if (TimeSpan.TryParse(s.RestartTime, out var ts))
                dtpRestartTime.Value = new DateTime(2000, 1, 1, ts.Hours, ts.Minutes, 0);
            chkRestart.Checked     = s.RestartEnabled;
            dtpRestartTime.Enabled = s.RestartEnabled;

            // Panel 3
            chkAutoLogin.Checked = s.AutoLoginEnabled;
            if (s.AutoLoginEnabled)
            {
                var (user, pass)    = AutoLoginHelper.GetCurrent();
                txtUsername.Text    = user;
                txtPassword.Text    = pass;
                txtUsername.Enabled = true;
                txtPassword.Enabled = true;
            }

            // Panel 4
            chkAutoLaunch.Checked = s.AutoLaunchEnabled;
            var detected = BrowserFinder.Find();
            if (!string.IsNullOrEmpty(s.BrowserPath) && File.Exists(s.BrowserPath))
            {
                txtBrowserPath.Text = s.BrowserPath;
            }
            else if (detected != null)
            {
                txtBrowserPath.Text = detected;
                bool isEdge = detected.IndexOf("msedge", StringComparison.OrdinalIgnoreCase) >= 0;
                lblBrowserDetected.ForeColor = Color.Gray;
                lblBrowserDetected.Text = isEdge
                    ? "Auto-detected: Microsoft Edge (fallback)"
                    : "Auto-detected: Google Chrome";
            }
            else
            {
                lblBrowserDetected.ForeColor = Color.DarkOrange;
                lblBrowserDetected.Text = "⚠ No browser found — please select manually.";
            }
            txtBrowserPath.Enabled = s.AutoLaunchEnabled;
            btnBrowse.Enabled      = s.AutoLaunchEnabled;

            // Panel 5
            chkAutoUpdates.Checked   = s.AutoUpdatesEnabled;
            chkPowerSettings.Checked = s.PowerSettingsEnabled;
            chkNotifications.Checked = s.NotificationsDisabled;
            chkStickyKeys.Checked    = s.StickyKeysDisabled;
            chkUsbStorage.Checked    = s.UsbStorageDisabled;

            // Welcome: show Verify button only if a previous config exists
            if (!string.IsNullOrEmpty(s.Url))
            {
                btnVerify.Visible  = true;
                btnStartSetup.Left = 200;
            }
        }

        // ═══════════════════════════════════════════════════════════════════════
        // Browse for Browser
        // ═══════════════════════════════════════════════════════════════════════

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog
            {
                Title = "Select Browser Executable",
                Filter = "Executables (*.exe)|*.exe",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtBrowserPath.Text     = dlg.FileName;
                    lblBrowserDetected.Text = string.Empty;
                }
            }
        }

        // ═══════════════════════════════════════════════════════════════════════
        // Summary (Panel 7)
        // ═══════════════════════════════════════════════════════════════════════

        private void RefreshSummary()
        {
            // Row 0 — Kiosk URL
            var url = txtUrl.Text.Trim();
            SetSummaryRow(0,
                !string.IsNullOrEmpty(url),
                !string.IsNullOrEmpty(url),
                string.IsNullOrEmpty(url) ? "No change will be made" : $"WILL BE APPLIED: {url}");

            // Row 1 — Nightly Restart
            SetSummaryRow(1,
                chkRestart.Checked,
                chkRestart.Checked,
                chkRestart.Checked
                    ? $"WILL BE APPLIED: Restart daily at {dtpRestartTime.Value:HH:mm}"
                    : "No change will be made");

            // Row 2 — Auto-Login
            bool loginOk = chkAutoLogin.Checked && !string.IsNullOrEmpty(txtUsername.Text.Trim());
            string loginDetail;
            if (!chkAutoLogin.Checked)
                loginDetail = "No change will be made";
            else if (string.IsNullOrEmpty(txtUsername.Text.Trim()))
                loginDetail = "⚠ Enabled but no username entered";
            else
                loginDetail = $"WILL BE APPLIED: Auto-login as {txtUsername.Text.Trim()}";
            SetSummaryRow(2, chkAutoLogin.Checked, loginOk, loginDetail);

            // Row 3 — Browser Kiosk
            bool browserOk = chkAutoLaunch.Checked && !string.IsNullOrEmpty(txtBrowserPath.Text.Trim());
            string browserDetail;
            if (!chkAutoLaunch.Checked)
                browserDetail = "No change will be made";
            else if (string.IsNullOrEmpty(txtBrowserPath.Text.Trim()))
                browserDetail = "⚠ Enabled but no browser selected";
            else
                browserDetail = $"WILL BE APPLIED: {Path.GetFileName(txtBrowserPath.Text)} in kiosk mode";
            SetSummaryRow(3, chkAutoLaunch.Checked, browserOk, browserDetail);

            // Row 4 — Auto Updates
            SetSummaryRow(4,
                chkAutoUpdates.Checked,
                chkAutoUpdates.Checked,
                chkAutoUpdates.Checked
                    ? "WILL BE APPLIED: Windows Update set to automatic (daily at 3 AM)"
                    : "No change will be made");

            // Row 5 — Power & Display
            SetSummaryRow(5,
                chkPowerSettings.Checked,
                chkPowerSettings.Checked,
                chkPowerSettings.Checked
                    ? "WILL BE APPLIED: Display/sleep set to Never, screensaver off"
                    : "No change will be made");

            // Row 6 — Notifications
            SetSummaryRow(6,
                chkNotifications.Checked,
                chkNotifications.Checked,
                chkNotifications.Checked
                    ? "WILL BE APPLIED: Notifications and error dialogs disabled"
                    : "No change will be made");

            // Row 7 — Sticky Keys
            SetSummaryRow(7,
                chkStickyKeys.Checked,
                chkStickyKeys.Checked,
                chkStickyKeys.Checked
                    ? "WILL BE APPLIED: Sticky/Toggle/Filter Keys popups disabled"
                    : "No change will be made");

            // Row 8 — USB Storage
            SetSummaryRow(8,
                chkUsbStorage.Checked,
                chkUsbStorage.Checked,
                chkUsbStorage.Checked
                    ? "WILL BE APPLIED: USB mass storage devices blocked"
                    : "No change will be made");

            // Row 9 — Startup Apps
            // Build _startupToDisable from the currently-checked boxes in Panel 6.
            _startupToDisable.Clear();
            foreach (Control ctrl in pnlStartupCheckboxes.Controls)
            {
                if (ctrl is CheckBox chk && chk.Checked && chk.Tag is string name)
                    _startupToDisable.Add(name);
            }
            if (_startupToDisable.Count > 0)
                SetSummaryRow(9, true, true,
                    $"WILL BE APPLIED: {_startupToDisable.Count} app(s) disabled " +
                    $"({string.Join(", ", _startupToDisable)})");
            else
                SetSummaryRow(9, false, false, "No change will be made");
        }

        private void SetSummaryRow(int i, bool enabled, bool ok, string text)
        {
            var lbl = lblSummaryValues[i];
            lbl.Text = text;
            if (!enabled)
                lbl.ForeColor = Color.Gray;
            else if (!ok)
                lbl.ForeColor = Color.DarkOrange;
            else
                lbl.ForeColor = Color.DarkGreen;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // Verify (Panel 8)
        // ═══════════════════════════════════════════════════════════════════════

        private void RunVerify()
        {
            pnlVerifyResults.Controls.Clear();

            var results = VerifyHelper.Verify(Settings.Load());
            int y = 0;

            foreach (var r in results)
            {
                string icon;
                Color  color;
                switch (r.Status)
                {
                    case VerifyStatus.Ok:
                        icon  = "✓";
                        color = Color.DarkGreen;
                        break;
                    case VerifyStatus.Fail:
                        icon  = "✗";
                        color = Color.DarkRed;
                        break;
                    default:
                        icon  = "—";
                        color = Color.Gray;
                        break;
                }

                var lblIcon = new Label
                {
                    Left = 0, Top = y, Width = 20, Height = 20,
                    Text = icon,
                    ForeColor = color,
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold)
                };
                var lblName = new Label
                {
                    Left = 22, Top = y, Width = 145, Height = 20,
                    Text = r.Name + ":",
                    Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(40, 40, 40)
                };
                var lblDetail = new Label
                {
                    Left = 170, Top = y, Width = ContentWidth - 170, Height = 20,
                    Text = r.Details,
                    ForeColor = color,
                    Font = new Font("Segoe UI", 8.5f)
                };

                pnlVerifyResults.Controls.AddRange(new Control[] { lblIcon, lblName, lblDetail });
                y += 20;
            }
        }

        // ═══════════════════════════════════════════════════════════════════════
        // Apply All (Finish)
        // ═══════════════════════════════════════════════════════════════════════

        private void ApplyAll()
        {
            var url = txtUrl.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show(
                    "Kiosk URL is required. Please go back to Step 1 and enter a URL.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            btnNext.Enabled = false;
            btnBack.Enabled = false;

            var errors = new StringBuilder();

            // ── Persist config ────────────────────────────────────────────────
            var s = new Settings
            {
                Url                   = url,
                RestartTime           = dtpRestartTime.Value.ToString("HH:mm"),
                RestartEnabled        = chkRestart.Checked,
                AutoLaunchEnabled     = chkAutoLaunch.Checked,
                BrowserPath           = txtBrowserPath.Text.Trim(),
                AutoLoginEnabled      = chkAutoLogin.Checked,
                AutoLoginUser         = txtUsername.Text.Trim(),
                AutoUpdatesEnabled    = chkAutoUpdates.Checked,
                PowerSettingsEnabled  = chkPowerSettings.Checked,
                NotificationsDisabled = chkNotifications.Checked,
                StickyKeysDisabled    = chkStickyKeys.Checked,
                UsbStorageDisabled    = chkUsbStorage.Checked
            };
            try { s.Save(); }
            catch (Exception ex) { errors.AppendLine("Config: " + ex.Message); }

            // ── Task Scheduler (nightly restart) ──────────────────────────────
            try
            {
                var (ok, msg) = chkRestart.Checked
                    ? TaskSchedulerHelper.Enable(s.RestartTime)
                    : TaskSchedulerHelper.Disable();
                if (!ok) errors.AppendLine("Restart task: " + msg);
            }
            catch (Exception ex) { errors.AppendLine("Restart task: " + ex.Message); }

            // ── Windows Auto-Login ────────────────────────────────────────────
            try
            {
                if (chkAutoLogin.Checked)
                {
                    var user = txtUsername.Text.Trim();
                    if (string.IsNullOrEmpty(user))
                        errors.AppendLine("Auto-login: Username is required.");
                    else
                        AutoLoginHelper.Enable(user, txtPassword.Text);
                }
                else
                {
                    AutoLoginHelper.Disable();
                }
            }
            catch (Exception ex) { errors.AppendLine("Auto-login: " + ex.Message); }

            // ── Browser Kiosk Startup ─────────────────────────────────────────
            try
            {
                if (chkAutoLaunch.Checked)
                {
                    var browserPath = txtBrowserPath.Text.Trim();
                    if (string.IsNullOrEmpty(browserPath) || !File.Exists(browserPath))
                        errors.AppendLine("Browser: Path is invalid or not found. Use Browse to select.");
                    else
                        StartupHelper.Enable(browserPath, url);
                }
                else
                {
                    StartupHelper.Disable();
                }
            }
            catch (Exception ex) { errors.AppendLine("Browser startup: " + ex.Message); }

            // ── Windows Auto Updates ──────────────────────────────────────────
            try
            {
                if (chkAutoUpdates.Checked)
                    WindowsUpdateHelper.Enable();
                else
                    WindowsUpdateHelper.Disable();
            }
            catch (Exception ex) { errors.AppendLine("Auto updates: " + ex.Message); }

            // ── Power Settings ────────────────────────────────────────────────
            try
            {
                if (chkPowerSettings.Checked)
                    PowerSettingsHelper.Enable();
                else
                    PowerSettingsHelper.Disable();
            }
            catch (Exception ex) { errors.AppendLine("Power settings: " + ex.Message); }

            // ── Notifications & Error Dialogs ─────────────────────────────────
            try
            {
                if (chkNotifications.Checked)
                    NotificationHelper.Disable();
                else
                    NotificationHelper.Enable();
            }
            catch (Exception ex) { errors.AppendLine("Notifications: " + ex.Message); }

            // ── Sticky Keys ───────────────────────────────────────────────────
            try
            {
                if (chkStickyKeys.Checked)
                    StickyKeysHelper.Disable();
                else
                    StickyKeysHelper.Enable();
            }
            catch (Exception ex) { errors.AppendLine("Sticky Keys: " + ex.Message); }

            // ── USB Storage ───────────────────────────────────────────────────
            try
            {
                if (chkUsbStorage.Checked)
                    UsbStorageHelper.Disable();
                else
                    UsbStorageHelper.Enable();
            }
            catch (Exception ex) { errors.AppendLine("USB storage: " + ex.Message); }

            // ── Startup app cleanup ───────────────────────────────────────────
            try
            {
                foreach (var entry in _startupToDisable)
                    StartupCleanupHelper.DisableEntry(entry);
            }
            catch (Exception ex) { errors.AppendLine("Startup cleanup: " + ex.Message); }

            // ── Result ────────────────────────────────────────────────────────
            if (errors.Length == 0)
            {
                MessageBox.Show(
                    "All settings applied successfully.\n\nThe PC is now configured for kiosk mode.",
                    "Setup Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show(
                    errors.ToString().TrimEnd(),
                    "Errors during Setup",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                btnNext.Enabled = true;
                btnBack.Enabled = true;
            }
        }
    }
}
