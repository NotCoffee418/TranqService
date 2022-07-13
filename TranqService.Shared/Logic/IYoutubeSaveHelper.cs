namespace TranqService.Shared.Logic
{
    public interface IYoutubeSaveHelper
    {
        Task DownloadVideoAsync(YoutubeVideoInfo videoData, string outputPath, string outputFormat);
        Task<List<YoutubeVideoInfo>> GetUndownloadedVideosAsync(string playlistId);
    }
}