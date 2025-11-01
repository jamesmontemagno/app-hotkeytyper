using System.Runtime.InteropServices;
using System.Text.Json;

namespace HotkeyTyper;

public partial class Form1 : Form
{
    // Windows API constants for hotkey registration
    private const int MOD_CONTROL = 0x0002;
    private const int MOD_SHIFT = 0x0004;
    private const int WM_HOTKEY = 0x0312;
    private const int HOTKEY_ID = 1;
    
    // Snippet constants
    private const string DefaultSnippetId = "default";
    private const string DefaultSnippetName = "Default";
    private const string DefaultSnippetContent = "Hello, this is my predefined text!";
    
    // Settings file path
    private readonly string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
    
    // Typing speed (1 = slowest, 10 = fastest)
    private int typingSpeed = 5;
    
    // Cancellation for in-progress typing
    private CancellationTokenSource? typingCts;
    private readonly KeyboardInputSender inputSender = new();
    
    // Code mode flag (limits maximum speed)
    private bool hasCodeMode = false;
    private int lastNonCodeSpeed = 10; // persisted
    private ToolTip? speedToolTip;
    
    // File source mode
    private bool fileSourceMode = false;
    private string fileSourcePath = string.Empty;
    
    // Snippet management
    private List<TextSnippet> snippets = new();
    private string? activeSnippetId;
    private int nextSnippetNumber = 1; // Tracks next snippet number for unique naming
    
    // Reliability heuristics for high-speed typing
    private const int MinFastDelayMs = 35; // Minimum delay enforced when speed >= 8
    private const int ContextualPauseMs = 140; // Extra pause after space following certain boundary chars
    private static readonly char[] ContextBoundaryChars = new[] { '>', ')', '}', ']' };
    
    // System tray components
    private NotifyIcon? trayIcon;
    private ContextMenuStrip? trayMenu;
    private Icon? appIcon; // custom generated icon
    private IntPtr appIconHandle = IntPtr.Zero; // native handle for cleanup
    
    // Windows API imports
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
    
    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
    
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    public Form1()
    {
        InitializeComponent();
        InitializeSystemTray();
        LoadSettings();
        // NOTE: Global hotkey registration is now handled in OnHandleCreated so that
        // it is automatically re-registered if the window handle is recreated (e.g.
        // when toggling ShowInTaskbar while minimizing to tray). Previously the hotkey
        // stopped working after minimizing because ShowInTaskbar can force a handle
        // recreation which invalidated the original RegisterHotKey binding.
        
        // Update UI after form is fully loaded
        this.Load += Form1_Load;
    }
    
    private void Form1_Load(object? sender, EventArgs e)
    {
        UpdateUIFromSettings();
    }
    
    private void UpdateUIFromSettings()
    {
        // Update snippet ComboBox
        if (cmbSnippets != null)
        {
            cmbSnippets.Items.Clear();
            foreach (var snippet in snippets)
            {
                cmbSnippets.Items.Add(snippet.Name);
            }
            
            var activeSnippet = GetActiveSnippet();
            if (activeSnippet != null)
            {
                int index = snippets.IndexOf(activeSnippet);
                if (index >= 0)
                {
                    cmbSnippets.SelectedIndex = index;
                }
            }
        }
        
        // Update text box with loaded active snippet content
        if (txtPredefinedText != null)
        {
            var activeSnippet = GetActiveSnippet();
            txtPredefinedText.Text = activeSnippet?.Content ?? string.Empty;
        }
        
        // Update typing speed slider with loaded value
        if (sliderTypingSpeed != null)
        {
            if (sliderTypingSpeed is LimitedTrackBar lt)
            {
                lt.SoftMax = hasCodeMode ? 8 : null;
            }
            if (hasCodeMode && typingSpeed > 8)
            {
                typingSpeed = 8; // clamp from settings if needed
            }
            sliderTypingSpeed.Value = Math.Min(Math.Max(typingSpeed, sliderTypingSpeed.Minimum), sliderTypingSpeed.Maximum);
        }
        
        // Update speed indicator label
        if (lblSpeedIndicator != null)
        {
            lblSpeedIndicator.Text = GetSpeedText(typingSpeed) + (hasCodeMode ? " (code mode)" : string.Empty);
        }
        if (chkHasCode != null)
        {
            chkHasCode.Checked = hasCodeMode;
        }
        if (chkUseFile != null)
        {
            chkUseFile.Checked = fileSourceMode;
        }
        if (txtFilePath != null)
        {
            txtFilePath.Text = fileSourcePath;
            txtFilePath.Enabled = fileSourceMode;
        }
        if (btnBrowseFile != null)
        {
            btnBrowseFile.Enabled = fileSourceMode;
        }
        if (txtPredefinedText != null)
        {
            txtPredefinedText.Enabled = !fileSourceMode;
        }
        UpdateTooltips();
    }
    
