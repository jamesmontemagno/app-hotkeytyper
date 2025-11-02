using System.Text.Json;
using Xunit;

namespace HotkeyTyper.Tests;

public class LoadSaveTests
{
    [Fact]
    public void SaveSettings_ThenLoad_PreservesAllData()
    {
        // Arrange
        string tempFile = Path.Combine(Path.GetTempPath(), $"test_settings_{Guid.NewGuid()}.json");

        var originalSettings = new AppSettings
        {
            PredefinedText = "",
            TypingSpeed = 7,
            HasCode = true,
            LastNonCodeSpeed = 9,
            UseFileSource = false,
            FileSourcePath = "",
            Snippets = new List<TextSnippet>
            {
                new TextSnippet
                {
                    Id = "test-1",
                    Name = "Test Snippet 1",
                    Content = "Test content 1",
                    LastUsed = DateTime.Now
                },
                new TextSnippet
                {
                    Id = "test-2",
                    Name = "Test Snippet 2",
                    Content = "Test content 2\nWith multiple lines",
                    LastUsed = DateTime.Now
                }
            },
            ActiveSnippetId = "test-2"
        };

        try
        {
            // Act - Save
            string json = JsonSerializer.Serialize(originalSettings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(tempFile, json);

            // Act - Load
            string loadedJson = File.ReadAllText(tempFile);
            var loadedSettings = JsonSerializer.Deserialize<AppSettings>(loadedJson);

            // Assert
            Assert.NotNull(loadedSettings);
            Assert.Equal(originalSettings.TypingSpeed, loadedSettings.TypingSpeed);
            Assert.Equal(originalSettings.HasCode, loadedSettings.HasCode);
            Assert.Equal(originalSettings.LastNonCodeSpeed, loadedSettings.LastNonCodeSpeed);
            Assert.Equal(originalSettings.UseFileSource, loadedSettings.UseFileSource);
            Assert.Equal(originalSettings.FileSourcePath, loadedSettings.FileSourcePath);
            Assert.Equal(originalSettings.ActiveSnippetId, loadedSettings.ActiveSnippetId);

            Assert.NotNull(loadedSettings.Snippets);
            Assert.Equal(2, loadedSettings.Snippets.Count);
            Assert.Equal("Test Snippet 1", loadedSettings.Snippets[0].Name);
            Assert.Equal("Test content 1", loadedSettings.Snippets[0].Content);
            Assert.Equal("Test Snippet 2", loadedSettings.Snippets[1].Name);
            Assert.Equal("Test content 2\nWith multiple lines", loadedSettings.Snippets[1].Content);
        }
        finally
        {
            // Cleanup
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void LoadSettings_WithEmptyContent_StillLoadsSnippet()
    {
        // Arrange - This is the scenario from the actual settings.json
        string json = @"{
  ""PredefinedText"": """",
  ""TypingSpeed"": 9,
  ""HasCode"": false,
  ""LastNonCodeSpeed"": 10,
  ""UseFileSource"": false,
  ""FileSourcePath"": """",
  ""Snippets"": [
    {
      ""Id"": ""default"",
      ""Name"": ""Default"",
      ""Content"": """",
 ""LastUsed"": ""2025-11-02T10:01:03.9555672-08:00""
    }
  ],
  ""ActiveSnippetId"": ""default""
}";
        
 // Act
        var settings = JsonSerializer.Deserialize<AppSettings>(json);
    
        // Assert
        Assert.NotNull(settings);
        Assert.NotNull(settings.Snippets);
  Assert.Single(settings.Snippets);
        Assert.Equal("default", settings.Snippets[0].Id);
        Assert.Equal("Default", settings.Snippets[0].Name);
     Assert.Equal("", settings.Snippets[0].Content); // Empty content is valid!
      Assert.Equal("default", settings.ActiveSnippetId);
    }
    
    [Fact]
    public void LoadSettings_WithContent_LoadsCorrectly()
    {
      // Arrange
        string json = @"{
  ""PredefinedText"": """",
  ""TypingSpeed"": 5,
  ""HasCode"": false,
  ""LastNonCodeSpeed"": 10,
  ""UseFileSource"": false,
  ""FileSourcePath"": """",
  ""Snippets"": [
    {
      ""Id"": ""snippet1"",
      ""Name"": ""My Snippet"",
      ""Content"": ""Hello World!"",
   ""LastUsed"": ""2025-01-01T00:00:00""
    }
  ],
  ""ActiveSnippetId"": ""snippet1""
}";
     
        // Act
        var settings = JsonSerializer.Deserialize<AppSettings>(json);
    
  // Assert
        Assert.NotNull(settings);
        Assert.NotNull(settings.Snippets);
        Assert.Single(settings.Snippets);
        Assert.Equal("snippet1", settings.Snippets[0].Id);
        Assert.Equal("My Snippet", settings.Snippets[0].Name);
        Assert.Equal("Hello World!", settings.Snippets[0].Content);
      Assert.Equal("snippet1", settings.ActiveSnippetId);
    }
}
