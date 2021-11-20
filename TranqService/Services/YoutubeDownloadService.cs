namespace TranqService.Services;
public class YoutubeDownloadService : BackgroundService
{
    private readonly ILogger<YoutubeDownloadService> _logger;
    private readonly IYoutubeSaveHelper _youtubeSaveHelper;
    private readonly IConfig _config;

    public YoutubeDownloadService(
        ILogger<YoutubeDownloadService> logger,
        IYoutubeSaveHelper youtubeSaveHelper,
        IConfig config
        )
    {
        _logger = logger;
        _youtubeSaveHelper = youtubeSaveHelper;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("YoutubeDownloadService: Checking for youtube downloads at: {time}", DateTimeOffset.Now);

            var ree = _config.VideoPlaylists;

            // List all videos that need to be downloaded
            List<YoutubeVideoModel> videoToDownload =
                await _youtubeSaveHelper.GetUndownloadedVideosAsync("PLosVD5wwGC2vJPYxoZlQKpvXMOTBKfzLY");

            //await ProcessPlaylistAsync(videoToDownload);

            // Check every hour
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task ProcessPlaylistAsync(
        List<YoutubeVideoModel> videoToDownload,
        string outputDirectory,
        string outputFormat,
        CancellationToken stoppingToken)
    {

    }
}