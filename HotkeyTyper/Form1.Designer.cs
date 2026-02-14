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
        // =====================================================================
        // Layout strategy: Dock-based (Top / Bottom / Fill)
        //
        //   Form
        //   ├── MenuStrip                          (Dock.Top, automatic)
        //   ├── headerPanel                        (Dock.Top, AutoSize)
        //   │     └── headerTable
        //   │           Row 0: lblInstructions     (full-width)
        //   │           Row 1: lblSnippet + combo  (label | stretch combo)
        //   │           Row 2: buttonFlow          (New, Copy, Rename, Delete)
        //   ├── footerPanel                        (Dock.Bottom, AutoSize)
        //   │     └── footerTable
        //   │           Row 0: speed row           (label | slider | indicator)
        //   │           Row 1: options row         (two checkboxes)
        //   │           Row 2: file row            (path | Browse)
        //   │           Row 3: action row          (Save, Minimize, Stop)
        //   │           Row 4: lblStatus
        //   └── editorPanel                        (Dock.Fill)
        //         └── txtPredefinedText             (Dock.Fill – main editor)
        //
        // The editor panel is DockStyle.Fill so it ALWAYS gets whatever vertical
        // space remains after the header and footer claim their auto-sized share.
        // =====================================================================

        // --- Instantiate all controls -------------------------------------------

        // Menu
        MenuStrip menuStrip = new MenuStrip();
        ToolStripMenuItem mnuFile = new ToolStripMenuItem();
        ToolStripMenuItem mnuSaveSnippet = new ToolStripMenuItem();
        ToolStripMenuItem mnuMinimizeToTray = new ToolStripMenuItem();
        ToolStripMenuItem mnuSettings = new ToolStripMenuItem();
        ToolStripMenuItem mnuConfigureHotkey = new ToolStripMenuItem();
        ToolStripMenuItem mnuTheme = new ToolStripMenuItem();
        mnuThemeSystem = new ToolStripMenuItem();
        mnuThemeLight = new ToolStripMenuItem();
        mnuThemeDark = new ToolStripMenuItem();
        ToolStripMenuItem mnuHelp = new ToolStripMenuItem();
        ToolStripMenuItem mnuCheckForUpdates = new ToolStripMenuItem();
        ToolStripMenuItem mnuAbout = new ToolStripMenuItem();

        // Header
        Panel headerPanel = new Panel();
        TableLayoutPanel headerTable = new TableLayoutPanel();
        Label lblInstructions = new Label();
        Label lblSnippet = new Label();
        cmbSnippets = new ComboBox();
        FlowLayoutPanel buttonFlow = new FlowLayoutPanel();
        Button btnNewSnippet = new Button();
        Button btnDuplicateSnippet = new Button();
        Button btnRenameSnippet = new Button();
        Button btnDeleteSnippet = new Button();

        // Editor (center / fill)
        Panel editorPanel = new Panel();
        txtPredefinedText = new TextBox();

        // Footer
        Panel footerPanel = new Panel();
        TableLayoutPanel footerTable = new TableLayoutPanel();
        TableLayoutPanel speedRow = new TableLayoutPanel();
        Label lblTypingSpeed = new Label();
        sliderTypingSpeed = new LimitedTrackBar();
        lblSpeedIndicator = new Label();
        TableLayoutPanel optionsRow = new TableLayoutPanel();
        chkHasCode = new CheckBox();
        chkUseFile = new CheckBox();
        TableLayoutPanel fileRow = new TableLayoutPanel();
        txtFilePath = new TextBox();
        btnBrowseFile = new Button();
        FlowLayoutPanel actionFlow = new FlowLayoutPanel();
        Button btnUpdate = new Button();
        Button btnMinimize = new Button();
        btnStop = new Button();
        lblStatus = new Label();

        components = new System.ComponentModel.Container();

        // --- Suspend layout -----------------------------------------------------
        menuStrip.SuspendLayout();
        headerPanel.SuspendLayout();
        headerTable.SuspendLayout();
        buttonFlow.SuspendLayout();
        editorPanel.SuspendLayout();
        footerPanel.SuspendLayout();
        footerTable.SuspendLayout();
        speedRow.SuspendLayout();
        optionsRow.SuspendLayout();
        fileRow.SuspendLayout();
        actionFlow.SuspendLayout();
        SuspendLayout();

        // =====================================================================
        // MENU STRIP (unchanged logic, just reorganized)
        // =====================================================================

        menuStrip.Name = "menuStrip";
        menuStrip.TabIndex = 0;

        // File menu
        mnuFile.Name = "mnuFile";
        mnuFile.Text = "&File";
        mnuSaveSnippet.Name = "mnuSaveSnippet";
        mnuSaveSnippet.Text = "&Save Snippet";
        mnuSaveSnippet.ShortcutKeys = Keys.Control | Keys.S;
        mnuSaveSnippet.Click += BtnUpdate_Click;
        mnuMinimizeToTray.Name = "mnuMinimizeToTray";
        mnuMinimizeToTray.Text = "&Minimize to Tray";
        mnuMinimizeToTray.Click += BtnMinimize_Click;
        mnuFile.DropDownItems.Add(mnuSaveSnippet);
        mnuFile.DropDownItems.Add(new ToolStripSeparator());
        mnuFile.DropDownItems.Add(mnuMinimizeToTray);

        // Settings menu
        mnuSettings.Name = "mnuSettings";
        mnuSettings.Text = "&Settings";
        mnuConfigureHotkey.Name = "mnuConfigureHotkey";
        mnuConfigureHotkey.Text = "Configure &Hotkey...";
        mnuConfigureHotkey.Click += MnuConfigureHotkey_Click;
        mnuTheme.Name = "mnuTheme";
        mnuTheme.Text = "&Theme";
        mnuThemeSystem.Name = "mnuThemeSystem";
        mnuThemeSystem.Text = "&System";
        mnuThemeSystem.Click += MnuThemeSystem_Click;
        mnuThemeLight.Name = "mnuThemeLight";
        mnuThemeLight.Text = "&Light";
        mnuThemeLight.Click += MnuThemeLight_Click;
        mnuThemeDark.Name = "mnuThemeDark";
        mnuThemeDark.Text = "&Dark";
        mnuThemeDark.Click += MnuThemeDark_Click;
        mnuTheme.DropDownItems.Add(mnuThemeSystem);
        mnuTheme.DropDownItems.Add(mnuThemeLight);
        mnuTheme.DropDownItems.Add(mnuThemeDark);
        mnuSettings.DropDownItems.Add(mnuConfigureHotkey);
        mnuSettings.DropDownItems.Add(new ToolStripSeparator());
        mnuSettings.DropDownItems.Add(mnuTheme);

        // Help menu
        mnuHelp.Name = "mnuHelp";
        mnuHelp.Text = "&Help";
        mnuCheckForUpdates.Name = "mnuCheckForUpdates";
        mnuCheckForUpdates.Text = "Check for &Updates...";
        mnuCheckForUpdates.Click += MnuCheckForUpdates_Click;
        mnuAbout.Name = "mnuAbout";
        mnuAbout.Text = "&About...";
        mnuAbout.Click += MnuAbout_Click;
        mnuHelp.DropDownItems.Add(mnuCheckForUpdates);
        mnuHelp.DropDownItems.Add(new ToolStripSeparator());
        mnuHelp.DropDownItems.Add(mnuAbout);

        menuStrip.Items.Add(mnuFile);
        menuStrip.Items.Add(mnuSettings);
        menuStrip.Items.Add(mnuHelp);

        // =====================================================================
        // HEADER PANEL  (Dock.Top, AutoSize)
        // =====================================================================

        headerPanel.AutoSize = true;
        headerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        headerPanel.Dock = DockStyle.Top;
        headerPanel.MinimumSize = new Size(0, 160);
        headerPanel.Padding = new Padding(12, 8, 12, 10);

        // Header table: 3 rows, 2 columns (label | content)
        headerTable.AutoSize = true;
        headerTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        headerTable.Dock = DockStyle.Top;
        headerTable.ColumnCount = 2;
        headerTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        headerTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        headerTable.RowCount = 3;
        headerTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        headerTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        headerTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        headerTable.Margin = new Padding(0);

        // Row 0: Instructions (spans both columns)
        lblInstructions.AutoSize = true;
        lblInstructions.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        lblInstructions.Margin = new Padding(3, 3, 3, 8);
        lblInstructions.Text = "Select or manage snippets, then press your hotkey anywhere to type the active snippet.";
        headerTable.SetColumnSpan(lblInstructions, 2);
        headerTable.Controls.Add(lblInstructions, 0, 0);

        // Row 1: Snippet label + combo
        lblSnippet.Anchor = AnchorStyles.Left;
        lblSnippet.AutoSize = true;
        lblSnippet.Font = new Font("Segoe UI", 9F);
        lblSnippet.Margin = new Padding(3);
        lblSnippet.Text = "Snippet:";
        headerTable.Controls.Add(lblSnippet, 0, 1);

        cmbSnippets.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        cmbSnippets.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbSnippets.Font = new Font("Segoe UI", 9F);
        cmbSnippets.Margin = new Padding(3);
        cmbSnippets.Name = "cmbSnippets";
        cmbSnippets.SelectedIndexChanged += CmbSnippets_SelectedIndexChanged;
        headerTable.Controls.Add(cmbSnippets, 1, 1);

        // Row 2: Snippet action buttons
        buttonFlow.AutoSize = true;
        buttonFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        buttonFlow.Dock = DockStyle.Fill;
        buttonFlow.FlowDirection = FlowDirection.LeftToRight;
        buttonFlow.Margin = new Padding(3, 2, 3, 8);
        buttonFlow.WrapContents = true;

        btnNewSnippet.AutoSize = true;
        btnNewSnippet.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        btnNewSnippet.Font = new Font("Segoe UI", 9F);
        btnNewSnippet.Margin = new Padding(0, 0, 4, 0);
        btnNewSnippet.Name = "btnNewSnippet";
        btnNewSnippet.Padding = new Padding(10, 4, 10, 4);
        btnNewSnippet.Text = "New";
        btnNewSnippet.Click += BtnNewSnippet_Click;
        buttonFlow.Controls.Add(btnNewSnippet);

        btnDuplicateSnippet.AutoSize = true;
        btnDuplicateSnippet.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        btnDuplicateSnippet.Font = new Font("Segoe UI", 9F);
        btnDuplicateSnippet.Margin = new Padding(0, 0, 4, 0);
        btnDuplicateSnippet.Name = "btnDuplicateSnippet";
        btnDuplicateSnippet.Padding = new Padding(10, 4, 10, 4);
        btnDuplicateSnippet.Text = "Copy";
        btnDuplicateSnippet.Click += BtnDuplicateSnippet_Click;
        buttonFlow.Controls.Add(btnDuplicateSnippet);

        btnRenameSnippet.AutoSize = true;
        btnRenameSnippet.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        btnRenameSnippet.Font = new Font("Segoe UI", 9F);
        btnRenameSnippet.Margin = new Padding(0, 0, 4, 0);
        btnRenameSnippet.Name = "btnRenameSnippet";
        btnRenameSnippet.Padding = new Padding(10, 4, 10, 4);
        btnRenameSnippet.Text = "Rename";
        btnRenameSnippet.Click += BtnRenameSnippet_Click;
        buttonFlow.Controls.Add(btnRenameSnippet);

        btnDeleteSnippet.AutoSize = true;
        btnDeleteSnippet.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        btnDeleteSnippet.Font = new Font("Segoe UI", 9F);
        btnDeleteSnippet.Margin = new Padding(0);
        btnDeleteSnippet.Name = "btnDeleteSnippet";
        btnDeleteSnippet.Padding = new Padding(10, 4, 10, 4);
        btnDeleteSnippet.Text = "Delete";
        btnDeleteSnippet.Click += BtnDeleteSnippet_Click;
        buttonFlow.Controls.Add(btnDeleteSnippet);

        headerTable.Controls.Add(buttonFlow, 1, 2);
        headerPanel.Controls.Add(headerTable);

        // =====================================================================
        // EDITOR PANEL  (Dock.Fill – takes ALL remaining vertical space)
        // =====================================================================

        editorPanel.Dock = DockStyle.Fill;
        editorPanel.Padding = new Padding(12, 10, 12, 8);

        txtPredefinedText.Dock = DockStyle.Fill;
        txtPredefinedText.Font = new Font("Consolas", 10F);
        txtPredefinedText.Multiline = true;
        txtPredefinedText.Name = "txtPredefinedText";
        txtPredefinedText.ScrollBars = ScrollBars.Vertical;
        txtPredefinedText.AcceptsTab = true;
        txtPredefinedText.AcceptsReturn = true;
        txtPredefinedText.WordWrap = true;
        editorPanel.Controls.Add(txtPredefinedText);

        // =====================================================================
        // FOOTER PANEL  (Dock.Bottom, AutoSize)
        // =====================================================================

        footerPanel.AutoSize = true;
        footerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        footerPanel.Dock = DockStyle.Bottom;
        footerPanel.Padding = new Padding(12, 4, 12, 8);

        // Footer table: 5 rows, 1 column
        footerTable.AutoSize = true;
        footerTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        footerTable.Dock = DockStyle.Top;
        footerTable.ColumnCount = 1;
        footerTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        footerTable.RowCount = 5;
        footerTable.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // speed
        footerTable.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // options
        footerTable.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // file
        footerTable.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // actions
        footerTable.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // status
        footerTable.Margin = new Padding(0);

        // --- Footer Row 0: Speed ------------------------------------------------
        speedRow.AutoSize = true;
        speedRow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        speedRow.Dock = DockStyle.Top;
        speedRow.ColumnCount = 3;
        speedRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        speedRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        speedRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        speedRow.RowCount = 1;
        speedRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        speedRow.Margin = new Padding(0, 3, 0, 3);

        lblTypingSpeed.Anchor = AnchorStyles.Left;
        lblTypingSpeed.AutoSize = true;
        lblTypingSpeed.Font = new Font("Segoe UI", 9F);
        lblTypingSpeed.Margin = new Padding(3);
        lblTypingSpeed.Text = "Typing Speed:";
        speedRow.Controls.Add(lblTypingSpeed, 0, 0);

        sliderTypingSpeed.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        sliderTypingSpeed.Margin = new Padding(3);
        sliderTypingSpeed.Maximum = 10;
        sliderTypingSpeed.Minimum = 1;
        sliderTypingSpeed.Name = "sliderTypingSpeed";
        sliderTypingSpeed.TickFrequency = 1;
        sliderTypingSpeed.TickStyle = TickStyle.None;
        sliderTypingSpeed.Value = typingSpeed;
        sliderTypingSpeed.ValueChanged += TypingSpeedSlider_ValueChanged;
        speedRow.Controls.Add(sliderTypingSpeed, 1, 0);

        lblSpeedIndicator.Anchor = AnchorStyles.Left;
        lblSpeedIndicator.AutoSize = true;
        lblSpeedIndicator.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
        lblSpeedIndicator.Margin = new Padding(3);
        lblSpeedIndicator.Name = "lblSpeedIndicator";
        lblSpeedIndicator.Text = "Normal";
        speedRow.Controls.Add(lblSpeedIndicator, 2, 0);

        footerTable.Controls.Add(speedRow, 0, 0);

        // --- Footer Row 1: Options -----------------------------------------------
        optionsRow.AutoSize = true;
        optionsRow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        optionsRow.Dock = DockStyle.Top;
        optionsRow.ColumnCount = 2;
        optionsRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        optionsRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        optionsRow.RowCount = 1;
        optionsRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        optionsRow.Margin = new Padding(0, 2, 0, 2);

        chkHasCode.Anchor = AnchorStyles.Left;
        chkHasCode.AutoSize = true;
        chkHasCode.Checked = hasCodeMode;
        chkHasCode.Font = new Font("Segoe UI", 9F);
        chkHasCode.Margin = new Padding(3);
        chkHasCode.Name = "chkHasCode";
        chkHasCode.Text = "Has Code (limit speed)";
        chkHasCode.CheckedChanged += ChkHasCode_CheckedChanged;
        optionsRow.Controls.Add(chkHasCode, 0, 0);

        chkUseFile.Anchor = AnchorStyles.Left;
        chkUseFile.AutoSize = true;
        chkUseFile.Font = new Font("Segoe UI", 9F);
        chkUseFile.Margin = new Padding(3);
        chkUseFile.Name = "chkUseFile";
        chkUseFile.Text = "Use File (.md/.txt)";
        chkUseFile.CheckedChanged += ChkUseFile_CheckedChanged;
        optionsRow.Controls.Add(chkUseFile, 1, 0);

        footerTable.Controls.Add(optionsRow, 0, 1);

        // --- Footer Row 2: File path ---------------------------------------------
        fileRow.AutoSize = true;
        fileRow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        fileRow.Dock = DockStyle.Top;
        fileRow.ColumnCount = 2;
        fileRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        fileRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        fileRow.RowCount = 1;
        fileRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        fileRow.Margin = new Padding(0, 2, 0, 2);

        txtFilePath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        txtFilePath.Enabled = false;
        txtFilePath.Font = new Font("Segoe UI", 9F);
        txtFilePath.Margin = new Padding(3);
        txtFilePath.Name = "txtFilePath";
        txtFilePath.ReadOnly = true;
        fileRow.Controls.Add(txtFilePath, 0, 0);

        btnBrowseFile.Anchor = AnchorStyles.Left;
        btnBrowseFile.AutoSize = true;
        btnBrowseFile.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        btnBrowseFile.Enabled = false;
        btnBrowseFile.Font = new Font("Segoe UI", 9F);
        btnBrowseFile.Margin = new Padding(3);
        btnBrowseFile.Name = "btnBrowseFile";
        btnBrowseFile.Padding = new Padding(10, 4, 10, 4);
        btnBrowseFile.Text = "Browse…";
        btnBrowseFile.Click += BtnBrowseFile_Click;
        fileRow.Controls.Add(btnBrowseFile, 1, 0);

        footerTable.Controls.Add(fileRow, 0, 2);

        // --- Footer Row 3: Action buttons ----------------------------------------
        actionFlow.AutoSize = true;
        actionFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        actionFlow.Dock = DockStyle.Top;
        actionFlow.FlowDirection = FlowDirection.LeftToRight;
        actionFlow.Margin = new Padding(0, 4, 0, 2);
        actionFlow.WrapContents = false;

        btnUpdate.AutoSize = true;
        btnUpdate.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        btnUpdate.Font = new Font("Segoe UI", 9F);
        btnUpdate.Margin = new Padding(0, 0, 6, 0);
        btnUpdate.Padding = new Padding(10, 4, 10, 4);
        btnUpdate.Text = "Save";
        btnUpdate.Click += BtnUpdate_Click;
        actionFlow.Controls.Add(btnUpdate);

        btnMinimize.AutoSize = true;
        btnMinimize.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        btnMinimize.Font = new Font("Segoe UI", 9F);
        btnMinimize.Margin = new Padding(0, 0, 6, 0);
        btnMinimize.Padding = new Padding(10, 4, 10, 4);
        btnMinimize.Text = "Minimize to Tray";
        btnMinimize.Click += BtnMinimize_Click;
        actionFlow.Controls.Add(btnMinimize);

        btnStop.AutoSize = true;
        btnStop.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        btnStop.Enabled = false;
        btnStop.Font = new Font("Segoe UI", 9F);
        btnStop.Margin = new Padding(0);
        btnStop.Name = "btnStop";
        btnStop.Padding = new Padding(10, 4, 10, 4);
        btnStop.Text = "Stop Typing";
        btnStop.Click += BtnStop_Click;
        actionFlow.Controls.Add(btnStop);

        footerTable.Controls.Add(actionFlow, 0, 3);

        // --- Footer Row 4: Status label ------------------------------------------
        lblStatus.AutoSize = true;
        lblStatus.Dock = DockStyle.Top;
        lblStatus.Font = new Font("Segoe UI", 9F);
        lblStatus.Margin = new Padding(3, 4, 3, 0);
        lblStatus.Name = "lblStatus";
        lblStatus.Text = "Status: Ready";
        footerTable.Controls.Add(lblStatus, 0, 4);

        footerPanel.Controls.Add(footerTable);

        // =====================================================================
        // FORM  –  Add controls in z-order: Fill first, then Bottom, Top, Menu
        // =====================================================================

        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(660, 780);
        MinimumSize = new Size(520, 560);
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = true;
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Hotkey Typer";

        // Z-order: lowest index docks last → Fill goes first
        Controls.Add(editorPanel);   // Dock.Fill   – gets ALL remaining space
        Controls.Add(footerPanel);   // Dock.Bottom – auto-sized footer
        Controls.Add(headerPanel);   // Dock.Top    – auto-sized header
        Controls.Add(menuStrip);     // Dock.Top    – menu bar (highest z-order)
        MainMenuStrip = menuStrip;

        // --- Resume layout ------------------------------------------------------
        menuStrip.ResumeLayout(false);
        menuStrip.PerformLayout();
        headerTable.ResumeLayout(false);
        headerTable.PerformLayout();
        buttonFlow.ResumeLayout(false);
        buttonFlow.PerformLayout();
        headerPanel.ResumeLayout(false);
        headerPanel.PerformLayout();
        editorPanel.ResumeLayout(false);
        editorPanel.PerformLayout();
        speedRow.ResumeLayout(false);
        speedRow.PerformLayout();
        optionsRow.ResumeLayout(false);
        optionsRow.PerformLayout();
        fileRow.ResumeLayout(false);
        fileRow.PerformLayout();
        actionFlow.ResumeLayout(false);
        actionFlow.PerformLayout();
        footerTable.ResumeLayout(false);
        footerTable.PerformLayout();
        footerPanel.ResumeLayout(false);
        footerPanel.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    // Backing fields
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
