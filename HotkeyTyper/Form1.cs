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
    
    // Predefined text to type
    private string predefinedText = "Hello, this is my predefined text!";
    
    // Typing speed (1 = slowest, 10 = fastest)
    private int typingSpeed = 5;
    
    // Cancellation for in-progress typing
    private CancellationTokenSource? typingCts;
    private readonly KeyboardInputSender inputSender = new();
    
    // Code mode flag (limits maximum speed)
    private bool hasCodeMode = false;
    
    // Reliability heuristics for high-speed typing
    private const int MinFastDelayMs = 35; // Minimum delay enforced when speed >= 8
    private const int ContextualPauseMs = 140; // Extra pause after space following certain boundary chars
    private static readonly char[] ContextBoundaryChars = new[] { '>', ')', '}', ']' };
    
    // System tray components
    private NotifyIcon? trayIcon;
    private ContextMenuStrip? trayMenu;
    
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
        
        // Register CTRL+SHIFT+1 as global hotkey
        RegisterHotKey(this.Handle, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, (int)Keys.D1);
        
        // Update UI after form is fully loaded
        this.Load += Form1_Load;
    }
    
    private void Form1_Load(object? sender, EventArgs e)
    {
        UpdateUIFromSettings();
    }
    
    private void UpdateUIFromSettings()
    {
        // Update text box with loaded predefined text
        if (txtPredefinedText != null)
        {
            txtPredefinedText.Text = predefinedText;
        }
        
        // Update typing speed slider with loaded value
        if (sliderTypingSpeed != null)
        {
            sliderTypingSpeed.Maximum = hasCodeMode ? 8 : 10;
            if (typingSpeed > sliderTypingSpeed.Maximum)
            {
                typingSpeed = sliderTypingSpeed.Maximum;
            }
            sliderTypingSpeed.Value = typingSpeed;
        }
        
        // Update speed indicator label
        if (lblSpeedIndicator != null)
        {
            lblSpeedIndicator.Text = GetSpeedText(typingSpeed);
        }
        if (chkHasCode != null)
        {
            chkHasCode.Checked = hasCodeMode;
        }
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
                    if (!string.IsNullOrEmpty(settings.PredefinedText))
                    {
                        predefinedText = settings.PredefinedText;
                    }
                    if (settings.TypingSpeed > 0 && settings.TypingSpeed <= 10)
                    {
                        typingSpeed = settings.TypingSpeed;
                    }
                    hasCodeMode = settings.HasCode;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading settings: {ex.Message}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
    
    private void SaveSettings()
    {
        try
        {
            var settings = new AppSettings { PredefinedText = predefinedText, TypingSpeed = typingSpeed, HasCode = hasCodeMode };
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
        trayIcon = new NotifyIcon()
        {
            Text = "Hotkey Typer - CTRL+SHIFT+1 Active",
            Visible = true,
            Icon = SystemIcons.Application
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
        _ = TypePredefinedTextAsync(token);
    }

    private async Task TypePredefinedTextAsync(CancellationToken token)
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
            for (int index = 0; index < predefinedText.Length; index++)
            {
                if (token.IsCancellationRequested) break;
                char c = predefinedText[index];

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
        if (txtPredefinedText != null)
        {
            predefinedText = txtPredefinedText.Text;
            SaveSettings();
            MessageBox.Show("Text updated and saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                lblSpeedIndicator.Text = GetSpeedText(typingSpeed);
            }
            SaveSettings();
        }
    }

    private void ChkHasCode_CheckedChanged(object? sender, EventArgs e)
    {
        if (sender is CheckBox cb)
        {
            hasCodeMode = cb.Checked;
            if (sliderTypingSpeed != null)
            {
                sliderTypingSpeed.Maximum = hasCodeMode ? 8 : 10;
                if (sliderTypingSpeed.Value > sliderTypingSpeed.Maximum)
                {
                    sliderTypingSpeed.Value = sliderTypingSpeed.Maximum;
                }
            }
            if (lblSpeedIndicator != null)
            {
                lblSpeedIndicator.Text = GetSpeedText(typingSpeed);
            }
            SaveSettings();
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
        
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }
}

// Settings class for JSON serialization
public class AppSettings
{
    public string PredefinedText { get; set; } = string.Empty;
    public int TypingSpeed { get; set; } = 5;
    public bool HasCode { get; set; } = false;
}
