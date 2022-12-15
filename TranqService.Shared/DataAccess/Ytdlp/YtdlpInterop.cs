using System.Text.RegularExpressions;

namespace TranqService.Shared.DataAccess.Ytdlp;

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

    const string VideoFormatData = "-f \"bestvideo[ext=mp4]+bestaudio[ext=m4a]/best[ext=mp4]/best\" --prefer-ffmpeg --add-metadata --embed-thumbnail --metadata-from-title \"%(artist)s - %(title)s\" --no-warnings";
    const string AudioFormatData = "-x --audio-format mp3 --prefer-ffmpeg --add-metadata --embed-thumbnail --metadata-from-title \"%(artist)s - %(title)s\" --no-warnings";


    public Task<(bool Success, string? ErrorMessage)> DownloadVideoAsync(string videoUrl, string savePath)
        => RunYtdlpCommandAsync(videoUrl, savePath, VideoFormatData);

    public Task<(bool Success, string? ErrorMessage)> DownloadAudioAsync(string videoUrl, string savePath)
        => RunYtdlpCommandAsync(videoUrl, savePath, AudioFormatData);

    public async Task<bool> ValidateFfmpegInstallationAsync()
    {
        (bool success, string outputStr) = await RunFfmpegCommand("-version");
        return success && outputStr.StartsWith("ffmpeg version ");
    }

    public async Task<string> GetCommentMetadata(string filePath)
    {
        if (!await ValidateFfmpegInstallationAsync())
            throw new Exception("Error: ffmpeg not installed");

        try
        {
            (bool success, string outputStr) = await RunFfprobeCommand($"-v quiet -of flat=s=_ -show_entries format_tags=comment \"{filePath}\"");
            if (success)
            {
                // Example output: format_tags_comment="https://www.youtube.com/watch?v=videoidhere" (newline)
                if (string.IsNullOrEmpty(outputStr) || !outputStr.Contains("format_tags_comment"))
                    return null;
                
                // Clear trailing whitespace and extract full url
                outputStr = outputStr.TrimEnd('\r', '\n');
                Regex rUrlFromTag = new Regex("format_tags_comment=\"(.+)\"(.+)?");
                if (!rUrlFromTag.IsMatch(outputStr))
                    return null;
                return rUrlFromTag.Match(outputStr).Groups[1].Value;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting copyright metadata. Continuing the application.");
        }

        return string.Empty;
    }

    private async Task<(bool Success, string? ErrorMessage)> RunYtdlpCommandAsync(string inputUrl, string outputFile, string formatData)
    {
        // Get relevant paths
        string ytdlpExe = _ytdlpPaths.GetYtdlpExePath();
        string workingDir = Path.GetDirectoryName(ytdlpExe);

        // Set working directory (required for ffmpeg)
        // Also helps with linux support
        string args = $"{formatData} -o \"{outputFile}\" {inputUrl}";
        try
        {
            string outputStr = await RunCommandAsync(workingDir, ytdlpExe, args);

            // Validate
            if (!File.Exists(outputFile))
                throw new Exception("RunYtdlpCommandAsync(): Download indicates completion but output file does not exist");

            // ..
            // Additional validation should go here

        }
        catch (Exception ex)
        {

            // Cleanup on fail
            if (File.Exists(outputFile))
                File.Delete(outputFile);
            _logger.Error(ex.Message, ex);

            // Attempt to extract a clean error
            string errorMessage = ex.Message;
            if (ex.Message.Contains("ERROR:"))
            {
                errorMessage = ex.Message.Split(Environment.NewLine)
                    .Where(x => x.Contains("ERROR:"))
                    .First().Trim();
            }
            return (false, errorMessage);
        }
        return (true, null);
    }

    private async Task<(bool Success, string ErrorMessage)> RunFfmpegCommand(string args)
    {
        // Get relevant paths
        string ffmpegExe = _ytdlpPaths.GetFfmpegExePath();
        string workingDir = Path.GetDirectoryName(ffmpegExe);

        // Call
        string outputStr = null;
        try
        {
            outputStr = await RunCommandAsync(workingDir, ffmpegExe, args);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message, ex);
            return (false, ex.Message);
        }
        return (true, outputStr);
    }

    private async Task<(bool Success, string ErrorMessage)> RunFfprobeCommand(string args)
    {
        // Get relevant paths
        string ffmpegExe = _ytdlpPaths.GetFfprobeExePath();
        string workingDir = Path.GetDirectoryName(ffmpegExe);

        // Call
        string outputStr = null;
        try
        {
            outputStr = await RunCommandAsync(workingDir, ffmpegExe, args);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message, ex);
            return (false, ex.Message);
        }
        return (true, outputStr);
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
