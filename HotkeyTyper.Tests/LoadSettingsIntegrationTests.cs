using System.Text.Json;
using Xunit;

namespace HotkeyTyper.Tests;

public class LoadSettingsIntegrationTests
{
    [Fact]
    public void LoadSettings_FirstRun_HotkeyIsNull()
    {
        // Arrange - Simulate first run with no settings file (defaults will be used)
        var newSettings = new AppSettings();
        
        // Assert - Hotkey should be null on first run
        Assert.Null(newSettings.Hotkey);
    }
    
    [Fact]
    public void LoadSettings_NoHotkeyInJson_LoadsAsNull()
    {
        // Arrange - Simulate loading settings without a hotkey property
        string json = @"{
            ""TypingSpeed"": 5,
            ""HasCode"": false,
            ""Snippets"": [
                {
                    ""Id"": ""default"",
                    ""Name"": ""Default"",
                    ""Content"": ""Hello"",
                    ""LastUsed"": ""2025-01-01T00:00:00""
                }
            ],
            ""ActiveSnippetId"": ""default""
        }";
        
        // Act
        var settings = JsonSerializer.Deserialize<AppSettings>(json);
        
        // Assert
        Assert.NotNull(settings);
        Assert.Null(settings.Hotkey);
    }

    [Fact]
    public void LoadSettings_Scenario_EmptyContentInSnippet()
    {
        // Arrange - Simulate the exact scenario from settings.json
        string tempFile = Path.Combine(Path.GetTempPath(), $"test_empty_content_{Guid.NewGuid()}.json");
        
     var settingsToSave = new AppSettings
{
       PredefinedText = "",
     TypingSpeed = 9,
 HasCode = false,
            LastNonCodeSpeed = 10,
       UseFileSource = false,
            FileSourcePath = "",
         Snippets = new List<TextSnippet>
        {
     new TextSnippet
  {
  Id = "default",
          Name = "Default",
      Content = "", // Empty content - this is the issue!
         LastUsed = DateTime.Parse("2025-11-02T10:01:03.9555672-08:00")
 }
    },
          ActiveSnippetId = "default"
  };
        
        try
        {
            // Save settings
            string json = JsonSerializer.Serialize(settingsToSave, new JsonSerializerOptions { WriteIndented = true });
      File.WriteAllText(tempFile, json);
            
            // Load settings
         string loadedJson = File.ReadAllText(tempFile);
            var loadedSettings = JsonSerializer.Deserialize<AppSettings>(loadedJson);
        
            // Verify
       Assert.NotNull(loadedSettings);
          Assert.NotNull(loadedSettings.Snippets);
      Assert.Single(loadedSettings.Snippets);
    
     var snippet = loadedSettings.Snippets[0];
  Assert.Equal("default", snippet.Id);
      Assert.Equal("Default", snippet.Name);
        Assert.Equal("", snippet.Content); // Empty content should load fine
   Assert.Equal("default", loadedSettings.ActiveSnippetId);
            
            // Simulate what Form1.LoadSettings does
 var snippets = loadedSettings.Snippets;
       string? activeSnippetId = loadedSettings.ActiveSnippetId;
   
      // Validate active snippet exists
          if (string.IsNullOrEmpty(activeSnippetId) || !snippets.Any(s => s.Id == activeSnippetId))
    {
     activeSnippetId = snippets.FirstOrDefault()?.Id;
         }
            
          Assert.Equal("default", activeSnippetId);
    
            // Get active snippet
 var activeSnippet = snippets.FirstOrDefault(s => s.Id == activeSnippetId);
         Assert.NotNull(activeSnippet);
            Assert.Equal("", activeSnippet.Content); // Should be empty string, not null
        }
      finally
        {
      if (File.Exists(tempFile))
   {
                File.Delete(tempFile);
  }
        }
    }
    
    [Fact]
    public void LoadSettings_AfterUpdate_ContentShouldPersist()
    {
        // Simulate the workflow: Load -> Update Content -> Save -> Load Again
      string tempFile = Path.Combine(Path.GetTempPath(), $"test_update_workflow_{Guid.NewGuid()}.json");
        
      try
  {
          // Initial save with empty content
var initialSettings = new AppSettings
       {
   PredefinedText = "",
      TypingSpeed = 5,
       HasCode = false,
   LastNonCodeSpeed = 10,
      UseFileSource = false,
                FileSourcePath = "",
             Snippets = new List<TextSnippet>
            {
     new TextSnippet
          {
    Id = "default",
 Name = "Default",
   Content = "",
         LastUsed = DateTime.Now
      }
    },
     ActiveSnippetId = "default"
            };
   
       string json = JsonSerializer.Serialize(initialSettings, new JsonSerializerOptions { WriteIndented = true });
 File.WriteAllText(tempFile, json);
    
  // Load settings
     json = File.ReadAllText(tempFile);
      var loadedSettings = JsonSerializer.Deserialize<AppSettings>(json);
            Assert.NotNull(loadedSettings);
      Assert.NotNull(loadedSettings.Snippets);
            
 // Simulate user updating content
  var activeSnippet = loadedSettings.Snippets.FirstOrDefault(s => s.Id == loadedSettings.ActiveSnippetId);
            Assert.NotNull(activeSnippet);
  activeSnippet.Content = "Updated content from user!";
   activeSnippet.LastUsed = DateTime.Now;
    
      // Save again
            json = JsonSerializer.Serialize(loadedSettings, new JsonSerializerOptions { WriteIndented = true });
  File.WriteAllText(tempFile, json);
        
            // Load once more to verify persistence
      json = File.ReadAllText(tempFile);
      var finalSettings = JsonSerializer.Deserialize<AppSettings>(json);
            Assert.NotNull(finalSettings);
            Assert.NotNull(finalSettings.Snippets);
            
   var finalSnippet = finalSettings.Snippets.FirstOrDefault(s => s.Id == finalSettings.ActiveSnippetId);
            Assert.NotNull(finalSnippet);
   Assert.Equal("Updated content from user!", finalSnippet.Content);
     }
        finally
        {
            if (File.Exists(tempFile))
       {
   File.Delete(tempFile);
     }
  }
 }
}
