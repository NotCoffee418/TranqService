using System.IO.Compression;

namespace TranqService.Ytdlp.Logic;

public class YtdlpUpdater : IYtdlpUpdater
{
    private IYtdlpPaths _ytdlpPaths;
    private IPathHelper _pathHelper;
    private IGithubAccess _githubAccess;
    private ILogger _logger;
    private IYtdlpInterop _ytdlpInterop;

    public YtdlpUpdater(
        IYtdlpPaths ytdlpPaths,
        IPathHelper pathHelper,
        IGithubAccess githubAccess,
        ILogger logger,
        IYtdlpInterop ytdlpInterop)
    {
        _ytdlpPaths = ytdlpPaths;
        _pathHelper = pathHelper;
        _githubAccess = githubAccess;
        _logger = logger;
        _ytdlpInterop = ytdlpInterop;
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

        string versionFilePath = _ytdlpPaths.GetYtdlpVersionFilePath();
        // Update required components
        try
        {
            await Task.WhenAll(
                InstallYtdlpAsync(),
                InstallFfmpegAsync()
                );
        }
        catch
        {
            // Corrupt the version file if it exists on failure
            if (File.Exists(versionFilePath))
                File.Delete(versionFilePath);
            throw;
        }

        // Update the installed version
        // Should be done here to ensure all components installed correctly
        await File.WriteAllTextAsync(versionFilePath, (await remoteVersion).ToString());

        UpdateSemaphore.Release();
    }

    /// <summary>
    /// ffmpeg should be installed to the same directory as yt-dlp on windows
    /// WARN: Linux should not call this, but instead throw an error requesting manual installation of ffmpeg
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private async Task InstallFfmpegAsync()
    {
        string sourceUrl = "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip";
        string zipSavePath = _pathHelper.GetAppdataPath(true, "ffmpeg-build.zip");
        string binOutputDir = Path.GetDirectoryName(_ytdlpPaths.GetYtdlpExePath());

        try
        {
            // Download latest ffmpeg build
            await DownloadFileAsync(sourceUrl, zipSavePath);

            // Extract binaries only
            using var zip = ZipFile.OpenRead(zipSavePath);
            var toExtract = zip.Entries.Where(x => x.FullName.EndsWith(".exe"));
            foreach (var entry in toExtract)
            {
                string entryOutputPath = Path.Join(binOutputDir, Path.GetFileName(entry.FullName));
                if (File.Exists(entryOutputPath))
                    File.Delete(entryOutputPath);
                entry.ExtractToFile(entryOutputPath);
            }
        }
        catch { throw; }
        finally
        {
            if (File.Exists(zipSavePath))
                File.Delete(zipSavePath);
        }

        // Validate install
        if (!await _ytdlpInterop.ValidateFfmpegInstallationAsync())
        {
            string error = "InstallFfmpegAsync(): " + (OperatingSystem.IsWindows() ?
                "Something went wrong installing ffmpeg" :
                "Please install ffmpeg manually. The application will now crash.");
            _logger.Error(error);
            throw new Exception(error);
        }

    }

    private async Task InstallYtdlpAsync()
    {
        // Update needed, download the exe
        await DownloadFileAsync(
                "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe",
                _ytdlpPaths.GetYtdlpExePath());
    }

    private async Task DownloadFileAsync(string url, string savePath)
    {
        try
        {
            var httpResult = await AppConstants.HTTPCLIENT.GetAsync(url);
            using var resultStream = await httpResult.Content.ReadAsStreamAsync();
            using var fileStream = File.Create(savePath);
            await resultStream.CopyToAsync(fileStream);
        }
        catch (Exception ex)
        {
            // Corrupt the version file if it exists on failure
            _logger.Error($"YtdlpUpdater failed to download. {ex.Message}");
            throw;
        }
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
