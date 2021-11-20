
namespace TranqService.Shared.Logic
{
    public interface IYoutubeSaveHelper
    {
        Task<List<YoutubeVideoModel>> GetUndownloadedVideosAsync(string playlistId);
    }
}