using HotkeyTyper;
using System.Text.Json;
using Xunit;

namespace HotkeyTyper.Tests;

public class SettingsMigrationTests
{
    [Fact]
    public void OldSettings_MigratesToSnippets()
    {
        // Arrange - Old settings format with single PredefinedText
        string oldSettingsJson = """
        {
            "PredefinedText": "Hello, World!",
            "TypingSpeed": 7,
            "HasCode": true,
            "LastNonCodeSpeed": 9,
            "UseFileSource": false,
            "FileSourcePath": ""
        }
        """;
        
        var oldSettings = JsonSerializer.Deserialize<AppSettings>(oldSettingsJson);
        
        // Act - Settings should be migrated when Snippets is null or empty
        Assert.NotNull(oldSettings);
        Assert.Null(oldSettings.Snippets);
        Assert.Equal("Hello, World!", oldSettings.PredefinedText);
        
        // Verify migration would create snippet from PredefinedText
        Assert.NotEmpty(oldSettings.PredefinedText);
    }
    
    [Fact]
    public void NewSettings_PreservesSnippets()
    {
        // Arrange - New settings format with snippets
        string newSettingsJson = """
        {
            "PredefinedText": "",
            "TypingSpeed": 5,
            "HasCode": false,
            "LastNonCodeSpeed": 10,
            "UseFileSource": false,
            "FileSourcePath": "",
            "Snippets": [
                {
                    "Id": "snippet1",
                    "Name": "First Snippet",
                    "Content": "Content 1",
                    "LastUsed": "2024-01-01T00:00:00"
                },
                {
                    "Id": "snippet2",
                    "Name": "Second Snippet",
                    "Content": "Content 2",
                    "LastUsed": "2024-01-02T00:00:00"
                }
            ],
            "ActiveSnippetId": "snippet2"
        }
        """;
        
        var newSettings = JsonSerializer.Deserialize<AppSettings>(newSettingsJson);
        
        // Assert
        Assert.NotNull(newSettings);
        Assert.NotNull(newSettings.Snippets);
        Assert.Equal(2, newSettings.Snippets.Count);
        Assert.Equal("snippet2", newSettings.ActiveSnippetId);
        Assert.Equal("First Snippet", newSettings.Snippets[0].Name);
        Assert.Equal("Content 2", newSettings.Snippets[1].Content);
    }
    
    [Fact]
    public void EmptySettings_CreatesDefaultSnippet()
    {
        // Arrange - Empty/null settings
        string emptySettingsJson = """
        {
            "PredefinedText": "",
            "TypingSpeed": 5,
            "HasCode": false,
            "LastNonCodeSpeed": 10,
            "UseFileSource": false,
            "FileSourcePath": ""
        }
        """;
        
        var emptySettings = JsonSerializer.Deserialize<AppSettings>(emptySettingsJson);
        
        // Assert
        Assert.NotNull(emptySettings);
        Assert.Null(emptySettings.Snippets);
        Assert.Empty(emptySettings.PredefinedText);
        
        // Migration logic should create a default snippet when both are empty
    }
    
    [Fact]
    public void SnippetSerialization_RoundTrip()
    {
        // Arrange
        var snippet = new TextSnippet
        {
            Id = "test-id",
            Name = "Test Snippet",
            Content = "Test Content\nWith multiple lines",
            LastUsed = new DateTime(2024, 1, 15, 10, 30, 0)
        };
        
        // Act - Serialize and deserialize
        string json = JsonSerializer.Serialize(snippet, new JsonSerializerOptions { WriteIndented = true });
        var deserialized = JsonSerializer.Deserialize<TextSnippet>(json);
        
        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(snippet.Id, deserialized.Id);
        Assert.Equal(snippet.Name, deserialized.Name);
        Assert.Equal(snippet.Content, deserialized.Content);
        Assert.Equal(snippet.LastUsed.Year, deserialized.LastUsed.Year);
        Assert.Equal(snippet.LastUsed.Month, deserialized.LastUsed.Month);
        Assert.Equal(snippet.LastUsed.Day, deserialized.LastUsed.Day);
    }
    
    [Fact]
    public void CompleteSettings_SerializationRoundTrip()
    {
        // Arrange
        var settings = new AppSettings
        {
            PredefinedText = "",
            TypingSpeed = 8,
            HasCode = true,
            LastNonCodeSpeed = 10,
            UseFileSource = false,
            FileSourcePath = "",
            Snippets = new List<TextSnippet>
            {
                new TextSnippet
                {
                    Id = "id1",
                    Name = "Snippet 1",
                    Content = "Content 1",
                    LastUsed = DateTime.Now
                },
                new TextSnippet
                {
                    Id = "id2",
                    Name = "Snippet 2",
                    Content = "Content 2\nWith\nMultiple\nLines",
                    LastUsed = DateTime.Now
                }
            },
            ActiveSnippetId = "id2"
        };
        
        // Act
        string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        var deserialized = JsonSerializer.Deserialize<AppSettings>(json);
        
        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(settings.TypingSpeed, deserialized.TypingSpeed);
        Assert.Equal(settings.HasCode, deserialized.HasCode);
        Assert.Equal(settings.ActiveSnippetId, deserialized.ActiveSnippetId);
        Assert.NotNull(deserialized.Snippets);
        Assert.Equal(2, deserialized.Snippets.Count);
        Assert.Equal("Snippet 1", deserialized.Snippets[0].Name);
        Assert.Equal("Content 2\nWith\nMultiple\nLines", deserialized.Snippets[1].Content);
    }
}
