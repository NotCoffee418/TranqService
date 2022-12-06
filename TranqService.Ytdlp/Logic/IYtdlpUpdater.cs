namespace TranqService.Ytdlp.Logic
{
    public interface IYtdlpUpdater
    {
        string GetYtdlpExePath();
        Task<DateTime?> GetYtdlpLocalVersionTimeAsync();
        string GetYtdlpVersionFilePath();
        Task TryUpdateYtdlpAsync();
    }
}