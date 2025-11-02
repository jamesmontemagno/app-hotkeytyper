namespace HotkeyTyper;

/// <summary>
/// Centralized color scheme for the application with dark mode support.
/// Provides cached color instances to avoid repeated Color.FromArgb allocations.
/// </summary>
internal static class AppColors
{
    // ===== Dark Mode Detection =====

    /// <summary>
    /// Gets whether the application is currently in dark mode.
    /// </summary>
    public static bool IsDarkMode => Application.IsDarkModeEnabled;

    // ===== Background Colors =====

    /// <summary>
    /// Primary background color for dialogs and forms.
    /// Dark: #202020, Light: SystemColors.Control
    /// </summary>
    public static Color Background => IsDarkMode ? DarkBackground : SystemColors.Control;

    private static readonly Color DarkBackground = Color.FromArgb(32, 32, 32);

    /// <summary>
    /// Secondary background color for input controls like TextBox.
    /// Dark: #2D2D30, Light: SystemColors.Window
    /// </summary>
    public static Color InputBackground => IsDarkMode ? DarkInputBackground : SystemColors.Window;

    private static readonly Color DarkInputBackground = Color.FromArgb(45, 45, 48);

    /// <summary>
    /// Button background color in dark mode.
    /// Dark: #2D2D30, Light: Uses default button styling
    /// </summary>
    public static Color ButtonBackground => IsDarkMode ? DarkInputBackground : SystemColors.Control;

    // ===== Foreground/Text Colors =====

    /// <summary>
    /// Primary text color for labels and static text.
    /// Dark: #F0F0F0, Light: SystemColors.ControlText
    /// </summary>
    public static Color Text => IsDarkMode ? DarkTextPrimary : SystemColors.ControlText;

    private static readonly Color DarkTextPrimary = Color.FromArgb(240, 240, 240);

    /// <summary>
    /// Secondary text color for less prominent text.
    /// Dark: #C8C8C8, Light: SystemColors.ControlText
    /// </summary>
    public static Color TextSecondary => IsDarkMode ? DarkTextSecondary : SystemColors.ControlText;

    private static readonly Color DarkTextSecondary = Color.FromArgb(200, 200, 200);

    /// <summary>
    /// Tertiary text color for hints and disabled-looking text.
    /// Dark: #B4B4B4, Light: SystemColors.GrayText
    /// </summary>
    public static Color TextTertiary => IsDarkMode ? DarkTextTertiary : SystemColors.GrayText;

    private static readonly Color DarkTextTertiary = Color.FromArgb(180, 180, 180);

    /// <summary>
    /// Text color for input controls like TextBox.
    /// Dark: #DCDCDC, Light: SystemColors.WindowText
    /// </summary>
    public static Color InputText => IsDarkMode ? DarkInputText : SystemColors.WindowText;

    private static readonly Color DarkInputText = Color.FromArgb(220, 220, 220);

    // ===== Border Colors =====

    /// <summary>
    /// Border color for controls in dark mode.
    /// Dark: #555555, Light: Uses default border
    /// </summary>
    public static Color Border => IsDarkMode ? DarkBorder : SystemColors.ControlDark;

    private static readonly Color DarkBorder = Color.FromArgb(85, 85, 85);

    // ===== Accent/Action Colors =====

    /// <summary>
    /// Primary accent color for buttons and interactive elements.
    /// Consistent across light and dark modes.
    /// </summary>
    public static Color Accent { get; } = Color.FromArgb(0, 120, 215);

    /// <summary>
    /// Darker shade of accent for hover/pressed states.
    /// </summary>
    public static Color AccentDark { get; } = Color.FromArgb(0, 84, 153);

    /// <summary>
    /// Button foreground color in dark mode (for accent buttons).
    /// </summary>
    public static Color AccentText { get; } = Color.White;

    // ===== Status Colors =====

    /// <summary>
    /// Success/positive state color.
    /// Dark: LightGreen, Light: Green
    /// </summary>
    public static Color Success => IsDarkMode ? Color.LightGreen : Color.Green;

    /// <summary>
    /// Warning/in-progress state color.
    /// Dark: Orange, Light: DarkOrange
    /// </summary>
    public static Color Warning => IsDarkMode ? Color.Orange : Color.DarkOrange;

    /// <summary>
    /// Error/negative state color.
    /// Dark: IndianRed, Light: Red
    /// </summary>
    public static Color Error => IsDarkMode ? Color.IndianRed : Color.Red;

    // ===== Icon/Theme Colors =====

    /// <summary>
    /// Icon gradient start color.
    /// Light theme: MediumPurple, Dark theme: Lighter purple
    /// </summary>
    public static Color IconGradientStart => IsDarkMode ? IconGradientStartDark : Color.MediumPurple;

    private static readonly Color IconGradientStartDark = Color.FromArgb(200, 162, 235);

    /// <summary>
    /// Icon gradient end color.
    /// Light theme: DeepSkyBlue, Dark theme: Lighter sky blue
    /// </summary>
    public static Color IconGradientEnd => IsDarkMode ? IconGradientEndDark : Color.DeepSkyBlue;

    private static readonly Color IconGradientEndDark = Color.FromArgb(135, 206, 250);

    /// <summary>
    /// Icon border and text color.
    /// Always white/light gray for contrast on gradient background.
    /// </summary>
    public static Color IconBorder => DarkTextPrimary;

    /// <summary>
    /// Icon text color.
    /// </summary>
    public static Color IconText => DarkTextPrimary;

    // ===== TrackBar Colors =====

    /// <summary>
    /// Track bar track color.
    /// </summary>
    public static Color TrackBarTrack { get; } = Color.Gray;

    /// <summary>
    /// Track bar tick color (enabled).
    /// </summary>
    public static Color TrackBarTick { get; } = Color.Black;

    /// <summary>
    /// Track bar tick color (disabled/grayed).
    /// </summary>
    public static Color TrackBarTickDisabled { get; } = Color.LightGray;

    /// <summary>
    /// Track bar thumb (slider) color.
    /// </summary>
    public static Color TrackBarThumb { get; } = Color.DodgerBlue;

    /// <summary>
    /// Track bar thumb outline color.
    /// </summary>
    public static Color TrackBarThumbOutline { get; } = Color.Black;
}
