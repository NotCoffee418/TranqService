
namespace TranqService.Shared.Data
{
    public interface IConfig
    {
        Dictionary<string, string> VideoPlaylists { get; }
        Dictionary<string, string> MusicPlaylists { get; }
        string YoutubeApiKey { get; }
    }
}