using System.Text.Json;

namespace HotkeyTyper;

/// <summary>
/// Manages application theme preferences.
/// </summary>
internal static class ThemeManager
{
    private static readonly string PreferencesFilePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, 
        "preferences.json");

    private static ThemePreferences? _currentPreferences;

    /// <summary>
    /// Gets the current theme mode setting.
    /// </summary>
    public static ThemeMode CurrentTheme => _currentPreferences?.Theme ?? ThemeMode.System;

    /// <summary>
    /// Loads theme preferences from disk.
    /// </summary>
    public static void LoadPreferences()
    {
        try
        {
            if (File.Exists(PreferencesFilePath))
            {
                string json = File.ReadAllText(PreferencesFilePath);
                _currentPreferences = JsonSerializer.Deserialize<ThemePreferences>(json);
            }
            else
            {
                _currentPreferences = new ThemePreferences();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading preferences: {ex.Message}");
            _currentPreferences = new ThemePreferences();
        }
    }

    /// <summary>
    /// Saves theme preferences to disk.
    /// </summary>
    public static void SavePreferences()
    {
        try
        {
            if (_currentPreferences == null)
            {
                _currentPreferences = new ThemePreferences();
            }

            string json = JsonSerializer.Serialize(_currentPreferences, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(PreferencesFilePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving preferences: {ex.Message}");
        }
    }

    /// <summary>
    /// Sets the theme mode and persists the preference.
    /// </summary>
    /// <param name="mode">The theme mode to set.</param>
    public static void SetTheme(ThemeMode mode)
    {
        if (_currentPreferences == null)
        {
            _currentPreferences = new ThemePreferences();
        }

        _currentPreferences.Theme = mode;
        SavePreferences();
    }

    /// <summary>
    /// Applies the current theme to the application.
    /// </summary>
    public static void ApplyTheme()
    {
        var systemColorMode = CurrentTheme switch
        {
            ThemeMode.Light => SystemColorMode.Classic,
            ThemeMode.Dark => SystemColorMode.Dark,
            ThemeMode.System => SystemColorMode.System,
            _ => SystemColorMode.System
        };

        Application.SetColorMode(systemColorMode);
    }
}

/// <summary>
/// Theme mode options.
/// </summary>
public enum ThemeMode
{
    System = 0,
    Light = 1,
    Dark = 2
}

/// <summary>
/// Preferences class for JSON serialization.
/// </summary>
public class ThemePreferences
{
    public ThemeMode Theme { get; set; } = ThemeMode.System;
}
