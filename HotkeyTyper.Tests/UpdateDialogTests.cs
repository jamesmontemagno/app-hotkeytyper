using HotkeyTyper;
using Xunit;

namespace HotkeyTyper.Tests;

public class UpdateDialogTests
{
    [Fact]
    public void UpdateDialog_CanBeCreated()
    {
        // Arrange & Act
        var exception = Record.Exception(() =>
        {
            using var dialog = new UpdateDialog("1.0.0", "Test changelog");
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void UpdateDialog_HandlesNullChangelog()
    {
        // Arrange & Act
        var exception = Record.Exception(() =>
        {
            using var dialog = new UpdateDialog("1.0.0", null!);
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void UpdateDialog_HandlesEmptyChangelog()
    {
        // Arrange & Act
        var exception = Record.Exception(() =>
        {
            using var dialog = new UpdateDialog("1.0.0", "");
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void UpdateDialog_HandlesMarkdownContent()
    {
        // Arrange
        var markdownContent = @"# Release Notes

## What's New
- Feature 1
- Feature 2

### Bug Fixes
- Fixed bug A
- Fixed bug B

```csharp
var code = ""sample"";
```
";

        // Act
        var exception = Record.Exception(() =>
        {
            using var dialog = new UpdateDialog("1.0.0", markdownContent);
        });

        // Assert
        Assert.Null(exception);
    }
}
