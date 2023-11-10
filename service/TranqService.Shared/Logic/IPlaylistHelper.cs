namespace TranqService.Shared.Logic
{
    public interface IPlaylistHelper
    {
        Task<List<YoutubeVideoInfo>> GetUndownloadedVideosAsync(string playlistId);
    }
}