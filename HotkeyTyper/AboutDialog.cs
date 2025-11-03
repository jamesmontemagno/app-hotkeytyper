namespace HotkeyTyper;

/// <summary>
/// About dialog that respects system dark mode
/// </summary>
internal class AboutDialog : Form
{
    public AboutDialog(string version)
    {
        InitializeDialog(version);
    }

    private void InitializeDialog(string version)
    {
        Text = "About Hotkey Typer";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(400, 200);

        // Apply dark mode colors
        BackColor = AppColors.Background;
        ForeColor = AppColors.Text;

        Label lblTitle = new Label
        {
            Text = "Hotkey Typer",
            Location = new Point(20, 20),
            Size = new Size(360, 30),
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = AppColors.Text
        };

        Label lblVersion = new Label
        {
            Text = $"Version {version}",
            Location = new Point(20, 55),
            Size = new Size(360, 20),
            Font = new Font("Segoe UI", 10F),
            ForeColor = AppColors.TextSecondary
        };

        Label lblDescription = new Label
        {
            Text = "A simple utility to type predefined text snippets\nusing a configurable global hotkey.",
            Location = new Point(20, 85),
            Size = new Size(360, 40),
            Font = new Font("Segoe UI", 9F),
            ForeColor = AppColors.TextSecondary
        };

        Label lblCopyright = new Label
        {
            Text = $"� {DateTime.Now.Year} James Montemagno",
            Location = new Point(20, 130),
            Size = new Size(360, 20),
            Font = new Font("Segoe UI", 9F),
            ForeColor = AppColors.TextTertiary
        };

        Button btnOK = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(305, 160),
            Size = new Size(75, 25),
            Font = new Font("Segoe UI", 9F),
            FlatStyle = AppColors.IsDarkMode ? FlatStyle.Flat : FlatStyle.Standard
        };

        if (AppColors.IsDarkMode)
        {
            btnOK.BackColor = AppColors.ButtonBackground;
            btnOK.ForeColor = AppColors.Text;
            btnOK.FlatAppearance.BorderColor = AppColors.Border;
        }

        Controls.AddRange(new Control[] { lblTitle, lblVersion, lblDescription, lblCopyright, btnOK });
        AcceptButton = btnOK;
        CancelButton = btnOK;
    }

    /// <summary>
    /// Shows the about dialog
    /// </summary>
    public static void Show(IWin32Window owner, string version)
    {
        using AboutDialog dialog = new AboutDialog(version);
        dialog.ShowDialog(owner);
    }
}
