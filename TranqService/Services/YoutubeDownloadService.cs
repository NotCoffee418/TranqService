namespace TranqService.Services;
public class YoutubeDownloadService : BackgroundService
{
    private readonly ILogger<YoutubeDownloadService> _logger;
    private readonly IYoutubeSaveHelper _youtubeSaveHelper;

    public YoutubeDownloadService(
        ILogger<YoutubeDownloadService> logger,
        IYoutubeSaveHelper youtubeSaveHelper
        )
    {
        _logger = logger;
        _youtubeSaveHelper = youtubeSaveHelper;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("YoutubeDownloadService: Checking for youtube downloads at: {time}", DateTimeOffset.Now);

            var AAAAAAAA = await _youtubeSaveHelper.GetUndownloadedVideosAsync("PLosVD5wwGC2vJPYxoZlQKpvXMOTBKfzLY");

            // Check every hour
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}