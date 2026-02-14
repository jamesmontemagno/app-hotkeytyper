namespace HotkeyTyper;

/// <summary>
/// Dialog for configuring the global hotkey with dark mode support
/// </summary>
internal class HotkeyConfigDialog : Form
{
    private readonly TextBox txtHotkey;
    private readonly Button btnOK;
    private readonly Button btnCancel;
    private readonly Button btnClear;
    private readonly Label lblInstructions;
    
    private string? capturedHotkey;
    private int capturedModifiers;
    private Keys capturedKey = Keys.None;
    
    public string? HotkeyString => capturedHotkey;
    public int Modifiers => capturedModifiers;
    public Keys Key => capturedKey;
    
    private const int MOD_CONTROL = 0x0002;
    private const int MOD_SHIFT = 0x0004;
    private const int MOD_ALT = 0x0001;
    
    public HotkeyConfigDialog(string currentHotkey)
    {
        Text = "Configure Hotkey";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(450, 230);
        
        // Apply dark mode colors
        BackColor = AppColors.Background;
        ForeColor = AppColors.Text;
        
        lblInstructions = new Label
        {
            Text = "Click in the text box below and press your desired key combination.\nAt least one modifier (Ctrl, Shift, Alt) is required.",
            Location = new Point(24, 20),
            Size = new Size(400, 42),
            Font = new Font("Segoe UI", 9F),
            ForeColor = AppColors.TextSecondary
        };
        
        txtHotkey = new TextBox
        {
            Text = currentHotkey,
            Location = new Point(24, 72),
            Size = new Size(400, 26),
            Font = new Font("Segoe UI", 10F),
            ReadOnly = true,
            BackColor = AppColors.InputBackground,
            ForeColor = AppColors.InputText,
            BorderStyle = AppColors.IsDarkMode ? BorderStyle.FixedSingle : BorderStyle.Fixed3D
        };
        txtHotkey.KeyDown += TxtHotkey_KeyDown;
        
        // Initialize captured values from current hotkey
        capturedHotkey = string.IsNullOrEmpty(currentHotkey) ? null : currentHotkey;
        
        btnClear = new Button
        {
            Text = "Clear",
            Location = new Point(24, 112),
            Size = new Size(84, 36),
            Font = new Font("Segoe UI", 9F),
            FlatStyle = AppColors.IsDarkMode ? FlatStyle.Flat : FlatStyle.Standard
        };
        btnClear.Click += BtnClear_Click;
        
        btnOK = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(270, 180),
            Size = new Size(84, 36),
            Font = new Font("Segoe UI", 9F),
            FlatStyle = AppColors.IsDarkMode ? FlatStyle.Flat : FlatStyle.Standard
        };
        
        btnCancel = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Location = new Point(360, 180),
            Size = new Size(84, 36),
            Font = new Font("Segoe UI", 9F),
            FlatStyle = AppColors.IsDarkMode ? FlatStyle.Flat : FlatStyle.Standard
        };
        
        if (AppColors.IsDarkMode)
        {
            btnOK.BackColor = AppColors.Accent;
            btnOK.ForeColor = AppColors.AccentText;
            btnOK.FlatAppearance.BorderColor = AppColors.AccentDark;
            
            btnCancel.BackColor = AppColors.ButtonBackground;
            btnCancel.ForeColor = AppColors.Text;
            btnCancel.FlatAppearance.BorderColor = AppColors.Border;
            
            btnClear.BackColor = AppColors.ButtonBackground;
            btnClear.ForeColor = AppColors.Text;
            btnClear.FlatAppearance.BorderColor = AppColors.Border;
        }
        
        Controls.AddRange(new Control[] { lblInstructions, txtHotkey, btnClear, btnOK, btnCancel });
        AcceptButton = btnOK;
        CancelButton = btnCancel;
    }
    
    private void TxtHotkey_KeyDown(object? sender, KeyEventArgs e)
    {
        e.SuppressKeyPress = true;
        e.Handled = true;
        
        // Ignore modifier-only presses
        if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey || 
            e.KeyCode == Keys.Menu || e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
        {
            return;
        }
        
        // Build modifier flags
        int mods = 0;
        if (e.Control) mods |= MOD_CONTROL;
        if (e.Shift) mods |= MOD_SHIFT;
        if (e.Alt) mods |= MOD_ALT;
        
        // Validate - must have at least one modifier
        if (mods == 0)
        {
            lblInstructions.Text = "At least one modifier (Ctrl, Shift, Alt) is required.";
            lblInstructions.ForeColor = AppColors.Error;
            return;
        }
        
        // Format and display
        capturedModifiers = mods;
        capturedKey = e.KeyCode;
        capturedHotkey = FormatHotkey(mods, e.KeyCode);
        txtHotkey.Text = capturedHotkey;
        
        // Reset instructions
        lblInstructions.Text = "Click in the text box below and press your desired key combination.\nAt least one modifier (Ctrl, Shift, Alt) is required.";
        lblInstructions.ForeColor = AppColors.TextSecondary;
    }
    
    private void BtnClear_Click(object? sender, EventArgs e)
    {
        txtHotkey.Text = string.Empty;
        capturedHotkey = null;
        capturedModifiers = 0;
        capturedKey = Keys.None;
    }
    
    private string FormatHotkey(int modifiers, Keys key)
    {
        List<string> parts = new();
        
        if ((modifiers & MOD_CONTROL) != 0)
            parts.Add("Ctrl");
        if ((modifiers & MOD_SHIFT) != 0)
            parts.Add("Shift");
        if ((modifiers & MOD_ALT) != 0)
            parts.Add("Alt");
        
        parts.Add(key.ToString());
        
        return string.Join("+", parts);
    }
    
    /// <summary>
    /// Shows the hotkey configuration dialog
    /// </summary>
    /// <returns>DialogResult indicating if user confirmed the change</returns>
    public static DialogResult Show(IWin32Window owner, string currentHotkey, out string? newHotkey, out int modifiers, out Keys key)
    {
        using var dialog = new HotkeyConfigDialog(currentHotkey);
        var result = dialog.ShowDialog(owner);
        
        newHotkey = dialog.HotkeyString;
        modifiers = dialog.Modifiers;
        key = dialog.Key;
        
        return result;
    }
}
