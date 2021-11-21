using CG.Web.MegaApiClient;
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
            _logger.Warning("YoutubeThis is a fresh database. Waiting for mega to be explored before downloading.");
            targetDirNode = await backgroundMegaPrep;

            // Get all known filenames in this directory
            var knownFiles = (await _megaApiHandler.ListFileNodes(targetDirNode))
                .Select(x => x.Name)
                .ToList();

            // Filter out files we already have
            allVideosToDownload = allVideosToDownload
                .Where(x => !knownFiles.Contains(x.GetFileName(outputFormat)))
                .ToList();
        }
        

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
                string tmpPath = Path.Join(Path.GetTempPath(), videoData.GetFileName(outputFormat));
                downloadedList.Add((videoData, tmpPath));

                // Run a fresh task
                Task task = Task.Run(async () => await _youtubeSaveHelper.DownloadVideoAsync(
                    videoData, tmpPath, outputFormat));
                runningTasks.Add(task);
            }

            // Wait for last 5 tasks to complete
            if (!stoppingToken.IsCancellationRequested)
                await Task.WhenAll(runningTasks);

            // Wait for mega to find directory node
            if (backgroundMegaPrep != null && targetDirNode == null)
                targetDirNode = await backgroundMegaPrep;

            // Save files to mega, async, no parallel!
            foreach (var vidData in downloadedList)
            {
                bool markAsComplete = true;
                INode uploadedNode = null;

                // Duplicate filename
                if (vidData.Item1.IsDuplicate)
                    _logger.Warning("YoutubeDownloadService: Not downloading which already has a file with the same name. Marking as complete instead");
                // Failed to download for some reason
                else if (!vidData.Item1.IsDownloaded)
                    markAsComplete = false; // happens when download failed
                // Upload file to mega
                else
                    uploadedNode = await _megaApiHandler.UploadFile(vidData.Item2, targetDirNode);

                // Mark as downloaded in database
                if (markAsComplete)
                    await _youtubeQueries.MarkVideoAsDownloadedAsync(
                        vidData.Item1.VideoGuid, 
                        vidData.Item1.PlaylistGuid, 
                        uploadedNode == null ? null : uploadedNode.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.Fatal("YoutubeDownloaderService failed on ProcessPlaylistAsync: {0}", ex);
            throw;
        }
        finally
        {
            foreach (string path in downloadedList.Select(x => x.Item2))
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