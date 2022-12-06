namespace TranqService.Ytdlp.DataAccess;

public class YtdlpInterop : IYtdlpInterop
{
    private IYtdlpPaths _ytdlpPaths;
    private ILogger _logger;

    public YtdlpInterop(
        IYtdlpPaths ytdlpPaths,
        ILogger logger)
    {
        _ytdlpPaths = ytdlpPaths;
        _logger = logger;
    }

    const string VideoFormatData = "-f \"bestvideo[ext=mp4]+bestaudio[ext=m4a]/best[ext=mp4]/best\" --prefer-ffmpeg";
    const string AudioFormatData = "-x --audio-format mp3 --prefer-ffmpeg";


    public Task<bool> DownloadVideo(string videoUrl, string savePath)
        => RunYtdlpCommandAsync(videoUrl, savePath, VideoFormatData);

    public Task<bool> DownloadAudio(string videoUrl, string savePath)
        => RunYtdlpCommandAsync(videoUrl, savePath, AudioFormatData);

    public async Task<bool> ValidateFfmpegInstallationAsync()
    {
        string workingDir = Path.GetDirectoryName(_ytdlpPaths.GetYtdlpExePath());
        string ffmpegPath = Path.Join(workingDir, "ffmpeg");
        string versionOutput = await RunCommandAsync(workingDir, ffmpegPath, "-version");
        return versionOutput.StartsWith("ffmpeg version ");
    }

    private async Task<bool> RunYtdlpCommandAsync(string inputUrl, string outputFile, string formatData)
    {
        // Get relevant paths
        string ytdlpExe = _ytdlpPaths.GetYtdlpExePath();
        string workingDir = Path.GetDirectoryName(ytdlpExe);

        // Set working directory (required for ffmpeg)
        // Also helps with linux support
        string args = $"{formatData} -o \"{outputFile}\" {inputUrl}";
        string outputStr = await RunCommandAsync(workingDir, ytdlpExe, args);

        // Validate
        if (!File.Exists(outputFile))
            throw new Exception("RunYtdlpCommandAsync(): Download indicates completion but output file does not exist");

        // ...

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="workingDir"></param>
    /// <param name="exePath">Must be full path on windows</param>
    /// <param name="args"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task<string> RunCommandAsync(string workingDir, string exePath, string args)
    {
        var result = await Cli.Wrap(exePath)
            .WithWorkingDirectory(workingDir)
            .WithArguments(args)
            .ExecuteBufferedAsync();

        if (!string.IsNullOrEmpty(result.StandardError))
        {
            string err = $"{Path.GetFileName(exePath)}: {result.StandardError}";
            _logger.Error(err);
            throw new Exception(err);
        }
        else return result.StandardOutput;
    }
}
