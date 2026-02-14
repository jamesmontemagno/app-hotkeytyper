namespace HotkeyTyper;

/// <summary>
/// Custom message box that respects the application's dark mode theme.
/// Use this instead of built-in MessageBox.Show throughout the application.
/// </summary>
internal class ThemedMessageBox : Form
{
    private Label lblMessage = null!;
    private PictureBox? picIcon;
    private Button? btnPrimary;
    private Button? btnSecondary;
    private Button? btnThird;

    private ThemedMessageBox(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        InitializeDialog(message, title, buttons, icon);
    }

    private void InitializeDialog(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        Text = title;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;

        // Apply dark mode colors
        BackColor = AppColors.Background;
        ForeColor = AppColors.Text;

        // Icon
        int iconSize = 32;
        int leftMargin = 20;
        int topMargin = 20;
        
        // Set icon based on MessageBoxIcon
        Icon? systemIcon = icon switch
        {
            MessageBoxIcon.Error => SystemIcons.Error,
            MessageBoxIcon.Question => SystemIcons.Question,
            MessageBoxIcon.Warning => SystemIcons.Warning,
            MessageBoxIcon.Information => SystemIcons.Information,
            _ => null
        };

        if (systemIcon != null)
        {
            picIcon = new PictureBox
            {
                Location = new Point(leftMargin, topMargin),
                Size = new Size(iconSize, iconSize),
                SizeMode = PictureBoxSizeMode.CenterImage,
                Image = systemIcon.ToBitmap()
            };
        }

        // Message label
        int messagePadding = 20;
        int messageLeft = leftMargin + (systemIcon != null ? iconSize + 15 : 0);
        
        lblMessage = new Label
        {
            Text = message,
            Location = new Point(messageLeft, topMargin),
            AutoSize = true,
            MaximumSize = new Size(400, 0),
            Font = new Font("Segoe UI", 9F),
            ForeColor = AppColors.Text
        };

        Controls.Add(lblMessage);
        if (picIcon != null)
        {
            Controls.Add(picIcon);
        }

        // Measure message to determine form size
        using (Graphics g = CreateGraphics())
        {
            SizeF textSize = g.MeasureString(message, lblMessage.Font, lblMessage.MaximumSize.Width);
            lblMessage.Size = new Size((int)Math.Ceiling(textSize.Width), (int)Math.Ceiling(textSize.Height));
        }

        // Calculate form dimensions
        int formWidth = Math.Max(340, messageLeft + lblMessage.Width + messagePadding);
        int messageBottom = topMargin + Math.Max(lblMessage.Height, iconSize);
        int buttonTop = messageBottom + 20;
        int buttonHeight = 36;
        int buttonWidth = 84;
        int buttonSpacing = 10;
        int formHeight = buttonTop + buttonHeight + 20;

        ClientSize = new Size(formWidth, formHeight);

        // Add buttons based on MessageBoxButtons
        int buttonCount = 0;
        switch (buttons)
        {
            case MessageBoxButtons.OK:
                btnPrimary = CreateButton("OK", DialogResult.OK, buttonCount++);
                AcceptButton = btnPrimary;
                CancelButton = btnPrimary;
                break;

            case MessageBoxButtons.OKCancel:
                btnSecondary = CreateButton("Cancel", DialogResult.Cancel, buttonCount++);
                btnPrimary = CreateButton("OK", DialogResult.OK, buttonCount++);
                AcceptButton = btnPrimary;
                CancelButton = btnSecondary;
                break;

            case MessageBoxButtons.YesNo:
                btnSecondary = CreateButton("No", DialogResult.No, buttonCount++);
                btnPrimary = CreateButton("Yes", DialogResult.Yes, buttonCount++);
                AcceptButton = btnPrimary;
                CancelButton = btnSecondary;
                break;

            case MessageBoxButtons.YesNoCancel:
                btnThird = CreateButton("Cancel", DialogResult.Cancel, buttonCount++);
                btnSecondary = CreateButton("No", DialogResult.No, buttonCount++);
                btnPrimary = CreateButton("Yes", DialogResult.Yes, buttonCount++);
                AcceptButton = btnPrimary;
                CancelButton = btnThird;
                break;

            case MessageBoxButtons.RetryCancel:
                btnSecondary = CreateButton("Cancel", DialogResult.Cancel, buttonCount++);
                btnPrimary = CreateButton("Retry", DialogResult.Retry, buttonCount++);
                AcceptButton = btnPrimary;
                CancelButton = btnSecondary;
                break;

            case MessageBoxButtons.AbortRetryIgnore:
                btnThird = CreateButton("Ignore", DialogResult.Ignore, buttonCount++);
                btnSecondary = CreateButton("Retry", DialogResult.Retry, buttonCount++);
                btnPrimary = CreateButton("Abort", DialogResult.Abort, buttonCount++);
                AcceptButton = btnPrimary;
                break;
        }

        // Position buttons (right-aligned, from right to left)
        int currentRight = formWidth - 20;
        if (btnPrimary != null)
        {
            btnPrimary.Location = new Point(currentRight - buttonWidth, buttonTop);
            currentRight -= buttonWidth + buttonSpacing;
        }
        if (btnSecondary != null)
        {
            btnSecondary.Location = new Point(currentRight - buttonWidth, buttonTop);
            currentRight -= buttonWidth + buttonSpacing;
        }
        if (btnThird != null)
        {
            btnThird.Location = new Point(currentRight - buttonWidth, buttonTop);
        }
    }

    private Button CreateButton(string text, DialogResult result, int index)
    {
        Button button = new Button
        {
            Text = text,
            DialogResult = result,
            Size = new Size(84, 36),
            Font = new Font("Segoe UI", 9F),
            FlatStyle = AppColors.IsDarkMode ? FlatStyle.Flat : FlatStyle.Standard,
            TabIndex = index
        };

        if (AppColors.IsDarkMode)
        {
            // Primary button (first/rightmost) gets accent color
            if (index == 0)
            {
                button.BackColor = AppColors.Accent;
                button.ForeColor = AppColors.AccentText;
                button.FlatAppearance.BorderColor = AppColors.AccentDark;
            }
            else
            {
                button.BackColor = AppColors.ButtonBackground;
                button.ForeColor = AppColors.Text;
                button.FlatAppearance.BorderColor = AppColors.Border;
            }
        }

        Controls.Add(button);
        return button;
    }

    /// <summary>
    /// Displays a themed message box with the specified message and title.
    /// </summary>
    public static DialogResult Show(string message, string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
    {
        using ThemedMessageBox dialog = new ThemedMessageBox(message, title, buttons, icon);
        return dialog.ShowDialog();
    }

    /// <summary>
    /// Displays a themed message box with the specified message, title, and owner.
    /// </summary>
    public static DialogResult Show(IWin32Window owner, string message, string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
    {
        using ThemedMessageBox dialog = new ThemedMessageBox(message, title, buttons, icon);
        return dialog.ShowDialog(owner);
    }
}
