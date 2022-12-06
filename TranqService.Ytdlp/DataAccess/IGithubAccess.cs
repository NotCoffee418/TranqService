namespace TranqService.Ytdlp.DataAccess
{
    public interface IGithubAccess
    {
        Task<DateTime> GetLatestYtDlpVersionAsync();
    }
}