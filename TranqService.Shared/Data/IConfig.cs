
namespace TranqService.Shared.Data
{
    public interface IConfig
    {
        Dictionary<string, string> VideoPlaylists { get; }
        string YoutubeApiKey { get; }
    }
}