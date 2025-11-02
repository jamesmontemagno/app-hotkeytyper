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
        ClientSize = new Size(500, 600);
        Text = "Hotkey Typer - CTRL+SHIFT+1 to Type Text";
        StartPosition = FormStartPosition.CenterScreen;
        MaximizeBox = false;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        
        // Create controls
        CreateControls();
    }
    
    private void CreateControls()
    {
        // Label for instructions
        var lblInstructions = new Label
        {
            Text = "Select or manage snippets below.\nPress CTRL+SHIFT+1 anywhere to type active snippet:",
            Location = new Point(20, 20),
            // Increased height to ensure second line isn't clipped on various DPI scales
            Size = new Size(460, 55),
            Font = new Font("Segoe UI", 10F, FontStyle.Regular)
        };
        
        // Snippet selector label
        var lblSnippet = new Label
        {
            Text = "Snippet:",
            Location = new Point(20, 85),
            Size = new Size(60, 20),
            Font = new Font("Segoe UI", 9F)
        };
        
        // Snippet selector ComboBox
        var cmbSnippets = new ComboBox
        {
            Name = "cmbSnippets",
            Location = new Point(85, 82),
            Size = new Size(300, 23),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9F)
        };
        cmbSnippets.SelectedIndexChanged += CmbSnippets_SelectedIndexChanged;
        
        // Snippet management buttons - moved under dropdown
        var btnNewSnippet = new Button
        {
            Name = "btnNewSnippet",
            Text = "New",
            Location = new Point(85, 110),
            Size = new Size(70, 25),
            Font = new Font("Segoe UI", 9F)
        };
        btnNewSnippet.Click += BtnNewSnippet_Click;
        
        var btnDuplicateSnippet = new Button
        {
            Name = "btnDuplicateSnippet",
            Text = "Copy",
            Location = new Point(160, 110),
            Size = new Size(70, 25),
            Font = new Font("Segoe UI", 9F)
        };
        btnDuplicateSnippet.Click += BtnDuplicateSnippet_Click;
        
        var btnRenameSnippet = new Button
        {
            Name = "btnRenameSnippet",
            Text = "Rename",
            Location = new Point(235, 110),
            Size = new Size(70, 25),
            Font = new Font("Segoe UI", 9F)
        };
        btnRenameSnippet.Click += BtnRenameSnippet_Click;
        
        var btnDeleteSnippet = new Button
        {
            Name = "btnDeleteSnippet",
            Text = "Delete",
            Location = new Point(310, 110),
            Size = new Size(75, 25),
            Font = new Font("Segoe UI", 9F)
        };
        btnDeleteSnippet.Click += BtnDeleteSnippet_Click;
        
        // TextBox for predefined text
        var txtPredefinedText = new TextBox
        {
            Name = "txtPredefinedText",
            Text = string.Empty, // Content will be loaded from active snippet
            Location = new Point(20, 145),
            Size = new Size(460, 135),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Segoe UI", 9F)
        };
        
        // Typing speed label
        var lblTypingSpeed = new Label
        {
            Text = "Typing Speed:",
            Location = new Point(20, 290),
            Size = new Size(80, 20),
            Font = new Font("Segoe UI", 9F)
        };
        
        // Typing speed slider (custom for soft limit visuals)
        var sliderTypingSpeed = new LimitedTrackBar
        {
            Name = "sliderTypingSpeed",
            Location = new Point(105, 285),
            Size = new Size(200, 45),
            Minimum = 1,
            Maximum = 10,
            Value = typingSpeed,
            TickFrequency = 1,
            TickStyle = TickStyle.None,
            SoftMax = hasCodeMode ? 8 : null
        };
        sliderTypingSpeed.ValueChanged += TypingSpeedSlider_ValueChanged;
        
        // Speed indicator label
        var lblSpeedIndicator = new Label
        {
            Name = "lblSpeedIndicator",
            Text = "Normal", // Default value, will be updated after load
            Location = new Point(315, 290),
            Size = new Size(100, 20),
            Font = new Font("Segoe UI", 9F, FontStyle.Italic)
        };

        // Checkbox: Has Code (limits speed)
        var chkHasCode = new CheckBox
        {
            Name = "chkHasCode",
            Text = "Has Code (limit speed)",
            Location = new Point(20, 350),
            Size = new Size(180, 20),
            Checked = hasCodeMode,
            Font = new Font("Segoe UI", 9F)
        };
        chkHasCode.CheckedChanged += ChkHasCode_CheckedChanged;

        // Use file checkbox
        var chkUseFile = new CheckBox
        {
            Name = "chkUseFile",
            Text = "Use File (.md/.txt)",
            Location = new Point(220, 350),
            Size = new Size(150, 20),
            Checked = false,
            Font = new Font("Segoe UI", 9F)
        };
        chkUseFile.CheckedChanged += ChkUseFile_CheckedChanged;

        // File path textbox
        var txtFilePath = new TextBox
        {
            Name = "txtFilePath",
            Location = new Point(20, 380),
            Size = new Size(370, 23),
            Text = string.Empty,
            Enabled = false,
            ReadOnly = true,
            Font = new Font("Segoe UI", 9F)
        };

        // Browse button
        var btnBrowseFile = new Button
        {
            Name = "btnBrowseFile",
            Text = "Browse…",
            Location = new Point(400, 378),
            Size = new Size(80, 26),
            Enabled = false,
            Font = new Font("Segoe UI", 9F)
        };
        btnBrowseFile.Click += BtnBrowseFile_Click;
        
        // Button to save/update text
        var btnUpdate = new Button
        {
            Text = "Save",
            Location = new Point(20, 415),
            Size = new Size(100, 30),
            Font = new Font("Segoe UI", 9F)
        };
        btnUpdate.Click += BtnUpdate_Click;
        
        // Minimize to tray button
        var btnMinimize = new Button
        {
            Text = "Minimize to Tray",
            Location = new Point(20, 455),
            Size = new Size(150, 30),
            Font = new Font("Segoe UI", 9F)
        };
        btnMinimize.Click += BtnMinimize_Click;

        // Stop typing button
        var btnStop = new Button
        {
            Name = "btnStop",
            Text = "Stop Typing",
            Location = new Point(20, 495),
            Size = new Size(150, 30),
            Font = new Font("Segoe UI", 9F),
            Enabled = false // Enabled only while typing is in progress
        };
        btnStop.Click += BtnStop_Click;
        
        // Status label
        var lblStatus = new Label
        {
            Name = "lblStatus",
            Text = "Status: Hotkey CTRL+SHIFT+1 is active",
            Location = new Point(180, 420),
            Size = new Size(300, 20),
            Font = new Font("Segoe UI", 9F)
        };
        
        // Add controls to form
        Controls.AddRange(new Control[] { 
            lblInstructions,
            lblSnippet,
            cmbSnippets,
            btnNewSnippet,
            btnDuplicateSnippet,
            btnRenameSnippet,
            btnDeleteSnippet,
            txtPredefinedText, 
            lblTypingSpeed,
            sliderTypingSpeed,
            lblSpeedIndicator,
            chkHasCode,
            chkUseFile,
            txtFilePath,
            btnBrowseFile,
            btnUpdate,
            btnMinimize,
            btnStop,
            lblStatus
        });
        
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
