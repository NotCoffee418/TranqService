namespace TranqService.Shared.DataAccess.ApiHandlers
{
    public interface IGithubAccess
    {
        Task<DateTime> GetLatestYtDlpVersionAsync();
    }
}