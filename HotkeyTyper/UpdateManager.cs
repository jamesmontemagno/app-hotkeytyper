using System.ComponentModel;
using System.Diagnostics;
using Updatum;

namespace HotkeyTyper;

/// <summary>
/// Manages application updates via GitHub Releases using the Updatum library.
/// </summary>
internal static class UpdateManager
{
    private static readonly UpdatumManager AppUpdater = new("jamesmontemagno", "app-hotkeytyper")
    {
        // Since we're .NET single-file app, Updatum will auto-detect .exe
        // No need for AssetRegexPattern as default works (win-x64)
        FetchOnlyLatestRelease = true, // Saves API tokens
    };

    /// <summary>
    /// Gets whether an update is available.
    /// </summary>
    public static bool IsUpdateAvailable => AppUpdater.IsUpdateAvailable;

    /// <summary>
    /// Gets the latest release version as a string.
    /// </summary>
    public static string? LatestVersion => AppUpdater.LatestReleaseTagVersionStr;

    /// <summary>
    /// Checks for available updates asynchronously.
    /// </summary>
    /// <returns>True if an update is available; otherwise, false.</returns>
    public static async Task<bool> CheckForUpdatesAsync()
    {
        try
        {
            return await AppUpdater.CheckForUpdatesAsync();
        }
        catch (Exception ex)
        {
            // Log but don't crash
            Debug.WriteLine($"Update check failed: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Gets the changelog for recent releases.
    /// </summary>
    /// <param name="maxReleases">Maximum number of releases to include in changelog.</param>
    /// <returns>Changelog text or null if not available.</returns>
    public static string? GetChangelog(int maxReleases = 3)
    {
        return AppUpdater.GetChangelog(maxReleases);
    }

    /// <summary>
    /// Downloads and installs the latest update.
    /// </summary>
    /// <param name="progress">Optional progress reporter for download percentage.</param>
    /// <returns>True if successful; otherwise, false. Note: app will restart on success.</returns>
    public static async Task<bool> DownloadAndInstallUpdateAsync(IProgress<double>? progress = null)
    {
        try
        {
            PropertyChangedEventHandler? handler = null;
            if (progress != null)
            {
                handler = (s, e) =>
                {
                    if (e.PropertyName == nameof(UpdatumManager.DownloadedPercentage))
                    {
                        progress.Report(AppUpdater.DownloadedPercentage);
                    }
                };
                AppUpdater.PropertyChanged += handler;
            }

            try
            {
                var download = await AppUpdater.DownloadUpdateAsync();
                if (download == null) return false;

                // Save settings before upgrade
                // (Already handled by existing code)

                await AppUpdater.InstallUpdateAsync(download);
                return true; // Won't actually return - app will restart
            }
            finally
            {
                // Unregister handler to prevent memory leaks
                if (handler != null)
                {
                    AppUpdater.PropertyChanged -= handler;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Update failed: {ex.Message}");
            return false;
        }
    }
}
