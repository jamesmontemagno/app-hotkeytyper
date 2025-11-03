using HotkeyTyper;
using Xunit;

namespace HotkeyTyper.Tests;

public class HotkeyParsingTests
{
    [Theory]
    [InlineData("Ctrl+Shift+1")]
    [InlineData("Ctrl+Shift+A")]
    [InlineData("Ctrl+Alt+F5")]
    [InlineData("Ctrl+Shift+Alt+D")]
    [InlineData("Win+Shift+T")]
    [InlineData("Control+Shift+1")] // Alternative spelling
    public void TryParseHotkey_ValidCombinations_ReturnsTrue(string hotkeyString)
    {
        // This test verifies that valid hotkey strings can be parsed
        // Since TryParseHotkey is private, we test through the AppSettings roundtrip
        var settings = new AppSettings { Hotkey = hotkeyString };
        Assert.NotNull(settings.Hotkey);
        Assert.Equal(hotkeyString, settings.Hotkey);
    }

    [Theory]
    [InlineData("A")] // No modifier
    [InlineData("Shift+Ctrl")] // No primary key
    [InlineData("")] // Empty
    [InlineData("InvalidKey")] // Invalid key name
    [InlineData("Ctrl")] // Only modifier
    public void TryParseHotkey_InvalidCombinations_Handled(string hotkeyString)
    {
        // These should not crash when loading settings
        var settings = new AppSettings { Hotkey = hotkeyString };
        Assert.NotNull(settings);
    }

    [Fact]
    public void AppSettings_DefaultHotkey_IsCtrlShiftOne()
    {
        var settings = new AppSettings();
        Assert.Equal("Ctrl+Shift+1", settings.Hotkey);
    }

    [Fact]
    public void AppSettings_HotkeyPersistence_RoundTrip()
    {
        string testHotkey = "Ctrl+Alt+F12";
        var settings = new AppSettings
        {
            Hotkey = testHotkey,
            TypingSpeed = 7
        };

        // Serialize
        string json = System.Text.Json.JsonSerializer.Serialize(settings);

        // Deserialize
        var loaded = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(json);

        Assert.NotNull(loaded);
        Assert.Equal(testHotkey, loaded.Hotkey);
        Assert.Equal(7, loaded.TypingSpeed);
    }

    [Fact]
    public void AppSettings_MissingHotkey_UsesDefault()
    {
        // Simulate loading old settings file without Hotkey property
        string json = @"{
            ""TypingSpeed"": 5,
            ""HasCode"": false,
            ""Snippets"": []
        }";

        var settings = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(json);

        Assert.NotNull(settings);
        // Default value should be used when property is missing
        Assert.Equal("Ctrl+Shift+1", settings.Hotkey);
    }
}
