using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace KioskLauncher
{
    public class MainForm : Form
    {
        // ── Controls ────────────────────────────────────────────────────────
        private TextBox    txtUrl             = null!;
        private CheckBox   chkRestart         = null!;
        private DateTimePicker dtpRestartTime = null!;
        private CheckBox   chkAutoLogin       = null!;
        private TextBox    txtUsername         = null!;
        private TextBox    txtPassword         = null!;
        private CheckBox   chkAutoLaunch       = null!;
        private TextBox    txtBrowserPath      = null!;
        private Button     btnBrowse           = null!;
        private Label      lblBrowserDetected  = null!;
        private Button     btnSave             = null!;
        private Label      lblStatus           = null!;

        // ── Constructor ─────────────────────────────────────────────────────
        public MainForm()
        {
            BuildUI();
            LoadSettings();
        }

        // ── UI Construction ──────────────────────────────────────────────────
        private void BuildUI()
        {
            SuspendLayout();

            Text = "Kiosk Launcher";
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9f);
            BackColor = Color.FromArgb(245, 245, 248);

            const int L  = 14;   // left margin for groups
            const int GW = 472;  // group box width

            int y = 12;

            // ─── KIOSK URL ─────────────────────────────────────────────────
            var grpUrl = CreateGroup("Kiosk URL", y, L, GW, 52);
            txtUrl = new TextBox { Left = 8, Top = 20, Width = 450 };
            grpUrl.Controls.Add(txtUrl);
            y += 52 + 8;

            // ─── NIGHTLY RESTART ───────────────────────────────────────────
            var grpRestart = CreateGroup("Nightly Restart", y, L, GW, 52);
            chkRestart = new CheckBox
            {
                Left = 8, Top = 22, AutoSize = true,
                Text = "Restart PC every night at:"
            };
            dtpRestartTime = new DateTimePicker
            {
                Left = 210, Top = 20, Width = 80,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "HH:mm",
                ShowUpDown = true,
                Enabled = false,
                Value = new DateTime(2000, 1, 1, 2, 0, 0)
            };
            chkRestart.CheckedChanged += (sender, e) =>
                dtpRestartTime.Enabled = chkRestart.Checked;

            grpRestart.Controls.AddRange(new Control[] { chkRestart, dtpRestartTime });
            y += 52 + 8;

            // ─── WINDOWS AUTO-LOGIN ────────────────────────────────────────
            var grpLogin = CreateGroup("Windows Auto-Login", y, L, GW, 132);
            chkAutoLogin = new CheckBox
            {
                Left = 8, Top = 20, AutoSize = true,
                Text = "Enable Windows auto-login on startup"
            };
            var lblWarn = new Label
            {
                Left = 8, Top = 43, Width = 452, Height = 30,
                Text = "⚠  Password stored in registry (same as netplwiz). " +
                       "Only use on a dedicated, physically secure PC.",
                ForeColor = Color.DarkOrange,
                Font = new Font("Segoe UI", 8f)
            };
            var lblUser = new Label { Left = 8, Top = 82, AutoSize = true, Text = "Username:" };
            txtUsername = new TextBox { Left = 80, Top = 79, Width = 200, Enabled = false };
            var lblPass = new Label { Left = 8, Top = 110, AutoSize = true, Text = "Password:" };
            txtPassword = new TextBox
            {
                Left = 80, Top = 107, Width = 200,
                UseSystemPasswordChar = true, Enabled = false
            };
            chkAutoLogin.CheckedChanged += (sender, e) =>
            {
                txtUsername.Enabled = chkAutoLogin.Checked;
                txtPassword.Enabled = chkAutoLogin.Checked;
            };
            grpLogin.Controls.AddRange(new Control[]
                { chkAutoLogin, lblWarn, lblUser, txtUsername, lblPass, txtPassword });
            y += 132 + 8;

            // ─── BROWSER KIOSK MODE ────────────────────────────────────────
            var grpBrowser = CreateGroup("Browser Kiosk Mode", y, L, GW, 80);
            chkAutoLaunch = new CheckBox
            {
                Left = 8, Top = 20, AutoSize = true,
                Text = "Open URL in kiosk mode on every startup"
            };
            var lblBrow = new Label { Left = 8, Top = 51, AutoSize = true, Text = "Browser:" };
            txtBrowserPath = new TextBox { Left = 68, Top = 48, Width = 304, Enabled = false };
            btnBrowse = new Button
            {
                Left = 378, Top = 47, Width = 80, Height = 24,
                Text = "Browse...", Enabled = false
            };
            lblBrowserDetected = new Label
            {
                Left = 8, Top = 68, Width = 452, Height = 16,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 7.5f)
            };
            btnBrowse.Click += BtnBrowse_Click;
            chkAutoLaunch.CheckedChanged += (sender, e) =>
            {
                txtBrowserPath.Enabled = chkAutoLaunch.Checked;
                btnBrowse.Enabled = chkAutoLaunch.Checked;
            };
            grpBrowser.Controls.AddRange(new Control[]
                { chkAutoLaunch, lblBrow, txtBrowserPath, btnBrowse, lblBrowserDetected });
            y += 80 + 14;

            // ─── SAVE & STATUS ─────────────────────────────────────────────
            btnSave = new Button
            {
                Left = L, Top = y, Width = 130, Height = 32,
                Text = "Save && Apply",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(60, 120, 190),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            lblStatus = new Label
            {
                Left = L + 140, Top = y + 7, Width = 330, Height = 20,
                ForeColor = Color.Gray, Text = "Not saved yet."
            };

            Controls.AddRange(new Control[]
                { grpUrl, grpRestart, grpLogin, grpBrowser, btnSave, lblStatus });
            ClientSize = new Size(500, y + 52);

            ResumeLayout(false);
        }

        private static GroupBox CreateGroup(string title, int top, int left, int width, int height)
        {
            return new GroupBox
            {
                Text = title,
                Left = left, Top = top,
                Width = width, Height = height,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                BackColor = Color.White
            };
        }

        // ── Load Saved Settings ──────────────────────────────────────────────
        private void LoadSettings()
        {
            var s = Settings.Load();

            txtUrl.Text = s.Url;

            if (TimeSpan.TryParse(s.RestartTime, out var ts))
                dtpRestartTime.Value = new DateTime(2000, 1, 1, ts.Hours, ts.Minutes, 0);

            chkRestart.Checked = s.RestartEnabled;
            dtpRestartTime.Enabled = s.RestartEnabled;

            chkAutoLogin.Checked = s.AutoLoginEnabled;
            if (s.AutoLoginEnabled)
            {
                var (user, pass) = AutoLoginHelper.GetCurrent();
                txtUsername.Text = user;
                txtPassword.Text = pass;
                txtUsername.Enabled = true;
                txtPassword.Enabled = true;
            }

            chkAutoLaunch.Checked = s.AutoLaunchEnabled;

            // Browser path: saved path takes priority; auto-detect as fallback
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
            btnBrowse.Enabled = s.AutoLaunchEnabled;
        }

        // ── Browse for Browser ───────────────────────────────────────────────
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
                    txtBrowserPath.Text = dlg.FileName;
                    lblBrowserDetected.Text = string.Empty;
                }
            }
        }

        // ── Save & Apply ─────────────────────────────────────────────────────
        private void BtnSave_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            ShowStatus("Applying...", Color.Gray);
            Application.DoEvents();

            // Validate
            var url = txtUrl.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                ShowStatus("⚠ Kiosk URL is required.", Color.DarkRed);
                btnSave.Enabled = true;
                return;
            }

            var errors = new StringBuilder();

            // ── Persist config ────────────────────────────────────────────
            var s = new Settings
            {
                Url               = url,
                RestartTime       = dtpRestartTime.Value.ToString("HH:mm"),
                RestartEnabled    = chkRestart.Checked,
                AutoLaunchEnabled = chkAutoLaunch.Checked,
                BrowserPath       = txtBrowserPath.Text.Trim(),
                AutoLoginEnabled  = chkAutoLogin.Checked,
                AutoLoginUser     = txtUsername.Text.Trim()
            };
            try { s.Save(); }
            catch (Exception ex) { errors.AppendLine("Config: " + ex.Message); }

            // ── Task Scheduler (nightly restart) ──────────────────────────
            try
            {
                var (ok, msg) = chkRestart.Checked
                    ? TaskSchedulerHelper.Enable(s.RestartTime)
                    : TaskSchedulerHelper.Disable();

                if (!ok) errors.AppendLine("Restart task: " + msg);
            }
            catch (Exception ex) { errors.AppendLine("Restart task: " + ex.Message); }

            // ── Windows Auto-Login ─────────────────────────────────────────
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

            // ── Browser Kiosk Startup ──────────────────────────────────────
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

            // ── Result ────────────────────────────────────────────────────
            if (errors.Length == 0)
            {
                ShowStatus("✓ All settings applied successfully.", Color.DarkGreen);
            }
            else
            {
                ShowStatus("⚠ Applied with errors — see details.", Color.DarkRed);
                MessageBox.Show(
                    errors.ToString().TrimEnd(),
                    "Errors during Save & Apply",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            btnSave.Enabled = true;
        }

        private void ShowStatus(string text, Color color)
        {
            lblStatus.ForeColor = color;
            lblStatus.Text = text;
        }
    }
}
