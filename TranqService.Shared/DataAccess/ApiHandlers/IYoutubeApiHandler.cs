namespace TranqService.Shared.DataAccess.ApiHandlers
{
    public interface IYoutubeApiHandler
    {
        string Escape(string input);
        Task<List<YoutubeVideoInfo>> GetAllPlaylistItemsAsync(string playlistId);
    }
}