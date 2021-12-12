using CG.Web.MegaApiClient;
using Polly;
using Polly.Contrib.WaitAndRetry;
using TranqService.Shared.DataAccess;

namespace TranqService.Services;
public class YoutubeDownloadService : BackgroundService
{
    private readonly Serilog.ILogger _logger;
    private readonly IYoutubeSaveHelper _youtubeSaveHelper;
    private readonly IYoutubeQueries _youtubeQueries;
    private readonly IConfig _config;
    private readonly IMegaApiHandler _megaApiHandler;

    public YoutubeDownloadService(
        Serilog.ILogger logger,
        IYoutubeSaveHelper youtubeSaveHelper,
        IYoutubeQueries youtubeQueries,
        IMegaApiHandler megaApiHandler,
        IConfig config)
    {
        _logger = logger;
        _youtubeSaveHelper = youtubeSaveHelper;
        _youtubeQueries = youtubeQueries;
        _config = config;
        _megaApiHandler = megaApiHandler;
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
        // Parallel search for approperiate node directory
        // This is a slow operation
        INode? targetDirNode = null;
        Task<INode>? backgroundMegaPrep = null;

        if (allVideosToDownload.Count > 0)
            backgroundMegaPrep = PrepareMegaDirectory(outputDirectory, allVideosToDownload, outputFormat);

        // If first time running after DB reset, wait for mega so we can avoid downloading a whole playlist when we already have it in mega
        if (await _youtubeQueries.CountDownloadedVideosAsync() < 10)
        {
            // Wait for mega
            _logger.Warning("YoutubeDownloadService: This is a fresh database. Waiting for mega to be explored before downloading.");
            targetDirNode = await backgroundMegaPrep;

            // Get all known filenames in this directory
            var knownFiles = (await _megaApiHandler.ListFileNodes(targetDirNode))
                .Select(x => x.Name)
                .ToList();

            // Filter out files we already have
            var alreadyDownloadedFiles = allVideosToDownload
                .Where(x => knownFiles.Contains(x.GetFileName(outputFormat)))
                .ToList();

            // mark them as complete
            _logger.Warning(
                "Found {0} files which were already downloaded. Marking them as complete in the database.", 
                alreadyDownloadedFiles.Count());
            var markerTasks = new List<Task>();
            alreadyDownloadedFiles.ForEach(x => markerTasks.Add(
                _youtubeQueries.MarkVideoAsDownloadedAsync(x.VideoGuid, x.PlaylistGuid, null)));
            await Task.WhenAll(markerTasks);

            // Continue downloading the files we dont have yet
            var alreadyDownloadedVideoGuids = alreadyDownloadedFiles.Select(x => x.VideoGuid);
            allVideosToDownload = allVideosToDownload
                .Where(x => !alreadyDownloadedVideoGuids.Contains(x.VideoGuid))
                .ToList();
        }    

        // Prepare queue
        Queue<YoutubeVideoModel> queue = new();
        foreach (YoutubeVideoModel videoData in allVideosToDownload)
            queue.Enqueue(videoData);

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

                // Store download location for video
                YoutubeVideoModel videoData = queue.Dequeue();
                string tmpPath = Path.Join(Path.GetTempPath(), videoData.GetFileName(outputFormat));
                tempFiles.Add(tmpPath);

                // Run a fresh task
                Task dlTask = Task.Run(async () =>
                {
                    // Prepare retry policy
                    var delay = Backoff.ConstantBackoff(TimeSpan.FromMilliseconds(20000), retryCount: 5);
                    var retryPolicy = Policy
                        .Handle<Exception>()
                        .WaitAndRetryAsync(delay);

                    // Download the video
                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        await _youtubeSaveHelper.DownloadVideoAsync(
                            videoData, tmpPath, outputFormat);
                    });
                    

                    // Await mega
                    if (targetDirNode == null)
                        targetDirNode = await backgroundMegaPrep;

                    // Start uploader task
                    runningUploaders.Add(Task.Run(async () =>
                    {
                        // Wait for mega to find directory node
                        if (backgroundMegaPrep != null && targetDirNode == null)
                            targetDirNode = await backgroundMegaPrep;

                        bool markAsComplete = true;
                        INode uploadedNode = null;

                        // Duplicate filename
                        if (videoData.IsDuplicate)
                            _logger.Warning("YoutubeDownloadService: Not downloading which already has a file with the same name. Marking as complete instead");
                        // Failed to download for some reason
                        else if (!videoData.IsDownloaded)
                            markAsComplete = false; // happens when download failed
                                                    // Upload file to mega
                        else
                            await retryPolicy.ExecuteAsync(async () => 
                                uploadedNode = await _megaApiHandler.UploadFile(tmpPath, targetDirNode));

                        // Mark as downloaded in database
                        if (markAsComplete)
                            await _youtubeQueries.MarkVideoAsDownloadedAsync(
                                videoData.VideoGuid,
                                videoData.PlaylistGuid,
                                uploadedNode == null ? null : uploadedNode.Id);
                    }));
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

    /// <summary>
    /// Returns the desired directory INode and marks any duplicate files
    /// </summary>
    /// <param name="outputDirectory"></param>
    /// <param name="targetFiles"></param>
    /// <returns></returns>
    private async Task<INode> PrepareMegaDirectory(string outputDirectory, List<YoutubeVideoModel> targetFiles, string desiredExt)
    {
        // Preload all nodes to avoid running it multiple times.
        // Slow operation
        List<INode> allNodes = await _megaApiHandler.GetAllNodes();

        // Find the desired result node or creates it
        INode resultNode = await _megaApiHandler.FindDirectoryNode(outputDirectory, allNodes);

        // Get all files in target directory
        List<string> existingFileNames = (await _megaApiHandler.ListFileNodes(resultNode))
            .Select(x => x.Name)
            .ToList();

        // Mark any duplicate files
        foreach(var video in targetFiles)
        {
            string desiredFilename = video.GetFileName(desiredExt);
            if (existingFileNames.Contains(desiredFilename))
                video.IsDuplicate = true;
        }

        // Return requested directory INode
        _logger.Information("PrepareMegaDirectory complete.");
        return resultNode;
    }
}