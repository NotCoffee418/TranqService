namespace TranqService.Ytdlp.Logic;

public class YtdlpUpdater : IYtdlpUpdater
{
    private IYtdlpPaths _ytdlpPaths;
    private IGithubAccess _githubAccess;
    private ILogger _logger;

    public YtdlpUpdater(
        IYtdlpPaths ytdlpPaths,
        IGithubAccess githubAccess,
        ILogger logger)
    {
        _ytdlpPaths = ytdlpPaths;
        _githubAccess = githubAccess;
        _logger = logger;
    }


    SemaphoreSlim UpdateSemaphore { get; } = new(1, 1);

    /// <summary>
    /// Validates ytdl installation and updates if needed.
    /// Threadsafe.
    /// </summary>
    /// <returns></returns>
    public async Task TryUpdateYtdlpAsync()
    {
        // Prevent multiple instances of this function running as it will break things
        await UpdateSemaphore.WaitAsync();

        // Get version info
        Task<DateTime?> localVersion = GetYtdlpLocalVersionTimeAsync();
        Task<DateTime> remoteVersion = _githubAccess.GetLatestYtDlpVersionAsync();

        // Do nothing if up-to-date
        if (await localVersion is not null && await remoteVersion <= await localVersion)
        {
            UpdateSemaphore.Release();
            return;
        }

        // Update needed, download the exe
        string exePath = _ytdlpPaths.GetYtdlpExePath();
        string versionFilePath = _ytdlpPaths.GetYtdlpVersionFilePath();
        try
        {
            var httpResult = await AppConstants.HTTPCLIENT.GetAsync(
            "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe");
            using var resultStream = await httpResult.Content.ReadAsStreamAsync();
            using var fileStream = File.Create(exePath);
            resultStream.CopyTo(fileStream);
        }
        catch (Exception ex)
        {
            // Corrupt the version file if it exists on failure
            if (File.Exists(versionFilePath))
                File.Delete(versionFilePath);
            _logger.Error("TryUpdateYtdlpAsync(): " + ex.Message);
            throw;
        }

        // Update the installed version
        await File.WriteAllTextAsync(versionFilePath, (await remoteVersion).ToString());
        UpdateSemaphore.Release();
    }

    /// <summary>
    /// Version is inferred from publish time, not actual version.
    /// Returns null when not installed (correctly).
    /// </summary>
    /// <returns></returns>
    public async Task<DateTime?> GetYtdlpLocalVersionTimeAsync()
    {
        // Ensure exe exists, otherwise assume something went wrong and reinstall
        if (!File.Exists(_ytdlpPaths.GetYtdlpExePath())) return null;

        // Ensure version file exists
        string versionFilePath = _ytdlpPaths.GetYtdlpVersionFilePath();
        if (!File.Exists(versionFilePath)) return null;

        // Attempt to extract version
        string versionString = await File.ReadAllTextAsync(versionFilePath);
        if (!DateTime.TryParse(versionString, out DateTime versionTime))
            return null;

        // Valid version time found, return it
        return versionTime;
    }
}
