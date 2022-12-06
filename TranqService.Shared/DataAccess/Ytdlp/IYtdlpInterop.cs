namespace TranqService.Shared.DataAccess.Ytdlp
{
    public interface IYtdlpInterop
    {
        Task<bool> DownloadAudioAsync(string videoUrl, string savePath);
        Task<bool> DownloadVideoAsync(string videoUrl, string savePath);
        Task<bool> ValidateFfmpegInstallationAsync();
    }
}