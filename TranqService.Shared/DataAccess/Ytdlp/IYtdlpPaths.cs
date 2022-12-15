namespace TranqService.Shared.DataAccess.Ytdlp
{
    public interface IYtdlpPaths
    {
        string GetFfmpegExePath();
        string GetFfprobeExePath();
        string GetYtdlpExePath();
        string GetYtdlpVersionFilePath();
    }
}