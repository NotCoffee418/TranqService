namespace TranqService.Shared.Logic;
public class PlaylistHelper : IPlaylistHelper
{
    private IYoutubeApiHandler _youtubeApiHandler;
    private IYoutubeVideoInfoQueries _processedYoutubeVideoQueries;
    private ILogger _logger;

    public PlaylistHelper(
        IYoutubeApiHandler youtubeApiHandler,
        IYoutubeVideoInfoQueries processedYoutubeVideoQueries,
        ILogger logger)
    {
        _youtubeApiHandler = youtubeApiHandler;
        _processedYoutubeVideoQueries = processedYoutubeVideoQueries;
        _logger = logger;
    }

    public async Task<List<YoutubeVideoInfo>> GetUndownloadedVideosAsync(string playlistId)
    {
        // Get all video IDs
        var allVideosInPlaylist = await _youtubeApiHandler.GetAllPlaylistItemsAsync(playlistId);

        // Get all previously downloaded videos
        var allPreviouslyDownloadedVideos = (await _processedYoutubeVideoQueries
            .GetDownloadedVideoGuidsInPlaylistAsync(playlistId))
            .ToList();

        // Filter out undownloaded
        return allVideosInPlaylist
            .Where(x => !allPreviouslyDownloadedVideos.Contains(x.VideoGuid))
            .ToList();
    }
}