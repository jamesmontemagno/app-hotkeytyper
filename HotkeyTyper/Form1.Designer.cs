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
        components = new System.ComponentModel.Container();
        
        // Form properties
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(520, 640);
        Text = "Hotkey Typer - CTRL+SHIFT+1 to Type Text";
        StartPosition = FormStartPosition.CenterScreen;
        MaximizeBox = true;
        FormBorderStyle = FormBorderStyle.Sizable;
        MinimumSize = new Size(520, 640);
        
        // Create menu
        CreateMenu();
        
        // Create controls
        CreateControls();
    }
    
    private void CreateMenu()
    {
        var menuStrip = new MenuStrip
        {
            Name = "menuStrip",
            Dock = DockStyle.Top
        };
        
        var mnuHelp = new ToolStripMenuItem
        {
            Name = "mnuHelp",
            Text = "&Help"
        };
        
        var mnuCheckForUpdates = new ToolStripMenuItem
        {
            Name = "mnuCheckForUpdates",
            Text = "Check for &Updates..."
        };
        mnuCheckForUpdates.Click += MnuCheckForUpdates_Click;
        
        var mnuAbout = new ToolStripMenuItem
        {
            Name = "mnuAbout",
            Text = "&About..."
        };
        mnuAbout.Click += MnuAbout_Click;
        
        mnuHelp.DropDownItems.Add(mnuCheckForUpdates);
        mnuHelp.DropDownItems.Add(new ToolStripSeparator());
        mnuHelp.DropDownItems.Add(mnuAbout);
        menuStrip.Items.Add(mnuHelp);
        
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;
    }
    
    private void CreateControls()
    {
        // Create main TableLayoutPanel
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 8,
            Padding = new Padding(10)
        };
        
        // Configure column (single column at 100%)
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        
        // Configure rows
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Row 0: Instructions
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Row 1: Snippet management
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 70F)); // Row 2: Text content (70%)
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Row 3: Typing speed
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Row 4: Options checkboxes
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Row 5: File path
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F)); // Row 6: Spacer (fill remaining)
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Row 7: Action buttons
        
        // Row 0: Instructions Label
        var lblInstructions = new Label
        {
            Text = "Select or manage snippets below.\nPress CTRL+SHIFT+1 anywhere to type active snippet:",
            AutoSize = true,
            Anchor = AnchorStyles.Left | AnchorStyles.Top,
            Margin = new Padding(3),
            Font = new Font("Segoe UI", 10F, FontStyle.Regular)
        };
        mainLayout.Controls.Add(lblInstructions, 0, 0);
        
        // Row 1: Snippet Management Section (nested TLP)
        var snippetSection = new TableLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Margin = new Padding(3)
        };
        snippetSection.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Label column
        snippetSection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // Content column
        snippetSection.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // ComboBox row
        snippetSection.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons row
        
        var lblSnippet = new Label
        {
            Text = "Snippet:",
            Anchor = AnchorStyles.Left | AnchorStyles.Right,
            Margin = new Padding(3),
            Font = new Font("Segoe UI", 9F)
        };
        snippetSection.Controls.Add(lblSnippet, 0, 0);
        
        var cmbSnippets = new ComboBox
        {
            Name = "cmbSnippets",
            Anchor = AnchorStyles.Left | AnchorStyles.Right,
            Margin = new Padding(3),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9F)
        };
        cmbSnippets.SelectedIndexChanged += CmbSnippets_SelectedIndexChanged;
        snippetSection.Controls.Add(cmbSnippets, 1, 0);
        
        // Buttons in a FlowLayoutPanel for better wrapping
        var buttonFlow = new FlowLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            Margin = new Padding(3, 0, 3, 3),
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true
        };
        
        var btnNewSnippet = new Button
        {
            Name = "btnNewSnippet",
            Text = "New",
            AutoSize = true,
            Margin = new Padding(0, 0, 3, 0),
            Font = new Font("Segoe UI", 9F)
        };
        btnNewSnippet.Click += BtnNewSnippet_Click;
        buttonFlow.Controls.Add(btnNewSnippet);
        
        var btnDuplicateSnippet = new Button
        {
            Name = "btnDuplicateSnippet",
            Text = "Copy",
            AutoSize = true,
            Margin = new Padding(0, 0, 3, 0),
            Font = new Font("Segoe UI", 9F)
        };
        btnDuplicateSnippet.Click += BtnDuplicateSnippet_Click;
        buttonFlow.Controls.Add(btnDuplicateSnippet);
        
        var btnRenameSnippet = new Button
        {
            Name = "btnRenameSnippet",
            Text = "Rename",
            AutoSize = true,
            Margin = new Padding(0, 0, 3, 0),
            Font = new Font("Segoe UI", 9F)
        };
        btnRenameSnippet.Click += BtnRenameSnippet_Click;
        buttonFlow.Controls.Add(btnRenameSnippet);
        
        var btnDeleteSnippet = new Button
        {
            Name = "btnDeleteSnippet",
            Text = "Delete",
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 0),
            Font = new Font("Segoe UI", 9F)
        };
        btnDeleteSnippet.Click += BtnDeleteSnippet_Click;
        buttonFlow.Controls.Add(btnDeleteSnippet);
        
        snippetSection.Controls.Add(buttonFlow, 1, 1);
        mainLayout.Controls.Add(snippetSection, 0, 1);
        
        // Row 2: Text Content Area (MultiLine TextBox)
        var txtPredefinedText = new TextBox
        {
            Name = "txtPredefinedText",
            Text = string.Empty,
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Dock = DockStyle.Fill,
            Margin = new Padding(3),
            Font = new Font("Segoe UI", 9F)
        };
        mainLayout.Controls.Add(txtPredefinedText, 0, 2);
        
        // Row 3: Typing Speed Section (nested TLP)
        var speedSection = new TableLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Margin = new Padding(3)
        };
        speedSection.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Label
        speedSection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // Slider
        speedSection.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Speed indicator
        speedSection.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        
        var lblTypingSpeed = new Label
        {
            Text = "Typing Speed:",
            Anchor = AnchorStyles.Left | AnchorStyles.Right,
            Margin = new Padding(3),
            Font = new Font("Segoe UI", 9F)
        };
        speedSection.Controls.Add(lblTypingSpeed, 0, 0);
        
        var sliderTypingSpeed = new LimitedTrackBar
        {
            Name = "sliderTypingSpeed",
            Anchor = AnchorStyles.Left | AnchorStyles.Right,
            Margin = new Padding(3),
            Minimum = 1,
            Maximum = 10,
            Value = typingSpeed,
            TickFrequency = 1,
            TickStyle = TickStyle.None,
            SoftMax = hasCodeMode ? 8 : null
        };
        sliderTypingSpeed.ValueChanged += TypingSpeedSlider_ValueChanged;
        speedSection.Controls.Add(sliderTypingSpeed, 1, 0);
        
        var lblSpeedIndicator = new Label
        {
            Name = "lblSpeedIndicator",
            Text = "Normal",
            Anchor = AnchorStyles.Left | AnchorStyles.Right,
            Margin = new Padding(3),
            Font = new Font("Segoe UI", 9F, FontStyle.Italic)
        };
        speedSection.Controls.Add(lblSpeedIndicator, 2, 0);
        
        mainLayout.Controls.Add(speedSection, 0, 3);
        
        // Row 4: Options Checkboxes (nested TLP)
        var optionsSection = new TableLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(3)
        };
        optionsSection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        optionsSection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        optionsSection.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        
        var chkHasCode = new CheckBox
        {
            Name = "chkHasCode",
            Text = "Has Code (limit speed)",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(3),
            Checked = hasCodeMode,
            Font = new Font("Segoe UI", 9F)
        };
        chkHasCode.CheckedChanged += ChkHasCode_CheckedChanged;
        optionsSection.Controls.Add(chkHasCode, 0, 0);
        
        var chkUseFile = new CheckBox
        {
            Name = "chkUseFile",
            Text = "Use File (.md/.txt)",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(3),
            Checked = false,
            Font = new Font("Segoe UI", 9F)
        };
        chkUseFile.CheckedChanged += ChkUseFile_CheckedChanged;
        optionsSection.Controls.Add(chkUseFile, 1, 0);
        
        mainLayout.Controls.Add(optionsSection, 0, 4);
        
        // Row 5: File Path Section (nested TLP)
        var fileSection = new TableLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(3)
        };
        fileSection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // TextBox
        fileSection.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Button
        fileSection.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        
        var txtFilePath = new TextBox
        {
            Name = "txtFilePath",
            Text = string.Empty,
            Enabled = false,
            ReadOnly = true,
            Anchor = AnchorStyles.Left | AnchorStyles.Right,
            Margin = new Padding(3),
            Font = new Font("Segoe UI", 9F)
        };
        fileSection.Controls.Add(txtFilePath, 0, 0);
        
        var btnBrowseFile = new Button
        {
            Name = "btnBrowseFile",
            Text = "Browse…",
            AutoSize = true,
            Enabled = false,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(3),
            Font = new Font("Segoe UI", 9F)
        };
        btnBrowseFile.Click += BtnBrowseFile_Click;
        fileSection.Controls.Add(btnBrowseFile, 1, 0);
        
        mainLayout.Controls.Add(fileSection, 0, 5);
        
        // Row 6: Spacer (empty panel to fill remaining space)
        var spacer = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0)
        };
        mainLayout.Controls.Add(spacer, 0, 6);
        
        // Row 7: Action Buttons (FlowLayoutPanel with buttons and status)
        var actionSection = new FlowLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = new Padding(3)
        };
        
        var btnUpdate = new Button
        {
            Text = "Save",
            AutoSize = true,
            Margin = new Padding(0, 0, 5, 0),
            Font = new Font("Segoe UI", 9F)
        };
        btnUpdate.Click += BtnUpdate_Click;
        actionSection.Controls.Add(btnUpdate);
        
        var btnMinimize = new Button
        {
            Text = "Minimize to Tray",
            AutoSize = true,
            Margin = new Padding(0, 0, 5, 0),
            Font = new Font("Segoe UI", 9F)
        };
        btnMinimize.Click += BtnMinimize_Click;
        actionSection.Controls.Add(btnMinimize);
        
        var btnStop = new Button
        {
            Name = "btnStop",
            Text = "Stop Typing",
            AutoSize = true,
            Margin = new Padding(0, 0, 10, 0),
            Font = new Font("Segoe UI", 9F),
            Enabled = false
        };
        btnStop.Click += BtnStop_Click;
        actionSection.Controls.Add(btnStop);
        
        var lblStatus = new Label
        {
            Name = "lblStatus",
            Text = "Status: Hotkey CTRL+SHIFT+1 is active",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(0),
            Font = new Font("Segoe UI", 9F)
        };
        actionSection.Controls.Add(lblStatus);
        
        mainLayout.Controls.Add(actionSection, 0, 7);
        
        // Add main layout to form
        Controls.Add(mainLayout);
        
        // Store references for later use
        this.cmbSnippets = cmbSnippets;
        this.txtPredefinedText = txtPredefinedText;
        this.sliderTypingSpeed = sliderTypingSpeed;
        this.lblSpeedIndicator = lblSpeedIndicator;
        this.lblStatus = lblStatus;
        this.btnStop = btnStop;
        this.chkHasCode = chkHasCode;
        this.chkUseFile = chkUseFile;
        this.txtFilePath = txtFilePath;
        this.btnBrowseFile = btnBrowseFile;
    }
    
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

    #endregion
}
