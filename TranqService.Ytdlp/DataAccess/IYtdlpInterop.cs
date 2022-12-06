namespace TranqService.Ytdlp.DataAccess
{
    public interface IYtdlpInterop
    {
        Task<bool> DownloadAudio(string videoUrl, string savePath);
        Task<bool> DownloadVideo(string videoUrl, string savePath);
        Task<bool> ValidateFfmpegInstallationAsync();
    }
}