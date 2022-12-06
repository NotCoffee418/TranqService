namespace TranqService.Ytdlp.Logic
{
    public interface IYtdlpUpdater
    {
        Task<DateTime?> GetYtdlpLocalVersionTimeAsync();
        Task TryUpdateYtdlpAsync();
    }
}