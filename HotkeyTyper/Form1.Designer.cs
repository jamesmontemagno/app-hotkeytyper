namespace HotkeyTyper;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    // Dispose method is implemented in Form1.cs

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        // 1. Instantiate controls first
        MenuStrip menuStrip = new MenuStrip();
        ToolStripMenuItem mnuHelp = new ToolStripMenuItem();
        ToolStripMenuItem mnuCheckForUpdates = new ToolStripMenuItem();
        ToolStripMenuItem mnuAbout = new ToolStripMenuItem();
        ToolStripMenuItem mnuTheme = new ToolStripMenuItem();
        mnuThemeSystem = new ToolStripMenuItem();
        mnuThemeLight = new ToolStripMenuItem();
        mnuThemeDark = new ToolStripMenuItem();
        TableLayoutPanel mainLayout = new TableLayoutPanel();
        Label lblInstructions = new Label();
        TableLayoutPanel snippetSection = new TableLayoutPanel();
        Label lblSnippet = new Label();
        cmbSnippets = new ComboBox();
        FlowLayoutPanel buttonFlow = new FlowLayoutPanel();
        Button btnNewSnippet = new Button();
        Button btnDuplicateSnippet = new Button();
        Button btnRenameSnippet = new Button();
        Button btnDeleteSnippet = new Button();
        txtPredefinedText = new TextBox();
        TableLayoutPanel speedSection = new TableLayoutPanel();
        Label lblTypingSpeed = new Label();
        sliderTypingSpeed = new LimitedTrackBar();
        lblSpeedIndicator = new Label();
        TableLayoutPanel optionsSection = new TableLayoutPanel();
        chkHasCode = new CheckBox();
        chkUseFile = new CheckBox();
        TableLayoutPanel fileSection = new TableLayoutPanel();
        txtFilePath = new TextBox();
        btnBrowseFile = new Button();
        Panel spacer = new Panel();
        FlowLayoutPanel actionSection = new FlowLayoutPanel();
        Button btnUpdate = new Button();
        Button btnMinimize = new Button();
        btnStop = new Button();
        lblStatus = new Label();

        // 2. Components
        components = new System.ComponentModel.Container();

        // 3. Suspend layout
        menuStrip.SuspendLayout();
        mainLayout.SuspendLayout();
        snippetSection.SuspendLayout();
        buttonFlow.SuspendLayout();
        speedSection.SuspendLayout();
        optionsSection.SuspendLayout();
        fileSection.SuspendLayout();
        actionSection.SuspendLayout();
        SuspendLayout();

        // 4. Configure MenuStrip
        menuStrip.Name = "menuStrip";
        menuStrip.Size = new Size(520, 24);
        menuStrip.TabIndex = 0;

        // Configure menu items
        mnuHelp.Name = "mnuHelp";
        mnuHelp.Text = "&Help";
        mnuHelp.DropDownItems.Add(mnuCheckForUpdates);
        mnuHelp.DropDownItems.Add(new ToolStripSeparator());
        mnuHelp.DropDownItems.Add(mnuTheme);
        mnuHelp.DropDownItems.Add(new ToolStripSeparator());
        mnuHelp.DropDownItems.Add(mnuAbout);

        mnuCheckForUpdates.Name = "mnuCheckForUpdates";
        mnuCheckForUpdates.Text = "Check for &Updates...";
        mnuCheckForUpdates.Click += MnuCheckForUpdates_Click;

        mnuTheme.Name = "mnuTheme";
        mnuTheme.Text = "&Theme";
        mnuTheme.DropDownItems.Add(mnuThemeSystem);
        mnuTheme.DropDownItems.Add(mnuThemeLight);
        mnuTheme.DropDownItems.Add(mnuThemeDark);

        mnuThemeSystem.Name = "mnuThemeSystem";
        mnuThemeSystem.Text = "&System";
        mnuThemeSystem.Click += MnuThemeSystem_Click;

        mnuThemeLight.Name = "mnuThemeLight";
        mnuThemeLight.Text = "&Light";
        mnuThemeLight.Click += MnuThemeLight_Click;

        mnuThemeDark.Name = "mnuThemeDark";
        mnuThemeDark.Text = "&Dark";
        mnuThemeDark.Click += MnuThemeDark_Click;

        mnuAbout.Name = "mnuAbout";
        mnuAbout.Text = "&About...";
        mnuAbout.Click += MnuAbout_Click;

        menuStrip.Items.Add(mnuHelp);

        // Configure main layout
        mainLayout.ColumnCount = 1;
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        mainLayout.Dock = DockStyle.Fill;
        mainLayout.Location = new Point(0, 24);
        mainLayout.Name = "mainLayout";
        mainLayout.Padding = new Padding(10);
        mainLayout.RowCount = 8;
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayout.Size = new Size(520, 616);
        mainLayout.TabIndex = 1;

        // Row 0: Instructions
        lblInstructions.AutoSize = true;
        lblInstructions.Anchor = AnchorStyles.Left | AnchorStyles.Top;
        lblInstructions.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        lblInstructions.Margin = new Padding(3);
        lblInstructions.Text = "Select or manage snippets below.\nPress CTRL+SHIFT+1 anywhere to type active snippet:";
        mainLayout.Controls.Add(lblInstructions, 0, 0);

        // Row 1: Snippet section
        snippetSection.AutoSize = true;
        snippetSection.ColumnCount = 2;
        snippetSection.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        snippetSection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        snippetSection.Dock = DockStyle.Fill;
        snippetSection.Margin = new Padding(3);
        snippetSection.RowCount = 2;
        snippetSection.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        snippetSection.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        lblSnippet.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        lblSnippet.Font = new Font("Segoe UI", 9F);
        lblSnippet.Margin = new Padding(3);
        lblSnippet.Text = "Snippet:";
        snippetSection.Controls.Add(lblSnippet, 0, 0);

        cmbSnippets.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        cmbSnippets.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbSnippets.Font = new Font("Segoe UI", 9F);
        cmbSnippets.Margin = new Padding(3);
        cmbSnippets.Name = "cmbSnippets";
        cmbSnippets.SelectedIndexChanged += CmbSnippets_SelectedIndexChanged;
        snippetSection.Controls.Add(cmbSnippets, 1, 0);

        buttonFlow.AutoSize = true;
        buttonFlow.Dock = DockStyle.Fill;
        buttonFlow.FlowDirection = FlowDirection.LeftToRight;
        buttonFlow.Margin = new Padding(3, 0, 3, 3);
        buttonFlow.WrapContents = true;

        btnNewSnippet.AutoSize = true;
        btnNewSnippet.Font = new Font("Segoe UI", 9F);
        btnNewSnippet.Margin = new Padding(0, 0, 3, 0);
        btnNewSnippet.Name = "btnNewSnippet";
        btnNewSnippet.Text = "New";
        btnNewSnippet.Click += BtnNewSnippet_Click;
        buttonFlow.Controls.Add(btnNewSnippet);

        btnDuplicateSnippet.AutoSize = true;
        btnDuplicateSnippet.Font = new Font("Segoe UI", 9F);
        btnDuplicateSnippet.Margin = new Padding(0, 0, 3, 0);
        btnDuplicateSnippet.Name = "btnDuplicateSnippet";
        btnDuplicateSnippet.Text = "Copy";
        btnDuplicateSnippet.Click += BtnDuplicateSnippet_Click;
        buttonFlow.Controls.Add(btnDuplicateSnippet);

        btnRenameSnippet.AutoSize = true;
        btnRenameSnippet.Font = new Font("Segoe UI", 9F);
        btnRenameSnippet.Margin = new Padding(0, 0, 3, 0);
        btnRenameSnippet.Name = "btnRenameSnippet";
        btnRenameSnippet.Text = "Rename";
        btnRenameSnippet.Click += BtnRenameSnippet_Click;
        buttonFlow.Controls.Add(btnRenameSnippet);

        btnDeleteSnippet.AutoSize = true;
        btnDeleteSnippet.Font = new Font("Segoe UI", 9F);
        btnDeleteSnippet.Margin = new Padding(0);
        btnDeleteSnippet.Name = "btnDeleteSnippet";
        btnDeleteSnippet.Text = "Delete";
        btnDeleteSnippet.Click += BtnDeleteSnippet_Click;
        buttonFlow.Controls.Add(btnDeleteSnippet);

        snippetSection.Controls.Add(buttonFlow, 1, 1);
        mainLayout.Controls.Add(snippetSection, 0, 1);

        // Row 2: Text content
        txtPredefinedText.Dock = DockStyle.Fill;
        txtPredefinedText.Font = new Font("Segoe UI", 9F);
        txtPredefinedText.Margin = new Padding(3);
        txtPredefinedText.Multiline = true;
        txtPredefinedText.Name = "txtPredefinedText";
        txtPredefinedText.ScrollBars = ScrollBars.Vertical;
        mainLayout.Controls.Add(txtPredefinedText, 0, 2);

        // Row 3: Speed section
        speedSection.AutoSize = true;
        speedSection.ColumnCount = 3;
        speedSection.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        speedSection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        speedSection.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        speedSection.Dock = DockStyle.Fill;
        speedSection.Margin = new Padding(3);
        speedSection.RowCount = 1;
        speedSection.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        lblTypingSpeed.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        lblTypingSpeed.Font = new Font("Segoe UI", 9F);
        lblTypingSpeed.Margin = new Padding(3);
        lblTypingSpeed.Text = "Typing Speed:";
        speedSection.Controls.Add(lblTypingSpeed, 0, 0);

        sliderTypingSpeed.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        sliderTypingSpeed.Margin = new Padding(3);
        sliderTypingSpeed.Maximum = 10;
        sliderTypingSpeed.Minimum = 1;
        sliderTypingSpeed.Name = "sliderTypingSpeed";
        sliderTypingSpeed.TickFrequency = 1;
        sliderTypingSpeed.TickStyle = TickStyle.None;
        sliderTypingSpeed.Value = typingSpeed;
        sliderTypingSpeed.ValueChanged += TypingSpeedSlider_ValueChanged;
        speedSection.Controls.Add(sliderTypingSpeed, 1, 0);

        lblSpeedIndicator.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        lblSpeedIndicator.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
        lblSpeedIndicator.Margin = new Padding(3);
        lblSpeedIndicator.Name = "lblSpeedIndicator";
        lblSpeedIndicator.Text = "Normal";
        speedSection.Controls.Add(lblSpeedIndicator, 2, 0);

        mainLayout.Controls.Add(speedSection, 0, 3);

        // Row 4: Options section
        optionsSection.AutoSize = true;
        optionsSection.ColumnCount = 2;
        optionsSection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        optionsSection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        optionsSection.Dock = DockStyle.Fill;
        optionsSection.Margin = new Padding(3);
        optionsSection.RowCount = 1;
        optionsSection.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        chkHasCode.Anchor = AnchorStyles.Left;
        chkHasCode.AutoSize = true;
        chkHasCode.Checked = hasCodeMode;
        chkHasCode.Font = new Font("Segoe UI", 9F);
        chkHasCode.Margin = new Padding(3);
        chkHasCode.Name = "chkHasCode";
        chkHasCode.Text = "Has Code (limit speed)";
        chkHasCode.CheckedChanged += ChkHasCode_CheckedChanged;
        optionsSection.Controls.Add(chkHasCode, 0, 0);

        chkUseFile.Anchor = AnchorStyles.Left;
        chkUseFile.AutoSize = true;
        chkUseFile.Font = new Font("Segoe UI", 9F);
        chkUseFile.Margin = new Padding(3);
        chkUseFile.Name = "chkUseFile";
        chkUseFile.Text = "Use File (.md/.txt)";
        chkUseFile.CheckedChanged += ChkUseFile_CheckedChanged;
        optionsSection.Controls.Add(chkUseFile, 1, 0);

        mainLayout.Controls.Add(optionsSection, 0, 4);

        // Row 5: File section
        fileSection.AutoSize = true;
        fileSection.ColumnCount = 2;
        fileSection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        fileSection.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        fileSection.Dock = DockStyle.Fill;
        fileSection.Margin = new Padding(3);
        fileSection.RowCount = 1;
        fileSection.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        txtFilePath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        txtFilePath.Enabled = false;
        txtFilePath.Font = new Font("Segoe UI", 9F);
        txtFilePath.Margin = new Padding(3);
        txtFilePath.Name = "txtFilePath";
        txtFilePath.ReadOnly = true;
        fileSection.Controls.Add(txtFilePath, 0, 0);

        btnBrowseFile.Anchor = AnchorStyles.Left;
        btnBrowseFile.AutoSize = true;
        btnBrowseFile.Enabled = false;
        btnBrowseFile.Font = new Font("Segoe UI", 9F);
        btnBrowseFile.Margin = new Padding(3);
        btnBrowseFile.Name = "btnBrowseFile";
        btnBrowseFile.Text = "Browse…";
        btnBrowseFile.Click += BtnBrowseFile_Click;
        fileSection.Controls.Add(btnBrowseFile, 1, 0);

        mainLayout.Controls.Add(fileSection, 0, 5);

        // Row 6: Spacer
        spacer.Dock = DockStyle.Fill;
        spacer.Margin = new Padding(0);
        mainLayout.Controls.Add(spacer, 0, 6);

        // Row 7: Action section
        actionSection.AutoSize = true;
        actionSection.Dock = DockStyle.Fill;
        actionSection.FlowDirection = FlowDirection.LeftToRight;
        actionSection.Margin = new Padding(3);
        actionSection.WrapContents = true;

        btnUpdate.AutoSize = true;
        btnUpdate.Font = new Font("Segoe UI", 9F);
        btnUpdate.Margin = new Padding(0, 0, 5, 0);
        btnUpdate.Text = "Save";
        btnUpdate.Click += BtnUpdate_Click;
        actionSection.Controls.Add(btnUpdate);

        btnMinimize.AutoSize = true;
        btnMinimize.Font = new Font("Segoe UI", 9F);
        btnMinimize.Margin = new Padding(0, 0, 5, 0);
        btnMinimize.Text = "Minimize to Tray";
        btnMinimize.Click += BtnMinimize_Click;
        actionSection.Controls.Add(btnMinimize);

        btnStop.AutoSize = true;
        btnStop.Enabled = false;
        btnStop.Font = new Font("Segoe UI", 9F);
        btnStop.Margin = new Padding(0, 0, 10, 0);
        btnStop.Name = "btnStop";
        btnStop.Text = "Stop Typing";
        btnStop.Click += BtnStop_Click;
        actionSection.Controls.Add(btnStop);

        lblStatus.Anchor = AnchorStyles.Left;
        lblStatus.AutoSize = true;
        lblStatus.Font = new Font("Segoe UI", 9F);
        lblStatus.Margin = new Padding(0);
        lblStatus.Name = "lblStatus";
        lblStatus.Text = "Status: Hotkey CTRL+SHIFT+1 is active";
        actionSection.Controls.Add(lblStatus);

        mainLayout.Controls.Add(actionSection, 0, 7);

        // 5. Configure Form LAST
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(520, 640);
        Controls.Add(mainLayout);
        Controls.Add(menuStrip);
        FormBorderStyle = FormBorderStyle.Sizable;
        MainMenuStrip = menuStrip;
        MaximizeBox = true;
        MinimumSize = new Size(520, 640);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Hotkey Typer - CTRL+SHIFT+1 to Type Text";

        // 6. Resume layout
        menuStrip.ResumeLayout(false);
        menuStrip.PerformLayout();
        mainLayout.ResumeLayout(false);
        mainLayout.PerformLayout();
        snippetSection.ResumeLayout(false);
        snippetSection.PerformLayout();
        buttonFlow.ResumeLayout(false);
        buttonFlow.PerformLayout();
        speedSection.ResumeLayout(false);
        speedSection.PerformLayout();
        optionsSection.ResumeLayout(false);
        optionsSection.PerformLayout();
        fileSection.ResumeLayout(false);
        fileSection.PerformLayout();
        actionSection.ResumeLayout(false);
        actionSection.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    // 7. Backing fields at EOF
    private ComboBox cmbSnippets;
    private TextBox txtPredefinedText;
    private TrackBar sliderTypingSpeed;
    private Label lblSpeedIndicator;
    private Label lblStatus;
    private Button btnStop;
    private CheckBox chkHasCode;
    private CheckBox chkUseFile;
    private TextBox txtFilePath;
    private Button btnBrowseFile;
    private ToolStripMenuItem mnuThemeSystem;
    private ToolStripMenuItem mnuThemeLight;
    private ToolStripMenuItem mnuThemeDark;
}
