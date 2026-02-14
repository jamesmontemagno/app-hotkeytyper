using Microsoft.Web.WebView2.WinForms;
using Markdig;

namespace HotkeyTyper;

/// <summary>
/// Update notification dialog that respects system dark mode and renders Markdown
/// </summary>
internal class UpdateDialog : Form
{
    private WebView2? webViewChangelog;
    private readonly string markdownContent;

    public UpdateDialog(string latestVersion, string changelog)
    {
        markdownContent = changelog ?? string.Empty;
        InitializeDialog(latestVersion);
        // Load event will handle async WebView2 initialization
        this.Load += async (s, e) => await InitializeWebViewAsync();
    }

    private void InitializeDialog(string latestVersion)
    {
        Text = "Update Available";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(600, 450);

        // Apply dark mode colors
        BackColor = AppColors.Background;
        ForeColor = AppColors.Text;

        Label lblTitle = new Label
        {
            Text = $"A new version ({latestVersion}) is available!",
            Location = new Point(20, 20),
            Size = new Size(560, 25),
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = AppColors.Text
        };

        Label lblPrompt = new Label
        {
            Text = "Would you like to download and install it?",
            Location = new Point(20, 50),
            Size = new Size(560, 20),
            Font = new Font("Segoe UI", 9F),
            ForeColor = AppColors.TextSecondary
        };

        Label lblChangelogTitle = new Label
        {
            Text = "Recent Changes:",
            Location = new Point(20, 80),
            Size = new Size(560, 20),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = AppColors.TextSecondary
        };

        // Create WebView2 control for Markdown rendering
        webViewChangelog = new WebView2
        {
            Location = new Point(20, 105),
            Size = new Size(560, 290),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };

        Button btnYes = new Button
        {
            Text = "Yes, Update",
            DialogResult = DialogResult.Yes,
            Location = new Point(400, 410),
            Size = new Size(100, 30),
            Font = new Font("Segoe UI", 9F),
            FlatStyle = AppColors.IsDarkMode ? FlatStyle.Flat : FlatStyle.Standard
        };

        Button btnNo = new Button
        {
            Text = "Not Now",
            DialogResult = DialogResult.No,
            Location = new Point(510, 410),
            Size = new Size(75, 30),
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

        Controls.AddRange(new Control[] { lblTitle, lblPrompt, lblChangelogTitle, webViewChangelog, btnYes, btnNo });
        AcceptButton = btnYes;
        CancelButton = btnNo;
    }

    private async Task InitializeWebViewAsync()
    {
        try
        {
            if (webViewChangelog == null) return;

            // Ensure WebView2 runtime is initialized
            await webViewChangelog.EnsureCoreWebView2Async(null);

            // Convert Markdown to HTML
            var html = GenerateHtmlFromMarkdown(markdownContent);

            // Load the HTML content
            webViewChangelog.NavigateToString(html);
        }
        catch (Exception ex)
        {
            // Fallback: Replace WebView2 with a TextBox showing the error and raw content
            if (webViewChangelog != null)
            {
                var fallbackTextBox = new TextBox
                {
                    Text = $"Failed to render release notes: {ex.Message}\n\n{markdownContent}",
                    Location = webViewChangelog.Location,
                    Size = webViewChangelog.Size,
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    Font = new Font("Segoe UI", 9F),
                    BackColor = AppColors.InputBackground,
                    ForeColor = AppColors.InputText,
                    Anchor = webViewChangelog.Anchor
                };
                Controls.Remove(webViewChangelog);
                Controls.Add(fallbackTextBox);
            }
        }
    }

    private string GenerateHtmlFromMarkdown(string markdown)
    {
        // Convert Markdown to HTML using Markdig
        var htmlBody = Markdown.ToHtml(markdown ?? string.Empty);

        // Determine theme colors
        var bgColor = AppColors.IsDarkMode ? "#1e1e1e" : "#ffffff";
        var textColor = AppColors.IsDarkMode ? "#e0e0e0" : "#000000";
        var linkColor = AppColors.IsDarkMode ? "#4da6ff" : "#0066cc";
        var codeBlockBg = AppColors.IsDarkMode ? "#2d2d2d" : "#f5f5f5";
        var codeBorder = AppColors.IsDarkMode ? "#404040" : "#dddddd";
        var headingColor = AppColors.IsDarkMode ? "#ffffff" : "#000000";

        // Generate complete HTML document with styles
        return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            font-size: 14px;
            line-height: 1.6;
            color: {textColor};
            background-color: {bgColor};
            padding: 12px;
            margin: 0;
        }}
        h1, h2, h3, h4, h5, h6 {{
            color: {headingColor};
            margin-top: 16px;
            margin-bottom: 8px;
            font-weight: 600;
        }}
        h1 {{ font-size: 1.8em; }}
        h2 {{ font-size: 1.5em; }}
        h3 {{ font-size: 1.3em; }}
        h4 {{ font-size: 1.1em; }}
        p {{
            margin: 8px 0;
        }}
        ul, ol {{
            margin: 8px 0;
            padding-left: 24px;
        }}
        li {{
            margin: 4px 0;
        }}
        code {{
            background-color: {codeBlockBg};
            border: 1px solid {codeBorder};
            border-radius: 3px;
            padding: 2px 5px;
            font-family: 'Consolas', 'Courier New', monospace;
            font-size: 0.9em;
        }}
        pre {{
            background-color: {codeBlockBg};
            border: 1px solid {codeBorder};
            border-radius: 5px;
            padding: 10px;
            overflow-x: auto;
        }}
        pre code {{
            background: none;
            border: none;
            padding: 0;
        }}
        a {{
            color: {linkColor};
            text-decoration: none;
        }}
        a:hover {{
            text-decoration: underline;
        }}
        blockquote {{
            border-left: 4px solid {codeBorder};
            margin: 8px 0;
            padding-left: 12px;
            color: {textColor};
            opacity: 0.8;
        }}
        hr {{
            border: none;
            border-top: 1px solid {codeBorder};
            margin: 16px 0;
        }}
        table {{
            border-collapse: collapse;
            width: 100%;
            margin: 8px 0;
        }}
        th, td {{
            border: 1px solid {codeBorder};
            padding: 8px;
            text-align: left;
        }}
        th {{
            background-color: {codeBlockBg};
            font-weight: 600;
        }}
    </style>
</head>
<body>
{htmlBody}
</body>
</html>";
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
