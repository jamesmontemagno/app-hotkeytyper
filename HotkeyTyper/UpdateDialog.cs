namespace HotkeyTyper;

/// <summary>
/// Update notification dialog that respects system dark mode
/// </summary>
internal class UpdateDialog : Form
{
    private TextBox txtChangelog;

    public UpdateDialog(string latestVersion, string changelog)
    {
        InitializeDialog(latestVersion, changelog);
    }

    private void InitializeDialog(string latestVersion, string changelog)
    {
        Text = "Update Available";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(500, 350);

        // Apply dark mode colors
        BackColor = AppColors.Background;
        ForeColor = AppColors.Text;

        Label lblTitle = new Label
        {
            Text = $"A new version ({latestVersion}) is available!",
            Location = new Point(20, 20),
            Size = new Size(460, 25),
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = AppColors.Text
        };

        Label lblPrompt = new Label
        {
            Text = "Would you like to download and install it?",
            Location = new Point(20, 50),
            Size = new Size(460, 20),
            Font = new Font("Segoe UI", 9F),
            ForeColor = AppColors.TextSecondary
        };

        Label lblChangelogTitle = new Label
        {
            Text = "Recent Changes:",
            Location = new Point(20, 80),
            Size = new Size(460, 20),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = AppColors.TextSecondary
        };

        txtChangelog = new TextBox
        {
            Text = changelog,
            Location = new Point(20, 105),
            Size = new Size(460, 190),
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Segoe UI", 9F),
            BackColor = AppColors.InputBackground,
            ForeColor = AppColors.InputText,
            BorderStyle = AppColors.IsDarkMode ? BorderStyle.FixedSingle : BorderStyle.Fixed3D
        };

        Button btnYes = new Button
        {
            Text = "Yes, Update",
            DialogResult = DialogResult.Yes,
            Location = new Point(320, 310),
            Size = new Size(80, 25),
            Font = new Font("Segoe UI", 9F),
            FlatStyle = AppColors.IsDarkMode ? FlatStyle.Flat : FlatStyle.Standard
        };

        Button btnNo = new Button
        {
            Text = "Not Now",
            DialogResult = DialogResult.No,
            Location = new Point(410, 310),
            Size = new Size(70, 25),
            Font = new Font("Segoe UI", 9F),
            FlatStyle = AppColors.IsDarkMode ? FlatStyle.Flat : FlatStyle.Standard
        };

        if (AppColors.IsDarkMode)
        {
            btnYes.BackColor = AppColors.Accent;
            btnYes.ForeColor = AppColors.AccentText;
            btnYes.FlatAppearance.BorderColor = AppColors.AccentDark;

            btnNo.BackColor = AppColors.ButtonBackground;
            btnNo.ForeColor = AppColors.Text;
            btnNo.FlatAppearance.BorderColor = AppColors.Border;
        }

        Controls.AddRange(new Control[] { lblTitle, lblPrompt, lblChangelogTitle, txtChangelog, btnYes, btnNo });
        AcceptButton = btnYes;
        CancelButton = btnNo;
    }

    /// <summary>
    /// Shows the update dialog and returns true if user wants to update
    /// </summary>
    public static bool Show(IWin32Window owner, string latestVersion, string changelog)
    {
        using UpdateDialog dialog = new UpdateDialog(latestVersion, changelog);
        return dialog.ShowDialog(owner) == DialogResult.Yes;
    }
}
