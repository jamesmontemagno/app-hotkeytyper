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
    
    // Settings file path
    private readonly string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
    
    // Predefined text to type (legacy support)
    private string predefinedText = "Hello, this is my predefined text!";
    
    // Snippet management
    private List<Snippet> snippets = new();
    private string activeSnippetId = string.Empty;
    
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
        // Update snippet controls
        UpdateSnippetComboBox();
        UpdateSnippetTextBox();
        
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
        if (cmbSnippets != null)
        {
            cmbSnippets.Enabled = !fileSourceMode;
        }
        if (btnNewSnippet != null)
        {
            btnNewSnippet.Enabled = !fileSourceMode;
        }
        if (btnDuplicateSnippet != null)
        {
            btnDuplicateSnippet.Enabled = !fileSourceMode;
        }
        if (btnRenameSnippet != null)
        {
            btnRenameSnippet.Enabled = !fileSourceMode;
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
                    ApplySettingsWithMigration(settings);
                }
            }
            else
            {
                // Create default snippet for new installations
                CreateDefaultSnippet();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading settings: {ex.Message}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            
            // Fallback to default snippet on error
            if (snippets.Count == 0)
            {
                CreateDefaultSnippet();
            }
        }
    }
    
    private void ApplySettingsWithMigration(AppSettings settings)
    {
        // Load snippets if they exist
        if (settings.Snippets.Count > 0)
        {
            snippets = settings.Snippets;
            activeSnippetId = settings.ActiveSnippetId;
            
            // Ensure active snippet is valid
            if (string.IsNullOrEmpty(activeSnippetId) || !snippets.Any(s => s.Id == activeSnippetId))
            {
                activeSnippetId = snippets.First().Id;
            }
        }
        else
        {
            // Migration from old format: create default snippet from PredefinedText
            var defaultContent = !string.IsNullOrEmpty(settings.PredefinedText) 
                ? settings.PredefinedText 
                : predefinedText;
                
            var defaultSnippet = new Snippet
            {
                Id = "default",
                Name = "Default",
                Content = defaultContent,
                LastUsed = DateTime.Now
            };
            
            snippets = new List<Snippet> { defaultSnippet };
            activeSnippetId = "default";
            
            // Save the migrated data
            SaveSettings();
        }
        
        // Load other settings
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
    }
    
    private void CreateDefaultSnippet()
    {
        var defaultSnippet = new Snippet
        {
            Id = "default",
            Name = "Default",
            Content = predefinedText,
            LastUsed = DateTime.Now
        };
        
        snippets = new List<Snippet> { defaultSnippet };
        activeSnippetId = "default";
    }
    
    private void SaveSettings()
    {
        try
        {
            var settings = new AppSettings 
            { 
                Snippets = snippets,
                ActiveSnippetId = activeSnippetId,
                TypingSpeed = typingSpeed, 
                HasCode = hasCodeMode, 
                LastNonCodeSpeed = lastNonCodeSpeed, 
                UseFileSource = fileSourceMode, 
                FileSourcePath = fileSourcePath 
            };
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(settingsFilePath, json);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private Snippet? GetActiveSnippet()
    {
        return snippets.FirstOrDefault(s => s.Id == activeSnippetId);
    }
    
    private string GetCurrentSnippetContent()
    {
        var activeSnippet = GetActiveSnippet();
        return activeSnippet?.Content ?? predefinedText;
    }
    
    private void UpdateActiveSnippet(string newContent)
    {
        var activeSnippet = GetActiveSnippet();
        if (activeSnippet != null)
        {
            activeSnippet.Content = newContent;
            activeSnippet.LastUsed = DateTime.Now;
        }
    }
    
    private string GenerateUniqueSnippetId()
    {
        int counter = snippets.Count + 1;
        string baseId = "snippet";
        string id = $"{baseId}{counter}";
        
        while (snippets.Any(s => s.Id == id))
        {
            counter++;
            id = $"{baseId}{counter}";
        }
        
        return id;
    }
    
    private string? ShowInputDialog(string prompt, string title, string defaultValue = "")
    {
        using var form = new Form();
        form.Text = title;
        form.Width = 400;
        form.Height = 150;
        form.StartPosition = FormStartPosition.CenterParent;
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        form.MaximizeBox = false;
        form.MinimizeBox = false;
        
        var label = new Label { Text = prompt, Left = 20, Top = 20, Width = 350, Height = 20 };
        var textBox = new TextBox { Text = defaultValue, Left = 20, Top = 45, Width = 350, Height = 20 };
        var buttonOK = new Button { Text = "OK", Left = 240, Top = 75, Width = 60, DialogResult = DialogResult.OK };
        var buttonCancel = new Button { Text = "Cancel", Left = 310, Top = 75, Width = 60, DialogResult = DialogResult.Cancel };
        
        form.Controls.AddRange(new Control[] { label, textBox, buttonOK, buttonCancel });
        form.AcceptButton = buttonOK;
        form.CancelButton = buttonCancel;
        
        return form.ShowDialog() == DialogResult.OK ? textBox.Text : null;
    }
    
    private void UpdateSnippetComboBox()
    {
        if (cmbSnippets == null) return;
        
        cmbSnippets.Items.Clear();
        foreach (var snippet in snippets.OrderBy(s => s.Name))
        {
            cmbSnippets.Items.Add(snippet);
        }
        
        // Select the active snippet
        var activeSnippet = GetActiveSnippet();
        if (activeSnippet != null)
        {
            cmbSnippets.SelectedItem = activeSnippet;
        }
        
        // Update button states
        if (btnDeleteSnippet != null)
        {
            btnDeleteSnippet.Enabled = snippets.Count > 1;
        }
    }
    
    private void UpdateSnippetTextBox()
    {
        if (txtPredefinedText == null) return;
        
        var activeSnippet = GetActiveSnippet();
        txtPredefinedText.Text = activeSnippet?.Content ?? "";
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
            contentToType = GetCurrentSnippetContent();
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
            UpdateActiveSnippet(txtPredefinedText.Text);
            SaveSettings();
            if (lblStatus != null)
            {
                var activeSnippet = GetActiveSnippet();
                lblStatus.Text = $"Status: Snippet '{activeSnippet?.Name ?? "Unknown"}' saved";
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
            if (cmbSnippets != null) cmbSnippets.Enabled = !fileSourceMode;
            if (btnNewSnippet != null) btnNewSnippet.Enabled = !fileSourceMode;
            if (btnDuplicateSnippet != null) btnDuplicateSnippet.Enabled = !fileSourceMode;
            if (btnRenameSnippet != null) btnRenameSnippet.Enabled = !fileSourceMode;
            if (btnDeleteSnippet != null) btnDeleteSnippet.Enabled = !fileSourceMode && snippets.Count > 1;
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

    private void CmbSnippets_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (sender is ComboBox cmb && cmb.SelectedItem is Snippet selectedSnippet)
        {
            activeSnippetId = selectedSnippet.Id;
            UpdateSnippetTextBox();
            SaveSettings();
        }
    }

    private void BtnNewSnippet_Click(object? sender, EventArgs e)
    {
        string? name = ShowInputDialog("Enter name for new snippet:", "New Snippet");
            
        if (string.IsNullOrWhiteSpace(name)) return;
        
        name = name.Trim();
        
        // Check for duplicate names (case-insensitive)
        if (snippets.Any(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show("A snippet with this name already exists. Please choose a different name.", 
                "Duplicate Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        var newSnippet = new Snippet
        {
            Id = GenerateUniqueSnippetId(),
            Name = name,
            Content = "Type your snippet content here...",
            LastUsed = DateTime.Now
        };
        
        snippets.Add(newSnippet);
        activeSnippetId = newSnippet.Id;
        
        UpdateSnippetComboBox();
        UpdateSnippetTextBox();
        SaveSettings();
        
        if (lblStatus != null)
        {
            lblStatus.Text = $"Status: Created new snippet '{name}'";
            lblStatus.ForeColor = Color.Green;
        }
    }

    private void BtnDuplicateSnippet_Click(object? sender, EventArgs e)
    {
        var currentSnippet = GetActiveSnippet();
        if (currentSnippet == null) return;
        
        string? name = ShowInputDialog("Enter name for copied snippet:", "Duplicate Snippet", currentSnippet.Name + " - Copy");
            
        if (string.IsNullOrWhiteSpace(name)) return;
        
        name = name.Trim();
        
        // Check for duplicate names (case-insensitive)
        if (snippets.Any(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show("A snippet with this name already exists. Please choose a different name.", 
                "Duplicate Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        var duplicatedSnippet = new Snippet
        {
            Id = GenerateUniqueSnippetId(),
            Name = name,
            Content = currentSnippet.Content,
            LastUsed = DateTime.Now
        };
        
        snippets.Add(duplicatedSnippet);
        activeSnippetId = duplicatedSnippet.Id;
        
        UpdateSnippetComboBox();
        UpdateSnippetTextBox();
        SaveSettings();
        
        if (lblStatus != null)
        {
            lblStatus.Text = $"Status: Duplicated snippet as '{name}'";
            lblStatus.ForeColor = Color.Green;
        }
    }

    private void BtnRenameSnippet_Click(object? sender, EventArgs e)
    {
        var currentSnippet = GetActiveSnippet();
        if (currentSnippet == null) return;
        
        string? name = ShowInputDialog("Enter new name for snippet:", "Rename Snippet", currentSnippet.Name);
            
        if (string.IsNullOrWhiteSpace(name)) return;
        
        name = name.Trim();
        
        // Check if name is unchanged
        if (name.Equals(currentSnippet.Name, StringComparison.OrdinalIgnoreCase))
        {
            return; // No change needed
        }
        
        // Check for duplicate names (case-insensitive)
        if (snippets.Any(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show("A snippet with this name already exists. Please choose a different name.", 
                "Duplicate Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        var oldName = currentSnippet.Name;
        currentSnippet.Name = name;
        
        UpdateSnippetComboBox();
        SaveSettings();
        
        if (lblStatus != null)
        {
            lblStatus.Text = $"Status: Renamed snippet from '{oldName}' to '{name}'";
            lblStatus.ForeColor = Color.Green;
        }
    }

    private void BtnDeleteSnippet_Click(object? sender, EventArgs e)
    {
        if (snippets.Count <= 1)
        {
            MessageBox.Show("Cannot delete the last remaining snippet.", 
                "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        var currentSnippet = GetActiveSnippet();
        if (currentSnippet == null) return;
        
        var result = MessageBox.Show($"Are you sure you want to delete the snippet '{currentSnippet.Name}'?", 
            "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
        if (result == DialogResult.Yes)
        {
            snippets.Remove(currentSnippet);
            
            // Select first remaining snippet
            activeSnippetId = snippets.First().Id;
            
            UpdateSnippetComboBox();
            UpdateSnippetTextBox();
            SaveSettings();
            
            if (lblStatus != null)
            {
                lblStatus.Text = $"Status: Deleted snippet '{currentSnippet.Name}'";
                lblStatus.ForeColor = Color.Green;
            }
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

// Snippet data model
public class Snippet
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime LastUsed { get; set; } = DateTime.Now;
    
    public override string ToString()
    {
        return Name;
    }
}

// Settings class for JSON serialization
public class AppSettings
{
    // Legacy property for backward compatibility
    public string PredefinedText { get; set; } = string.Empty;
    
    // New snippet support
    public List<Snippet> Snippets { get; set; } = new();
    public string ActiveSnippetId { get; set; } = string.Empty;
    
    public int TypingSpeed { get; set; } = 5;
    public bool HasCode { get; set; } = false;
    public int LastNonCodeSpeed { get; set; } = 10;
    public bool UseFileSource { get; set; } = false;
    public string FileSourcePath { get; set; } = string.Empty;
}
