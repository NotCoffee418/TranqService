namespace TranqService.Shared.Logic;
public class YoutubeSaveHelper : IYoutubeSaveHelper
{
    private IYoutubeApiHandler _youtubeApiHandler;
    private IYoutubeQueries _processedYoutubeVideoQueries;

    public YoutubeSaveHelper(
        IYoutubeApiHandler youtubeApiHandler,
        IYoutubeQueries processedYoutubeVideoQueries)
    {
        _youtubeApiHandler = youtubeApiHandler;
        _processedYoutubeVideoQueries = processedYoutubeVideoQueries;
    }

    public async Task<List<YoutubeVideoModel>> GetUndownloadedVideosAsync(string playlistId)
    {
        // Get all playlist IDs
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
}