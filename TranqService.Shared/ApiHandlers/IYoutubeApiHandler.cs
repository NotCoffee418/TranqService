
namespace TranqServices.Shared.ApiHandlers
{
    public interface IYoutubeApiHandler
    {
        Task<List<YoutubeVideoModel>> GetAllPlaylistItemsAsync(string playlistId);
    }
}