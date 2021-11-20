namespace TranqService.Services
{
    public class YoutubeDownloadService : BackgroundService
    {
        private readonly ILogger<YoutubeDownloadService> _logger;

        public YoutubeDownloadService(
            ILogger<YoutubeDownloadService> logger

            )
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("YoutubeDownloadService: Checking for youtube downloads at: {time}", DateTimeOffset.Now);



                // Check every hour
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}