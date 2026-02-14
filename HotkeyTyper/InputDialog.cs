namespace HotkeyTyper;

/// <summary>
/// Simple input dialog for getting text input from user
/// </summary>
internal class InputDialog : Form
{
    private readonly TextBox txtInput;
    private readonly Button btnOK;
    private readonly Button btnCancel;
    
    public string InputText => txtInput.Text;
    
    public InputDialog(string prompt, string title, string defaultValue = "")
    {
        Text = title;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(420, 150);
        
        // Apply dark mode colors
        BackColor = AppColors.Background;
        ForeColor = AppColors.Text;
        
        var lblPrompt = new Label
        {
            Text = prompt,
            Location = new Point(20, 20),
            Size = new Size(380, 22),
            Font = new Font("Segoe UI", 9F),
            ForeColor = AppColors.Text
        };
        
        txtInput = new TextBox
        {
            Text = defaultValue,
            Location = new Point(20, 52),
            Size = new Size(380, 26),
            Font = new Font("Segoe UI", 9F),
            BackColor = AppColors.InputBackground,
            ForeColor = AppColors.InputText,
            BorderStyle = AppColors.IsDarkMode ? BorderStyle.FixedSingle : BorderStyle.Fixed3D
        };
        
        btnOK = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(240, 100),
            Size = new Size(84, 36),
            Font = new Font("Segoe UI", 9F),
            FlatStyle = AppColors.IsDarkMode ? FlatStyle.Flat : FlatStyle.Standard
        };
        
        btnCancel = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Location = new Point(330, 100),
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
        }
        
        Controls.AddRange(new Control[] { lblPrompt, txtInput, btnOK, btnCancel });
        AcceptButton = btnOK;
        CancelButton = btnCancel;
        
        // Select all text when dialog opens
        Load += (s, e) => txtInput.SelectAll();
    }
    
    /// <summary>
    /// Shows the input dialog and returns the entered text, or null if cancelled
    /// </summary>
    public static string? Show(string prompt, string title, string defaultValue = "")
    {
        using var dialog = new InputDialog(prompt, title, defaultValue);
        return dialog.ShowDialog() == DialogResult.OK ? dialog.InputText : null;
    }
}
