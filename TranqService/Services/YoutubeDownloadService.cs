namespace TranqService.Services;
public class YoutubeDownloadService : BackgroundService
{
    private readonly Serilog.ILogger _logger;
    private readonly IYoutubeSaveHelper _youtubeSaveHelper;
    private readonly IYoutubeVideoInfoQueries _youtubeQueries;
    private readonly IConfig _config;

    public YoutubeDownloadService(
        Serilog.ILogger logger,
        IYoutubeSaveHelper youtubeSaveHelper,
        IYoutubeVideoInfoQueries youtubeQueries,
        IConfig config)
    {
        _logger = logger;
        _youtubeSaveHelper = youtubeSaveHelper;
        _youtubeQueries = youtubeQueries;
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
        // Converts paths with variables to actual paths and creates missing paths
        Func<string, string> preparedOutputDir = (string inputPath) =>
        {
            // Replace variables
            string outputPath = inputPath
                .Replace("{Year}", DateTime.UtcNow.Year.ToString());

            // Ensure trailing slash depending on OS
            if (!outputPath.EndsWith(Path.DirectorySeparatorChar))
                outputPath = outputPath + Path.DirectorySeparatorChar;

            // Create missing directories
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            return outputPath;
        };

        // Video playlists
        foreach (var kvp in _config.VideoPlaylists)
        {
            string playlistGuid = kvp.Key;
            string outputDir = kvp.Value;

            // Populate queue with tasks
            List<YoutubeVideoInfo> videoToDownload =
                await _youtubeSaveHelper.GetUndownloadedVideosAsync(playlistGuid);

            await ProcessPlaylistAsync(
                videoToDownload,
                preparedOutputDir(outputDir),
                "mp4",
                stoppingToken);
        }

        // Music playlists
        foreach (var kvp in _config.MusicPlaylists)
        {
            string playlistGuid = kvp.Key;
            string outputDir = kvp.Value;

            // Populate queue with tasks
            List<YoutubeVideoInfo> videosToDownload =
                await _youtubeSaveHelper.GetUndownloadedVideosAsync(playlistGuid);

            // Run task
            await ProcessPlaylistAsync(
                videosToDownload,
                preparedOutputDir(outputDir),
                "mp3",
                stoppingToken);
        }
    }

    private async Task ProcessPlaylistAsync(
        List<YoutubeVideoInfo> allVideosToDownload,
        string outputDirectory,
        string outputFormat,
        CancellationToken stoppingToken)
    {
        // If first time running after DB reset, check any files downloaded
        // This doesn't work with variables but it's not worth the time since we're gonna safely persist DB anyway
        if (await _youtubeQueries.CountDownloadedVideosAsync() < 10)
        {
            // Get all known filenames in this directory
            var knownFiles = Directory.GetFiles(outputDirectory)
                .ToList();

            // Filter out files we already have
            var alreadyDownloadedFiles = allVideosToDownload
                .Where(x => knownFiles.Contains(x.GetFileName(outputFormat)))
                .ToList();

            // mark them as complete
            _logger.Warning(
                "Found {0} files which were already downloaded. Marking them as complete in the database.", 
                alreadyDownloadedFiles.Count());
            alreadyDownloadedFiles.ForEach(async x => 
                await _youtubeQueries.MarkVideoAsDownloadedAsync(x));

            // Continue downloading the files we dont have yet
            var alreadyDownloadedVideoGuids = alreadyDownloadedFiles.Select(x => x.VideoGuid);
            allVideosToDownload = allVideosToDownload
                .Where(x => !alreadyDownloadedVideoGuids.Contains(x.VideoGuid))
                .ToList();
        }

        // Prepare queue
        Queue<YoutubeVideoInfo> queue = new(allVideosToDownload);
        List<string> tempFiles = new List<string>();
        try
        {
            // Run downloaders 5 at a time
            List<Task> runningDownloaders = new List<Task>();
            List<Task> runningUploaders = new List<Task>();
            while (!stoppingToken.IsCancellationRequested && queue.Count > 0)
            {
                // Hold while max allowed parallel tasks is running
                while (runningDownloaders.Where(x => !x.IsCompleted).Count() >= 2)
                    await Task.Delay(200);

                // Run a fresh task
                YoutubeVideoInfo videoData = queue.Dequeue();
                string finalPath = Path.Join(outputDirectory, videoData.GetFileName(outputFormat));
                Task dlTask = Task.Run(async () =>
                {
                    // Store download location for video
                    string tmpPath = Path.Join(Path.GetTempPath(), $"{videoData.VideoGuid}.{outputFormat}");
                    tempFiles.Add(tmpPath);
                
                    bool markAsComplete = true;
                    if (File.Exists(finalPath))
                        _logger.Warning("File already exists, marking as complete: {0}", $"{videoData.VideoGuid} {videoData.Name}");
                    else
                    {
                        // Prepare retry policy
                        var delay = Backoff.ConstantBackoff(TimeSpan.FromMilliseconds(20000), retryCount: 5);
                        var retryPolicy = Policy
                            .Handle<Exception>()
                            .WaitAndRetryAsync(delay);

                        // Download the video to temp path
                        PolicyResult downloadResult = await retryPolicy.ExecuteAndCaptureAsync(async () =>
                            await _youtubeSaveHelper.DownloadVideoAsync(videoData, tmpPath, outputFormat));
                        
                        // Move the file to it's final destination
                        if (downloadResult.Outcome == OutcomeType.Successful)
                            File.Move(tmpPath, finalPath);
                        else
                        {
                            // Log exception
                            markAsComplete = false;
                            _logger.Error("YoutubeDownloaderService failed to download video: {0} {1} {2}",
                                videoData.VideoGuid, videoData.Name, downloadResult.FinalException.Message);
                        }
                    }

                    // Mark as downloaded in database
                    if (markAsComplete)
                        await _youtubeQueries.MarkVideoAsDownloadedAsync(videoData);
                });
                runningDownloaders.Add(dlTask);
            }

            // Wait for all tasks to complete
            if (!stoppingToken.IsCancellationRequested)
                await Task.WhenAll(runningDownloaders);
            if (!stoppingToken.IsCancellationRequested)
                await Task.WhenAll(runningUploaders);
        }
        catch (Exception ex)
        {
            _logger.Fatal("YoutubeDownloaderService failed on ProcessPlaylistAsync: {0}", ex);
            throw;
        }
        finally
        {
            foreach (string path in tempFiles)
                if (File.Exists(path))
                    File.Delete(path);
        }
    }
}