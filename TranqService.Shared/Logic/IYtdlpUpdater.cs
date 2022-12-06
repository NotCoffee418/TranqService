namespace TranqService.Shared.Logic
{
    public interface IYtdlpUpdater
    {
        Task<DateTime?> GetYtdlpLocalVersionTimeAsync();
        Task TryUpdateYtdlpAsync();
    }
}