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
        ClientSize = new Size(500, 340);
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
            Size = new Size(460, 40),
            Font = new Font("Segoe UI", 10F, FontStyle.Regular)
        };
        
        // TextBox for predefined text
        var txtPredefinedText = new TextBox
        {
            Name = "txtPredefinedText",
            Text = predefinedText,
            Location = new Point(20, 80),
            Size = new Size(460, 100),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Segoe UI", 9F)
        };
        
        // Typing speed label
        var lblTypingSpeed = new Label
        {
            Text = "Typing Speed:",
            Location = new Point(20, 190),
            Size = new Size(80, 20),
            Font = new Font("Segoe UI", 9F)
        };
        
        // Typing speed slider
        var sliderTypingSpeed = new TrackBar
        {
            Name = "sliderTypingSpeed",
            Location = new Point(105, 185),
            Size = new Size(200, 45),
            Minimum = 1,
            Maximum = 10,
            Value = typingSpeed,
            TickFrequency = 1,
            TickStyle = TickStyle.BottomRight
        };
        sliderTypingSpeed.ValueChanged += TypingSpeedSlider_ValueChanged;
        
        // Speed indicator label
        var lblSpeedIndicator = new Label
        {
            Name = "lblSpeedIndicator",
            Text = GetSpeedText(typingSpeed),
            Location = new Point(315, 190),
            Size = new Size(100, 20),
            Font = new Font("Segoe UI", 9F, FontStyle.Italic)
        };
        
        // Button to update text
        var btnUpdate = new Button
        {
            Text = "Update Text",
            Location = new Point(20, 220),
            Size = new Size(100, 30),
            Font = new Font("Segoe UI", 9F)
        };
        btnUpdate.Click += BtnUpdate_Click;
        
        // Minimize to tray button
        var btnMinimize = new Button
        {
            Text = "Minimize to Tray",
            Location = new Point(20, 260),
            Size = new Size(100, 30),
            Font = new Font("Segoe UI", 9F)
        };
        btnMinimize.Click += BtnMinimize_Click;
        
        // Status label
        var lblStatus = new Label
        {
            Text = "Status: Hotkey CTRL+SHIFT+1 is active",
            Location = new Point(140, 227),
            Size = new Size(300, 20),
            Font = new Font("Segoe UI", 9F),
            ForeColor = Color.Green
        };
        
        // Add controls to form
        Controls.AddRange(new Control[] { 
            lblInstructions, 
            txtPredefinedText, 
            lblTypingSpeed,
            sliderTypingSpeed,
            lblSpeedIndicator,
            btnUpdate, 
            btnMinimize,
            lblStatus
        });
        
        // Store references for later use
        this.txtPredefinedText = txtPredefinedText;
        this.sliderTypingSpeed = sliderTypingSpeed;
        this.lblSpeedIndicator = lblSpeedIndicator;
    }
    
    private TextBox txtPredefinedText;
    private TrackBar sliderTypingSpeed;
    private Label lblSpeedIndicator;

    #endregion
}
