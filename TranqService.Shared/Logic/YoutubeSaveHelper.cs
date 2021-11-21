using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace TranqService.Shared.Logic;
public class YoutubeSaveHelper : IYoutubeSaveHelper
{
    private IYoutubeApiHandler _youtubeApiHandler;
    private IYoutubeQueries _processedYoutubeVideoQueries;
    private ILogger _logger;

    YoutubeClient youtube = new YoutubeClient();

    public YoutubeSaveHelper(
        IYoutubeApiHandler youtubeApiHandler,
        IYoutubeQueries processedYoutubeVideoQueries,
        ILogger logger)
    {
        _youtubeApiHandler = youtubeApiHandler;
        _processedYoutubeVideoQueries = processedYoutubeVideoQueries;
        _logger = logger;
    }

    public async Task<List<YoutubeVideoModel>> GetUndownloadedVideosAsync(string playlistId)
    {
        // Get all video IDs
        var allVideosInPlaylist = await _youtubeApiHandler.GetAllPlaylistItemsAsync(playlistId);

        // Get all previously downloaded videos
        var allPreviouslyDownloadedVideos = (await _processedYoutubeVideoQueries
            .GetDownloadedVideoIdsInPlaylistAsync(playlistId))
            .ToList();

        // Filter out undownloaded
        return allVideosInPlaylist
            .Where(x => !allPreviouslyDownloadedVideos.Contains(x.VideoGuid))
            .ToList();
    }

    /// <summary>
    /// Download a video from youtube to a temp path
    /// </summary>
    /// <param name="videoData"></param>
    /// <param name="outputDir"></param>
    /// <param name="outputFormat"></param>
    /// <returns>success?</returns>
    public async Task DownloadVideoAsync(YoutubeVideoModel videoData, string outputPath, string outputFormat)
    {
        try
        {
            // Log
            _logger.Information(
                "YoutubeDownloaderService: Downloading {0} {1}", 
                videoData.VideoGuid, videoData.Name);

            // Get stream data
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoData.VideoGuid);

            // Select approperiate stream type
            IStreamInfo streamInfo = outputFormat switch
            {
                "mp4" => streamManifest.GetMuxedStreams().GetWithHighestVideoQuality(),
                "mp3" => streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate(),
                _ => throw new NotImplementedException($"Format {outputFormat} not supported")
            };

            // Get the actual stream
            var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

            // Download the stream to a file
            await youtube.Videos.Streams.DownloadAsync(streamInfo, outputPath);
            _logger.Information(
                "YoutubeDownloaderService: Finished downloading {0} {1}",
                videoData.VideoGuid, videoData.Name);
            videoData.IsDownloaded = true;
        }
        catch (Exception ex)
        {
            // Log exception
            _logger.Warning("YoutubeDownloaderService failed to download video: {0} {1} {2}", 
                videoData.VideoGuid, videoData.Name, ex);

            // Cleanup on fail
            if (File.Exists(outputPath))
                File.Delete(outputPath);

            videoData.IsDownloaded = false;
        }
    }
}