namespace TranqService.Shared.Logic
{
    public interface IYoutubeSaveHelper
    {
        Task<List<YoutubeVideoInfo>> GetUndownloadedVideosAsync(string playlistId);
    }
}