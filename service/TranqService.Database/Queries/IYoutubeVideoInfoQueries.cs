namespace TranqService.Database.Queries
{
    public interface IYoutubeVideoInfoQueries
    {
        Task<int> CountDownloadedVideosAsync();
        Task<int> CountVideosInPlaylist(string playlistId);
        Task<List<string>> GetDownloadedVideoGuidsInPlaylistAsync(string playlistGuid);
        Task MarkVideosAsDownloadedAsync(params YoutubeVideoInfo[] videoInfos);
    }
}