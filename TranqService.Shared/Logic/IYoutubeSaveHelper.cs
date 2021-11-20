
namespace TranqService.Shared.Logic
{
    public interface IYoutubeSaveHelper
    {
        Task<List<YoutubeVideoModel>> GetUndownloadedVideosAsync(string playlistId);
        Task DownloadVideoAsync(YoutubeVideoModel videoData, string outputDir, string outputFormat);
    }
}