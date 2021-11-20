
namespace TranqService.Shared.DataAccess
{
    public interface IYoutubeQueries
    {
        Task<IEnumerable<string>> GetDownloadedVideoIdsInPlaylistAsync(string playlistGuid);
        Task<int> GetPlaylistIdAsync(string playlistGuid);
        Task MarkVideoAsDownloadedAsync(string videoGuid, string playlistGuid, string? nodeId);
    }
}