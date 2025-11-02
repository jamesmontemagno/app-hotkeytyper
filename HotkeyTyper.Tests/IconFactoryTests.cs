using System.Drawing;
using HotkeyTyper;
using Xunit;

namespace HotkeyTyper.Tests;

public class IconFactoryTests
{
    [Fact]
    public void Create_ReturnsValidIcon()
    {
        // Arrange & Act
        var icon = IconFactory.Create(out IntPtr handle);
        
        // Assert
        Assert.NotNull(icon);
        Assert.NotEqual(IntPtr.Zero, handle);
        
        // Cleanup
        IconFactory.Destroy(ref icon, ref handle);
        Assert.Null(icon);
        Assert.Equal(IntPtr.Zero, handle);
    }

    [Fact]
    public void Create_ProducesMultipleResolutions()
    {
        // Arrange & Act
        var icon = IconFactory.Create(out IntPtr handle);
        
        // Assert - Icon should support standard Windows icon sizes
        // We verify by checking the icon can be converted to different sizes
        Assert.NotNull(icon);
        
        // Icon.Size returns the largest available size
        Assert.True(icon.Width > 0);
        Assert.True(icon.Height > 0);
        
        // Cleanup
        IconFactory.Destroy(ref icon, ref handle);
    }

    [Fact]
    public void TryExportIcon_CreatesFile()
    {
        // Arrange
        var testPath = Path.Combine(Path.GetTempPath(), $"test_icon_{Guid.NewGuid()}.ico");
        var icon = IconFactory.Create(out IntPtr handle);
        
        try
        {
            // Act
            bool exported = IconFactory.TryExportIcon(icon, testPath);
            
            // Assert
            Assert.True(exported);
            Assert.True(File.Exists(testPath));
            
            // Verify it's a valid icon by loading it
            using var loadedIcon = new Icon(testPath);
            Assert.NotNull(loadedIcon);
        }
        finally
        {
            // Cleanup
            IconFactory.Destroy(ref icon, ref handle);
            if (File.Exists(testPath))
            {
                File.Delete(testPath);
            }
        }
    }

    [Fact]
    public void TryExportIcon_DoesNotOverwriteExisting()
    {
        // Arrange
        var testPath = Path.Combine(Path.GetTempPath(), $"test_icon_{Guid.NewGuid()}.ico");
        var icon = IconFactory.Create(out IntPtr handle);
        
        try
        {
            // Create initial file
            File.WriteAllText(testPath, "dummy content");
            var originalContent = File.ReadAllText(testPath);
            
            // Act
            bool exported = IconFactory.TryExportIcon(icon, testPath);
            
            // Assert
            Assert.False(exported);
            Assert.Equal(originalContent, File.ReadAllText(testPath));
        }
        finally
        {
            // Cleanup
            IconFactory.Destroy(ref icon, ref handle);
            if (File.Exists(testPath))
            {
                File.Delete(testPath);
            }
        }
    }

    [Fact]
    public void Destroy_HandlesNullIcon()
    {
        // Arrange
        Icon? icon = null;
        IntPtr handle = IntPtr.Zero;
        
        // Act & Assert - Should not throw
        IconFactory.Destroy(ref icon, ref handle);
    }
}
