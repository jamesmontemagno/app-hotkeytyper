using System.Text.Json;
using Xunit;

namespace HotkeyTyper.Tests;

/// <summary>
/// End-to-end tests that simulate real user workflows with the settings system.
/// </summary>
public class EndToEndWorkflowTests
{
    [Fact]
    public void CompleteUserWorkflow_CreateEditSaveReload()
    {
        // Simulate a complete user session:
        // 1. First launch (no settings file exists)
    // 2. User types some content
        // 3. User saves
        // 4. App closes
   // 5. App reopens
        // 6. Content should still be there
   
        string tempFile = Path.Combine(Path.GetTempPath(), $"workflow_test_{Guid.NewGuid()}.json");
        
        try
        {
         // === SESSION 1: First Launch ===
  // Simulate LoadSettings when file doesn't exist
            AppSettings settings1;
       if (!File.Exists(tempFile))
{
        // App creates default snippet
           settings1 = new AppSettings
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
 Content = "Hello, this is my predefined text!", // Default content
            LastUsed = DateTime.Now
      }
         },
        ActiveSnippetId = "default"
     };
   }
       else
   {
           throw new Exception("Test setup error: temp file already exists");
   }
            
     // User types new content
      var activeSnippet = settings1.Snippets.FirstOrDefault(s => s.Id == settings1.ActiveSnippetId);
  Assert.NotNull(activeSnippet);
   activeSnippet.Content = "This is my custom text that I typed!";
 activeSnippet.LastUsed = DateTime.Now;
        
         // User clicks "Save"
            string json1 = JsonSerializer.Serialize(settings1, new JsonSerializerOptions { WriteIndented = true });
      File.WriteAllText(tempFile, json1);
            
       // Verify file was created
  Assert.True(File.Exists(tempFile));

            // === SESSION 2: App Restart ===
            // Simulate LoadSettings on next launch
            string json2 = File.ReadAllText(tempFile);
          var settings2 = JsonSerializer.Deserialize<AppSettings>(json2);
        
         Assert.NotNull(settings2);
     Assert.NotNull(settings2.Snippets);
            Assert.Single(settings2.Snippets);
            
            var loadedSnippet = settings2.Snippets.FirstOrDefault(s => s.Id == settings2.ActiveSnippetId);
     Assert.NotNull(loadedSnippet);
            Assert.Equal("This is my custom text that I typed!", loadedSnippet.Content);
          
     // === SESSION 3: User Edits Again ===
        loadedSnippet.Content = "Updated text after restart";
      loadedSnippet.LastUsed = DateTime.Now;
          
          // Save again
            string json3 = JsonSerializer.Serialize(settings2, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(tempFile, json3);
    
         // === SESSION 4: Verify Final State ===
            string json4 = File.ReadAllText(tempFile);
            var settings4 = JsonSerializer.Deserialize<AppSettings>(json4);
        
            Assert.NotNull(settings4);
 var finalSnippet = settings4.Snippets.FirstOrDefault(s => s.Id == settings4.ActiveSnippetId);
     Assert.NotNull(finalSnippet);
            Assert.Equal("Updated text after restart", finalSnippet.Content);
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
    public void MultipleSnippetsWorkflow_SwitchingAndSaving()
    {
        // Test switching between multiple snippets
      string tempFile = Path.Combine(Path.GetTempPath(), $"multi_snippet_test_{Guid.NewGuid()}.json");
        
        try
        {
// Create initial settings with two snippets
            var settings = new AppSettings
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
                  Id = "snippet1",
     Name = "Greeting",
      Content = "Hello, World!",
          LastUsed = DateTime.Now
      },
            new TextSnippet
        {
        Id = "snippet2",
             Name = "Signature",
    Content = "Best regards,\nJohn Doe",
      LastUsed = DateTime.Now
       }
          },
          ActiveSnippetId = "snippet1"
            };
        
            // Save initial state
     string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(tempFile, json);
            
       // Load settings
   json = File.ReadAllText(tempFile);
            var loadedSettings = JsonSerializer.Deserialize<AppSettings>(json);
   Assert.NotNull(loadedSettings);
   
            // Verify first snippet is active
         Assert.Equal("snippet1", loadedSettings.ActiveSnippetId);
         var active1 = loadedSettings.Snippets.FirstOrDefault(s => s.Id == loadedSettings.ActiveSnippetId);
    Assert.NotNull(active1);
   Assert.Equal("Hello, World!", active1.Content);
   
            // User switches to second snippet
            loadedSettings.ActiveSnippetId = "snippet2";
      
    // Save after switch
            json = JsonSerializer.Serialize(loadedSettings, new JsonSerializerOptions { WriteIndented = true });
       File.WriteAllText(tempFile, json);
      
            // Reload and verify
 json = File.ReadAllText(tempFile);
            var finalSettings = JsonSerializer.Deserialize<AppSettings>(json);
            Assert.NotNull(finalSettings);
       Assert.Equal("snippet2", finalSettings.ActiveSnippetId);
  
            var active2 = finalSettings.Snippets.FirstOrDefault(s => s.Id == finalSettings.ActiveSnippetId);
      Assert.NotNull(active2);
            Assert.Equal("Best regards,\nJohn Doe", active2.Content);
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
    public void SettingsPersistence_AllFields()
    {
 // Verify all settings fields persist correctly
      string tempFile = Path.Combine(Path.GetTempPath(), $"all_fields_test_{Guid.NewGuid()}.json");
        
 try
        {
            var settings = new AppSettings
         {
        PredefinedText = "", // Deprecated but should still serialize
    TypingSpeed = 8,
         HasCode = true,
      LastNonCodeSpeed = 9,
           UseFileSource = true,
   FileSourcePath = @"C:\test\file.txt",
          Snippets = new List<TextSnippet>
       {
        new TextSnippet
            {
       Id = "test-id",
    Name = "Test Snippet",
    Content = "Test Content\nLine 2\nLine 3",
       LastUsed = new DateTime(2025, 1, 15, 10, 30, 0)
          }
    },
       ActiveSnippetId = "test-id"
 };
          
            // Save
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
   File.WriteAllText(tempFile, json);
            
    // Load
            json = File.ReadAllText(tempFile);
            var loaded = JsonSerializer.Deserialize<AppSettings>(json);
 
            // Verify all fields
Assert.NotNull(loaded);
      Assert.Equal(8, loaded.TypingSpeed);
      Assert.True(loaded.HasCode);
       Assert.Equal(9, loaded.LastNonCodeSpeed);
        Assert.True(loaded.UseFileSource);
         Assert.Equal(@"C:\test\file.txt", loaded.FileSourcePath);
  Assert.Equal("test-id", loaded.ActiveSnippetId);
     
            Assert.NotNull(loaded.Snippets);
            Assert.Single(loaded.Snippets);
          Assert.Equal("test-id", loaded.Snippets[0].Id);
            Assert.Equal("Test Snippet", loaded.Snippets[0].Name);
        Assert.Equal("Test Content\nLine 2\nLine 3", loaded.Snippets[0].Content);
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
