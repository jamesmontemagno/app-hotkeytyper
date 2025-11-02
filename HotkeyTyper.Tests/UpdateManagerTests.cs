using HotkeyTyper;
using Xunit;

namespace HotkeyTyper.Tests;

public class UpdateManagerTests
{
    [Fact]
    public void UpdateManager_InitialState_IsNotAvailable()
    {
        // Arrange & Act
        // On first access, no update check has been performed
        var isAvailable = UpdateManager.IsUpdateAvailable;
        
        // Assert
        // Before CheckForUpdatesAsync is called, no update should be reported as available
        Assert.False(isAvailable);
    }

    [Fact]
    public void UpdateManager_LatestVersion_InitiallyNull()
    {
        // Arrange & Act
        // On first access, no version info should be available
        var version = UpdateManager.LatestVersion;
        
        // Assert
        // Before CheckForUpdatesAsync is called, version should be null or empty
        Assert.True(string.IsNullOrEmpty(version));
    }

    [Fact]
    public async Task UpdateManager_CheckForUpdatesAsync_DoesNotThrow()
    {
        // Arrange & Act
        var exception = await Record.ExceptionAsync(async () =>
        {
            await UpdateManager.CheckForUpdatesAsync();
        });
        
        // Assert
        // The method should handle errors gracefully and not throw
        Assert.Null(exception);
    }

    [Fact]
    public void UpdateManager_GetChangelog_DoesNotThrow()
    {
        // Arrange & Act
        var exception = Record.Exception(() =>
        {
            UpdateManager.GetChangelog(3);
        });
        
        // Assert
        // The method should handle errors gracefully and not throw
        Assert.Null(exception);
    }
}
