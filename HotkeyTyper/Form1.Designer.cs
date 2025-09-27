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
    ClientSize = new Size(500, 540); // Increased further to prevent clipping after additional downward shifts
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
            Text = "Configure your predefined text below.\nPress CTRL+SHIFT+1 anywhere to type it:",
            Location = new Point(20, 20),
            // Increased height to ensure second line isn't clipped on various DPI scales
            Size = new Size(460, 55),
            Font = new Font("Segoe UI", 10F, FontStyle.Regular)
        };
        
        // TextBox for predefined text (replaced with snippet selector and edit area)
        var lblSnippets = new Label
        {
            Text = "Active Snippet:",
            Location = new Point(20, 85),
            Size = new Size(100, 20),
            Font = new Font("Segoe UI", 9F)
        };
        
        // ComboBox for snippet selection
        var cmbSnippets = new ComboBox
        {
            Name = "cmbSnippets",
            Location = new Point(125, 83),
            Size = new Size(200, 23),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9F)
        };
        cmbSnippets.SelectedIndexChanged += CmbSnippets_SelectedIndexChanged;
        
        // Snippet management buttons
        var btnNewSnippet = new Button
        {
            Name = "btnNewSnippet",
            Text = "New",
            Location = new Point(335, 82),
            Size = new Size(50, 25),
            Font = new Font("Segoe UI", 8F)
        };
        btnNewSnippet.Click += BtnNewSnippet_Click;
        
        var btnDuplicateSnippet = new Button
        {
            Name = "btnDuplicateSnippet",
            Text = "Copy",
            Location = new Point(390, 82),
            Size = new Size(45, 25),
            Font = new Font("Segoe UI", 8F)
        };
        btnDuplicateSnippet.Click += BtnDuplicateSnippet_Click;
        
        var btnDeleteSnippet = new Button
        {
            Name = "btnDeleteSnippet",
            Text = "Del",
            Location = new Point(440, 82),
            Size = new Size(40, 25),
            Font = new Font("Segoe UI", 8F)
        };
        btnDeleteSnippet.Click += BtnDeleteSnippet_Click;
        
        // TextBox for snippet content editing
        var txtPredefinedText = new TextBox
        {
            Name = "txtPredefinedText",
            Text = predefinedText, // This will be updated by UpdateUIFromSettings
            Location = new Point(20, 115),
            Size = new Size(460, 105), // Reduced height to make room for snippet controls
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Segoe UI", 9F)
        };
        
        // Typing speed label
        var lblTypingSpeed = new Label
        {
            Text = "Typing Speed:",
            Location = new Point(20, 230),
            Size = new Size(80, 20),
            Font = new Font("Segoe UI", 9F)
        };
        
        // Typing speed slider (custom for soft limit visuals)
        var sliderTypingSpeed = new LimitedTrackBar
        {
            Name = "sliderTypingSpeed",
            Location = new Point(105, 225),
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
            Location = new Point(315, 230),
            Size = new Size(100, 20),
            Font = new Font("Segoe UI", 9F, FontStyle.Italic)
        };

        // Checkbox: Has Code (limits speed)
        var chkHasCode = new CheckBox
        {
            Name = "chkHasCode",
            Text = "Has Code (limit speed)",
            Location = new Point(20, 290), // Further down to avoid any overlap/cutoff
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
            Location = new Point(220, 290), // Align with Has Code new Y
            Size = new Size(150, 20),
            Checked = false,
            Font = new Font("Segoe UI", 9F)
        };
        chkUseFile.CheckedChanged += ChkUseFile_CheckedChanged;

        // File path textbox
        var txtFilePath = new TextBox
        {
            Name = "txtFilePath",
            Location = new Point(20, 320), // Shifted down to maintain spacing under checkboxes
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
            Location = new Point(400, 318), // Align with file path new Y
            Size = new Size(80, 26),
            Enabled = false,
            Font = new Font("Segoe UI", 9F)
        };
        btnBrowseFile.Click += BtnBrowseFile_Click;
        
        // Button to update text
        var btnUpdate = new Button
        {
            Text = "Save",
            Location = new Point(20, 355), // Shifted further down
            Size = new Size(100, 30),
            Font = new Font("Segoe UI", 9F)
        };
        btnUpdate.Click += BtnUpdate_Click;
        
        // Minimize to tray button
        var btnMinimize = new Button
        {
            Text = "Minimize to Tray",
            Location = new Point(20, 395), // Shifted down with rest
            // Widened to avoid text clipping on 125%/150% DPI
            Size = new Size(150, 30),
            Font = new Font("Segoe UI", 9F)
        };
        btnMinimize.Click += BtnMinimize_Click;

        // Stop typing button (placed under the minimize button)
        var btnStop = new Button
        {
            Name = "btnStop",
            Text = "Stop Typing",
            Location = new Point(20, 435), // Shifted down to maintain spacing
            Size = new Size(150, 30),
            Font = new Font("Segoe UI", 9F),
            Enabled = false // Enabled only while typing is in progress
        };
        btnStop.Click += BtnStop_Click;
        
        // Status label
        var lblStatus = new Label
        {
            Text = "Status: Hotkey CTRL+SHIFT+1 is active",
            Location = new Point(150, 370), // Repositioned for new layout
            Size = new Size(320, 20),
            Font = new Font("Segoe UI", 9F),
            ForeColor = Color.Green
        };
        
        // Add controls to form
        Controls.AddRange(new Control[] { 
            lblInstructions, 
            lblSnippets,
            cmbSnippets,
            btnNewSnippet,
            btnDuplicateSnippet,
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
        this.btnNewSnippet = btnNewSnippet;
        this.btnDuplicateSnippet = btnDuplicateSnippet;
        this.btnDeleteSnippet = btnDeleteSnippet;
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
    private Button btnNewSnippet;
    private Button btnDuplicateSnippet;
    private Button btnDeleteSnippet;
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
