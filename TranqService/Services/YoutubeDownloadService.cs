namespace TranqService.Services;
public class YoutubeDownloadService : BackgroundService
{
    private readonly Serilog.ILogger _logger;
    private readonly IYoutubeSaveHelper _youtubeSaveHelper;
    private readonly IConfig _config;

    public YoutubeDownloadService(
        Serilog.ILogger logger,
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
            _logger.Information("YoutubeDownloadService: Checking for youtube downloads at: {0}", DateTimeOffset.Now);


            // Process all playlists
            await ProcessAllPlaylists(stoppingToken);
            _logger.Information("YoutubeDownloaderService: Download session complete.");


            // Check every hour
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task ProcessAllPlaylists(CancellationToken stoppingToken)
    {
        // Prepare all tasks

            // Video playlists
            foreach (var kvp in _config.VideoPlaylists)
            {
                string playlistGuid = kvp.Key;
                string outputDir = kvp.Value;

                // Populate queue with tasks
                List<YoutubeVideoModel> videoToDownload =
                    await _youtubeSaveHelper.GetUndownloadedVideosAsync(playlistGuid);

                await ProcessPlaylistAsync(
                    videoToDownload,
                    outputDir,
                    "mp4",
                    stoppingToken);
            }

            // Music playlists
            foreach (var kvp in _config.MusicPlaylists)
            {
                string playlistGuid = kvp.Key;
                string outputDir = kvp.Value;

                // Populate queue with tasks
                List<YoutubeVideoModel> videosToDownload =
                    await _youtubeSaveHelper.GetUndownloadedVideosAsync(playlistGuid);

                // Run task
                await ProcessPlaylistAsync(
                    videosToDownload,
                    outputDir,
                    "mp3",
                    stoppingToken);
            }

            
    }

    private async Task ProcessPlaylistAsync(
        List<YoutubeVideoModel> allVideosToDownload,
        string outputDirectory,
        string outputFormat,
        CancellationToken stoppingToken)
    {
        // Prepare queue
        Queue<YoutubeVideoModel> queue = new();
        foreach (YoutubeVideoModel videoData in allVideosToDownload) 
            queue.Enqueue(videoData);


        // List of downloaded temp files
        List<(YoutubeVideoModel, string)> downloadedList = new();

        try
        {
            // Run downloaders 5 at a time
            List<Task> runningTasks = new List<Task>();
            while (!stoppingToken.IsCancellationRequested && queue.Count > 0)
            {
                // Hold while max allowed parallel tasks is running
                while (runningTasks.Where(x => !x.IsCompleted).Count() >= 5)
                    await Task.Delay(200);

                // Store download location for video
                YoutubeVideoModel videoData = queue.Dequeue();
                string tmpPath = Path.GetTempFileName();
                downloadedList.Add((videoData, tmpPath));

                // Run a fresh task
                Task task = Task.Run(async () => await _youtubeSaveHelper.DownloadVideoAsync(
                    videoData, tmpPath, outputFormat));
                runningTasks.Add(task);
            }

            // Wait for last 5 tasks to complete
            if (!stoppingToken.IsCancellationRequested)
                await Task.WhenAll(runningTasks);
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            foreach (string path in downloadedList.Select(x => x.Item2))
                if (File.Exists(path))
                    File.Delete(path);
        }
        
    }
}