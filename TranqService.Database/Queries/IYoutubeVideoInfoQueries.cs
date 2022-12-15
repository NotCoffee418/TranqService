namespace TranqService.Database.Queries
{
    public interface IYoutubeVideoInfoQueries
    {
        Task<int> CountDownloadedVideosAsync();
        Task<List<string>> GetDownloadedVideoGuidsInPlaylistAsync(string playlistGuid);
        Task MarkVideosAsDownloadedAsync(params YoutubeVideoInfo[] videoInfos);
    }
}