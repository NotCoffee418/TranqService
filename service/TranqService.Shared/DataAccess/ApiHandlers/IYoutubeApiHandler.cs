namespace TranqService.Shared.DataAccess.ApiHandlers
{
    public interface IYoutubeApiHandler
    {
        string Escape(string input);
        Task<List<YoutubeVideoInfo>> GetAllPlaylistItemsAsync(string playlistId);
        Task<(bool IsValid, string Name, string? ErrorMessage)> GetPlaylistInfoAsync(string playlistId);
    }
}