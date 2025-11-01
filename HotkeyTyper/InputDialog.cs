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
        ClientSize = new Size(350, 120);
        
        var lblPrompt = new Label
        {
            Text = prompt,
            Location = new Point(15, 15),
            Size = new Size(320, 20),
            Font = new Font("Segoe UI", 9F)
        };
        
        txtInput = new TextBox
        {
            Text = defaultValue,
            Location = new Point(15, 45),
            Size = new Size(320, 23),
            Font = new Font("Segoe UI", 9F)
        };
        
        btnOK = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(175, 80),
            Size = new Size(75, 25),
            Font = new Font("Segoe UI", 9F)
        };
        
        btnCancel = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Location = new Point(260, 80),
            Size = new Size(75, 25),
            Font = new Font("Segoe UI", 9F)
        };
        
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