    private void LoadSettings()
    {
        try
        {
            if (File.Exists(settingsFilePath))
            {
                string json = File.ReadAllText(settingsFilePath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                if (settings != null)
                {
                    if (settings.TypingSpeed > 0 && settings.TypingSpeed <= 10)
                    {
                        typingSpeed = settings.TypingSpeed;
                    }
                    hasCodeMode = settings.HasCode;
                    if (settings.LastNonCodeSpeed >= 1 && settings.LastNonCodeSpeed <= 10)
                    {
                        lastNonCodeSpeed = settings.LastNonCodeSpeed;
                    }
                    fileSourceMode = settings.UseFileSource;
                    if (!string.IsNullOrWhiteSpace(settings.FileSourcePath))
                    {
                        fileSourcePath = settings.FileSourcePath;
                    }
                    
                    // Migration: Convert old single PredefinedText to snippet if no snippets exist
                    if (settings.Snippets == null || settings.Snippets.Count == 0)
                    {
                        string contentToMigrate = !string.IsNullOrEmpty(settings.PredefinedText) 
                            ? settings.PredefinedText 
                            : DefaultSnippetContent;
                        snippets = new List<TextSnippet>
                        {
                            new TextSnippet
                            {
                                Id = DefaultSnippetId,
                                Name = DefaultSnippetName,
                                Content = contentToMigrate,
                                LastUsed = DateTime.Now
                            }
                        };
                        activeSnippetId = DefaultSnippetId;
                    }
                    else
                    {
                        snippets = settings.Snippets;
                        activeSnippetId = settings.ActiveSnippetId;
                        
                        // Validate active snippet exists
                        if (string.IsNullOrEmpty(activeSnippetId) || !snippets.Any(s => s.Id == activeSnippetId))
                        {
                            activeSnippetId = snippets.FirstOrDefault()?.Id;
                        }
                        
                        // Update snippet counter for unique naming
                        UpdateSnippetCounter();
                    }
                }
                else
                {
                    InitializeDefaultSnippet();
                }
            }
            else
            {
                InitializeDefaultSnippet();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading settings: {ex.Message}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            InitializeDefaultSnippet();
        }
    }
    
    private void UpdateSnippetCounter()
    {
        // Find highest numbered snippet to ensure unique naming
        int maxNum = 0;
        foreach (var snippet in snippets)
        {
            if (snippet.Name.StartsWith("Snippet ", StringComparison.OrdinalIgnoreCase))
            {
                string numPart = snippet.Name.Substring(8);
                if (int.TryParse(numPart, out int num))
                {
                    maxNum = Math.Max(maxNum, num);
                }
            }
        }
        nextSnippetNumber = maxNum + 1;
    }
    
    private void InitializeDefaultSnippet()
    {
        snippets = new List<TextSnippet>
        {
            new TextSnippet
            {
                Id = DefaultSnippetId,
                Name = DefaultSnippetName,
                Content = DefaultSnippetContent,
                LastUsed = DateTime.Now
            }
        };
        activeSnippetId = DefaultSnippetId;
        nextSnippetNumber = 2; // Next snippet will be "Snippet 2"
    }
    
    private void SaveSettings()
    {
        try
        {
            var settings = new AppSettings 
            { 
                PredefinedText = string.Empty, // Deprecated but kept for compatibility
                TypingSpeed = typingSpeed, 
                HasCode = hasCodeMode, 
                LastNonCodeSpeed = lastNonCodeSpeed, 
                UseFileSource = fileSourceMode, 
                FileSourcePath = fileSourcePath,
                Snippets = snippets,
                ActiveSnippetId = activeSnippetId
            };
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(settingsFilePath, json);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private void InitializeSystemTray()
    {
        // Create system tray icon
        if (appIcon == null)
        {
            // Generate & cache custom icon (shared between form & tray)
            appIcon = IconFactory.Create(out appIconHandle);
            this.Icon = appIcon; // taskbar / alt-tab icon
            // Export icon for user customization if not already present
            try
            {
                string exportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HotkeyTyper.ico");
                IconFactory.TryExportIcon(appIcon, exportPath);
            }
            catch { /* ignore export errors */ }
        }
        trayIcon = new NotifyIcon()
        {
            Text = "Hotkey Typer - CTRL+SHIFT+1 Active",
            Visible = true,
            Icon = appIcon ?? SystemIcons.Application
        };
        
        // Create context menu for tray icon
        trayMenu = new ContextMenuStrip();
        trayMenu.Items.Add("Show", null, ShowForm_Click);
        trayMenu.Items.Add("Exit", null, Exit_Click);
        
        trayIcon.ContextMenuStrip = trayMenu;
        trayIcon.DoubleClick += ShowForm_Click;
    }
    
    private void ShowForm_Click(object? sender, EventArgs e)
    {
        this.Show();
        this.WindowState = FormWindowState.Normal;
        this.ShowInTaskbar = true;
        this.BringToFront();
    }
    
    private void Exit_Click(object? sender, EventArgs e)
    {
        Application.Exit();
    }
    
    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
        {
            // Hotkey was pressed, begin typing (if not already typing)
            StartTyping();
        }
        
        base.WndProc(ref m);
    }
    
    private void StartTyping()
    {
        // Ignore if already typing
        if (typingCts != null)
        {
            return;
        }

        typingCts = new CancellationTokenSource();
        var token = typingCts.Token;

        // Determine content to type (just-in-time). Do NOT overwrite the user's textbox content when using file mode.
        string? contentToType;
        if (fileSourceMode)
        {
            bool truncated;
            contentToType = LoadFileContentForTyping(out truncated);
            if (contentToType == null)
            {
                // Abort typing if file not available
                typingCts.Dispose();
                typingCts = null;
                if (lblStatus != null)
                {
                    lblStatus.Text = "Status: File not found";
                    lblStatus.ForeColor = Color.Red;
                }
                return;
            }
            if (truncated && lblStatus != null)
            {
                lblStatus.Text = "Status: File truncated for typing";
                lblStatus.ForeColor = Color.DarkOrange;
            }
        }
        else
        {
            var activeSnippet = GetActiveSnippet();
            contentToType = activeSnippet?.Content ?? string.Empty;
        }

        // Update UI state
        if (lblStatus != null)
        {
            lblStatus.Text = "Status: Typing in progress...";
            lblStatus.ForeColor = Color.DarkOrange;
        }
        if (btnStop != null)
        {
            btnStop.Enabled = true;
        }

        // Fire and forget task (safe because we manage CTS and UI context)
        _ = TypePredefinedTextAsync(token, contentToType);
    }

    private async Task TypePredefinedTextAsync(CancellationToken token, string content)
    {
        try
        {
            // Flush any pending keys from the hotkey press
            SendKeys.Flush();
            
            // Capture current foreground window so we can restore focus after initial delay
            IntPtr targetWindow = GetForegroundWindow();

            // Wait for hotkey to be fully released and target window to be ready
            await Task.Delay(250, token);

            // Attempt to restore focus to previously active window (ignore failures)
            if (targetWindow != IntPtr.Zero)
            {
                _ = SetForegroundWindow(targetWindow);
            }
            
            // Calculate delay based on typing speed (1=slowest, 10=fastest)
            int baseDelay = 310 - (typingSpeed * 30); // Base delay calculation
            int variation = Math.Max(10, baseDelay / 3); // Variation amount
            Random random = new Random();
            
            char prev = '\0';
            for (int index = 0; index < content.Length; index++)
            {
                if (token.IsCancellationRequested) break;
                char c = content[index];

                // Contextual pause heuristic: if previous char was a boundary and current is a space, pause before continuing
                if (prev != '\0' && c == ' ' && ContextBoundaryChars.Contains(prev) && typingSpeed >= 7)
                {
                    try { await Task.Delay(ContextualPauseMs, token); } catch (OperationCanceledException) { break; }
                }

                // Use SendInput for improved reliability at high speeds.
                // Fallback to SendKeys only if character requires special translation (currently none, Unicode covers most cases).
                bool ok = inputSender.SendChar(c);
                if (!ok)
                {
                    // Fallback to SendKeys for this character
                    string fallback = EscapeForSendKeys(c);
                    if (fallback.Length > 0)
                    {
                        SendKeys.SendWait(fallback);
                    }
                }

                int delay = Math.Max(20, random.Next(Math.Max(10, baseDelay - variation), baseDelay + variation));
                if (typingSpeed >= 8 && delay < MinFastDelayMs)
                {
                    delay = MinFastDelayMs; // enforce safety margin
                }
                try
                {
                    await Task.Delay(delay, token);
                }
                catch (OperationCanceledException)
                {
                    break; // exit loop promptly on cancellation
                }

                prev = c;
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when user clicks Stop
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error typing text: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            // Reset UI state
            typingCts?.Dispose();
            typingCts = null;
            if (btnStop != null)
            {
                btnStop.Enabled = false;
            }
            if (lblStatus != null)
            {
                lblStatus.Text = "Status: Hotkey CTRL+SHIFT+1 is active";
                lblStatus.ForeColor = Color.Green;
            }
        }
    }

    /// <summary>
    /// Escapes a single character for SendKeys so that source code or other literal text
    /// (including parentheses and plus/caret/percent signs) can be typed without triggering
    /// SendKeys grouping or modifier semantics.
    /// </summary>
    internal static string EscapeForSendKeys(char c)
    {
        return c switch
        {
            // Modifier / special grouping characters that must be wrapped in braces to be literal
            '+' => "{+}",
            '^' => "{^}",
            '%' => "{%}",
            '~' => "{~}",
            '(' => "{(}",
            ')' => "{)}",
            // Braces require doubling pattern
            '{' => "{{}",
            '}' => "{}}",
            // Translate newlines / tabs to their SendKeys representations
            '\n' => "{ENTER}",
            '\r' => string.Empty, // ignore CR in CRLF to avoid double-enter
            '\t' => "{TAB}",
            _ => c.ToString()
        };
    }
    
    private void BtnUpdate_Click(object? sender, EventArgs e)
    {
        if (fileSourceMode)
        {
            // In file mode, saving text box content is irrelevant; show passive status.
            if (lblStatus != null)
            {
                lblStatus.Text = "Status: File mode active (text box not saved)";
                lblStatus.ForeColor = Color.DarkOrange;
            }
            return;
        }

        if (txtPredefinedText != null)
        {
            SaveActiveSnippetContent();
            SaveSettings();
            if (lblStatus != null)
            {
                lblStatus.Text = "Status: Snippet saved";
                lblStatus.ForeColor = Color.Green;
            }
        }
    }
    
    private void TypingSpeedSlider_ValueChanged(object? sender, EventArgs e)
    {
        if (sender is TrackBar slider)
        {
            if (hasCodeMode && slider.Value > 8)
            {
                slider.Value = 8; // clamp
            }
            typingSpeed = slider.Value;
            if (lblSpeedIndicator != null)
            {
                lblSpeedIndicator.Text = GetSpeedText(typingSpeed) + (hasCodeMode ? " (code mode)" : string.Empty);
            }
            SaveSettings();
        }
    }

    private void ChkHasCode_CheckedChanged(object? sender, EventArgs e)
    {
        if (sender is CheckBox cb)
        {
            hasCodeMode = cb.Checked;
            if (hasCodeMode)
            {
                // entering code mode: remember current non-code speed
                lastNonCodeSpeed = typingSpeed;
                if (typingSpeed > 8 && sliderTypingSpeed != null)
                {
                    typingSpeed = 8;
                    sliderTypingSpeed.Value = 8;
                    NotifyClamped();
                }
            }
            else
            {
                // leaving code mode: restore last non-code speed
                typingSpeed = Math.Min(Math.Max(lastNonCodeSpeed, 1), 10);
                if (sliderTypingSpeed != null)
                {
                    sliderTypingSpeed.Value = typingSpeed;
                }
            }
            if (sliderTypingSpeed is LimitedTrackBar lt)
            {
                lt.SoftMax = hasCodeMode ? 8 : null;
                sliderTypingSpeed.Invalidate();
            }
            if (lblSpeedIndicator != null)
            {
                lblSpeedIndicator.Text = GetSpeedText(typingSpeed) + (hasCodeMode ? " (code mode)" : string.Empty);
            }
            UpdateTooltips();
            SaveSettings();
        }
    }

    private void ChkUseFile_CheckedChanged(object? sender, EventArgs e)
    {
        if (sender is CheckBox cb)
        {
            fileSourceMode = cb.Checked;
            if (txtFilePath != null) txtFilePath.Enabled = fileSourceMode;
            if (btnBrowseFile != null) btnBrowseFile.Enabled = fileSourceMode;
            if (txtPredefinedText != null) txtPredefinedText.Enabled = !fileSourceMode;
            SaveSettings();
        }
    }

    private void BtnBrowseFile_Click(object? sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog
        {
            Title = "Select text or markdown file",
            Filter = "Text/Markdown (*.txt;*.md)|*.txt;*.md|All Files (*.*)|*.*",
            CheckFileExists = true,
            Multiselect = false
        };
        if (ofd.ShowDialog() == DialogResult.OK)
        {
            fileSourcePath = ofd.FileName;
            if (txtFilePath != null) txtFilePath.Text = fileSourcePath;
            SaveSettings();
        }
    }

    private string? LoadFileContentForTyping(out bool truncated)
    {
        truncated = false;
        if (string.IsNullOrWhiteSpace(fileSourcePath)) return null;
        try
        {
            if (!File.Exists(fileSourcePath)) return null;
            string text = File.ReadAllText(fileSourcePath);
            const int maxLen = 50000;
            if (text.Length > maxLen)
            {
                text = text.Substring(0, maxLen);
                truncated = true;
            }
            return text;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }
    }

    private void NotifyClamped()
    {
        if (lblStatus != null)
        {
            lblStatus.Text = "Status: Speed limited in code mode";
            lblStatus.ForeColor = Color.DarkOrange;
        }
    }

    private void UpdateTooltips()
    {
        if (speedToolTip == null && sliderTypingSpeed != null)
        {
            speedToolTip = new ToolTip();
        }
        if (speedToolTip != null && sliderTypingSpeed != null && lblSpeedIndicator != null)
        {
            string msg = hasCodeMode
                ? "Code mode: max speed limited to Fast (8) to improve reliability while typing code."
                : "Normal mode: speeds 1 (Very Slow) to 10 (Very Fast).";
            speedToolTip.SetToolTip(sliderTypingSpeed, msg);
            speedToolTip.SetToolTip(lblSpeedIndicator, msg);
        }
    }
    
    private string GetSpeedText(int speed)
    {
        return speed switch
        {
            1 or 2 => "Very Slow",
            3 or 4 => "Slow", 
            5 or 6 => "Normal",
            7 or 8 => "Fast",
            9 or 10 => "Very Fast",
            _ => "Normal"
        };
    }
    
    private void BtnMinimize_Click(object sender, EventArgs e)
    {
        this.WindowState = FormWindowState.Minimized;
        this.ShowInTaskbar = false;
    }
    
    private void BtnStop_Click(object? sender, EventArgs e)
    {
        if (typingCts != null && !typingCts.IsCancellationRequested)
        {
            typingCts.Cancel();
            if (lblStatus != null)
            {
                lblStatus.Text = "Status: Typing cancelled";
                lblStatus.ForeColor = Color.Red;
            }
            if (btnStop != null)
            {
                btnStop.Enabled = false;
            }
        }
    }
    
    // Snippet UI event handlers
    
    private void CmbSnippets_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (cmbSnippets == null) return;
        
        int index = cmbSnippets.SelectedIndex;
        if (index >= 0 && index < snippets.Count)
        {
            // Save current snippet content before switching
            SaveActiveSnippetContent();
            
            // Switch to new snippet
            activeSnippetId = snippets[index].Id;
            
            // Load new snippet content
            if (txtPredefinedText != null)
            {
                txtPredefinedText.Text = snippets[index].Content;
            }
            
            SaveSettings();
        }
    }
    
    private void BtnNewSnippet_Click(object? sender, EventArgs e)
    {
        CreateNewSnippet();
        UpdateUIFromSettings();
        if (lblStatus != null)
        {
            lblStatus.Text = "Status: New snippet created";
            lblStatus.ForeColor = Color.Green;
        }
    }
    
    private void BtnDuplicateSnippet_Click(object? sender, EventArgs e)
    {
        DuplicateActiveSnippet();
        UpdateUIFromSettings();
        if (lblStatus != null)
        {
            lblStatus.Text = "Status: Snippet duplicated";
            lblStatus.ForeColor = Color.Green;
        }
    }
    
    private void BtnRenameSnippet_Click(object? sender, EventArgs e)
    {
        var current = GetActiveSnippet();
        if (current == null) return;
        
        string? newName = InputDialog.Show(
            "Enter new name for snippet:",
            "Rename Snippet",
            current.Name);
        
        if (!string.IsNullOrWhiteSpace(newName))
        {
            RenameActiveSnippet(newName);
            UpdateUIFromSettings();
            if (lblStatus != null)
            {
                lblStatus.Text = "Status: Snippet renamed";
                lblStatus.ForeColor = Color.Green;
            }
        }
    }
    
    private void BtnDeleteSnippet_Click(object? sender, EventArgs e)
    {
        DeleteActiveSnippet();
        UpdateUIFromSettings();
        if (lblStatus != null)
        {
            lblStatus.Text = "Status: Snippet deleted";
            lblStatus.ForeColor = Color.Green;
        }
    }
    
    protected override void Dispose(bool disposing)
    {
        // Unregister hotkey when form is disposed
        UnregisterHotKey(this.Handle, HOTKEY_ID);
        
        // Clean up system tray
        if (trayIcon != null)
        {
            trayIcon.Visible = false;
            trayIcon.Dispose();
        }
        trayMenu?.Dispose();
        if (appIcon != null)
        {
            IconFactory.Destroy(ref appIcon, ref appIconHandle);
        }
        
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }
    
    // Snippet management methods
    
    private TextSnippet? GetActiveSnippet()
    {
        return snippets.FirstOrDefault(s => s.Id == activeSnippetId);
    }
    
    private void SaveActiveSnippetContent()
    {
        var activeSnippet = GetActiveSnippet();
        if (activeSnippet != null && txtPredefinedText != null)
        {
            activeSnippet.Content = txtPredefinedText.Text;
            activeSnippet.LastUsed = DateTime.Now;
        }
    }
    
    private void CreateNewSnippet()
    {
        string newId = Guid.NewGuid().ToString();
        var newSnippet = new TextSnippet
        {
            Id = newId,
            Name = $"Snippet {nextSnippetNumber++}",
            Content = string.Empty,
            LastUsed = DateTime.Now
        };
        snippets.Add(newSnippet);
        activeSnippetId = newId;
        SaveSettings();
    }
    
    private void DuplicateActiveSnippet()
    {
        var current = GetActiveSnippet();
        if (current == null) return;
        
        string newId = Guid.NewGuid().ToString();
        var duplicate = new TextSnippet
        {
            Id = newId,
            Name = $"{current.Name} (Copy)",
            Content = current.Content,
            LastUsed = DateTime.Now
        };
        snippets.Add(duplicate);
        activeSnippetId = newId;
        SaveSettings();
    }
    
    private void RenameActiveSnippet(string newName)
    {
        var current = GetActiveSnippet();
        if (current == null) return;
        
        // Validate: trim and check uniqueness (case-insensitive)
        newName = newName.Trim();
        if (string.IsNullOrEmpty(newName))
        {
            MessageBox.Show("Snippet name cannot be empty.", "Invalid Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        if (snippets.Any(s => s.Id != current.Id && string.Equals(s.Name, newName, StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show("A snippet with this name already exists.", "Duplicate Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        current.Name = newName;
        SaveSettings();
    }
    
    private void DeleteActiveSnippet()
    {
        if (snippets.Count <= 1)
        {
            MessageBox.Show("Cannot delete the last remaining snippet.", "Delete Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        var current = GetActiveSnippet();
        if (current == null) return;
        
        var result = MessageBox.Show($"Delete snippet '{current.Name}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result != DialogResult.Yes) return;
        
        snippets.Remove(current);
        activeSnippetId = snippets.FirstOrDefault()?.Id;
        SaveSettings();
    }

    /// <summary>
    /// Ensures the global hotkey is (re)registered whenever the form's native handle is created.
    /// This fixes a bug where minimizing to tray (setting ShowInTaskbar = false) can recreate the
    /// window handle and silently detach the previously registered hotkey.
    /// </summary>
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        TryRegisterGlobalHotkey();
    }

    /// <summary>
    /// Attempt to register CTRL+SHIFT+1 as a global hotkey. If the hotkey is already registered
    /// (stale from a previous handle) we first try to unregister. Any failure is surfaced in the
    /// status label and via a tray balloon tip for visibility.
    /// </summary>
    private void TryRegisterGlobalHotkey()
    {
        // Best-effort: in case a previous registration existed for a prior handle instance.
        UnregisterHotKey(this.Handle, HOTKEY_ID);

        if (!RegisterHotKey(this.Handle, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, (int)Keys.D1))
        {
            if (lblStatus != null)
            {
                lblStatus.Text = "Status: Failed to register global hotkey";
                lblStatus.ForeColor = Color.Red;
            }
            trayIcon?.ShowBalloonTip(3000, "Hotkey Typer", "Failed to register global hotkey CTRL+SHIFT+1. It may be in use by another application.", ToolTipIcon.Error);
        }
        else
        {
            if (lblStatus != null && lblStatus.Text.StartsWith("Status: Failed", StringComparison.OrdinalIgnoreCase))
            {
                lblStatus.Text = "Status: Hotkey CTRL+SHIFT+1 is active";
                lblStatus.ForeColor = Color.Green;
            }
        }
    }
}

// Settings class for JSON serialization
public class AppSettings
{
    public string PredefinedText { get; set; } = string.Empty;
    public int TypingSpeed { get; set; } = 5;
    public bool HasCode { get; set; } = false;
    public int LastNonCodeSpeed { get; set; } = 10;
    public bool UseFileSource { get; set; } = false;
    public string FileSourcePath { get; set; } = string.Empty;
    
    // New snippet-based system
    public List<TextSnippet>? Snippets { get; set; }
    public string? ActiveSnippetId { get; set; }
}

// Represents a single text snippet
public class TextSnippet
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime LastUsed { get; set; } = DateTime.MinValue;
}
