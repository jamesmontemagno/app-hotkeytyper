using System.Text.Json;
using HotkeyTyper;
using Xunit;

namespace HotkeyTyper.Tests;

public class SnippetMigrationTests
{
    [Fact]
    public void AppSettings_SerializesCorrectly()
    {
        // Arrange
        var settings = new AppSettings
        {
            Snippets = new List<Snippet>
            {
                new Snippet { Id = "test1", Name = "Test 1", Content = "Hello World", LastUsed = DateTime.Parse("2023-01-01") },
                new Snippet { Id = "test2", Name = "Test 2", Content = "Goodbye", LastUsed = DateTime.Parse("2023-01-02") }
            },
            ActiveSnippetId = "test1",
            TypingSpeed = 7,
            HasCode = true,
            UseFileSource = false
        };

        // Act
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        var deserialized = JsonSerializer.Deserialize<AppSettings>(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(2, deserialized.Snippets.Count);
        Assert.Equal("test1", deserialized.ActiveSnippetId);
        Assert.Equal(7, deserialized.TypingSpeed);
        Assert.True(deserialized.HasCode);
        Assert.Equal("Test 1", deserialized.Snippets.First(s => s.Id == "test1").Name);
        Assert.Equal("Hello World", deserialized.Snippets.First(s => s.Id == "test1").Content);
    }

    [Fact]
    public void AppSettings_NewFormat_SnippetsExist()
    {
        // Arrange - new format with snippets
        var newFormatJson = """
        {
          "Snippets": [
            {
              "Id": "snippet1",
              "Name": "Example",
              "Content": "New format content",
              "LastUsed": "2023-01-01T12:00:00"
            }
          ],
          "ActiveSnippetId": "snippet1",
          "TypingSpeed": 5,
          "HasCode": false,
          "LastNonCodeSpeed": 10,
          "UseFileSource": false,
          "FileSourcePath": ""
        }
        """;

        // Act
        var settings = JsonSerializer.Deserialize<AppSettings>(newFormatJson);

        // Assert
        Assert.NotNull(settings);
        Assert.Single(settings.Snippets);
        Assert.Equal("snippet1", settings.ActiveSnippetId);
        Assert.Equal("Example", settings.Snippets[0].Name);
        Assert.Equal("New format content", settings.Snippets[0].Content);
    }

    [Fact]
    public void Snippet_ToString_ReturnsName()
    {
        // Arrange
        var snippet = new Snippet
        {
            Id = "test",
            Name = "My Snippet",
            Content = "Some content"
        };

        // Act & Assert
        Assert.Equal("My Snippet", snippet.ToString());
    }
}