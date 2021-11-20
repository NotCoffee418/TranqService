
namespace TranqServices.Shared.ApiHandlers
{
    public interface IYoutubeApiHandler
    {
        Task<IEnumerable<YoutubeVideoModel>> GetAllPlaylistItemsAsync();
    }
}