namespace TranqService.Shared.DataAccess.Ytdlp
{
    public interface IYtdlpInterop
    {
        Task<(bool Success, string? ErrorMessage)> DownloadAudioAsync(string videoUrl, string savePath);
        Task<(bool Success, string? ErrorMessage)> DownloadVideoAsync(string videoUrl, string savePath);
        Task<string> GetCommentMetadata(string filePath);
        Task<bool> ValidateFfmpegInstallationAsync();
    }
}