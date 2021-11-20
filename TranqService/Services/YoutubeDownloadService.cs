using TranqServices.Shared.ApiHandlers;

namespace TranqService.Services;
public class YoutubeDownloadService : BackgroundService
{
    private readonly ILogger<YoutubeDownloadService> _logger;
    private readonly IYoutubeApiHandler _youtubeApiHandler;

    public YoutubeDownloadService(
        ILogger<YoutubeDownloadService> logger,
        IYoutubeApiHandler youtubeApiHandler
        )
    {
        _logger = logger;
        _youtubeApiHandler = youtubeApiHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("YoutubeDownloadService: Checking for youtube downloads at: {time}", DateTimeOffset.Now);

            var AAAAAAAA = await _youtubeApiHandler.GetAllPlaylistItemsAsync();

            // Check every hour
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}