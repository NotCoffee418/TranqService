
namespace TranqService.Shared.Data
{
    public interface IConfig
    {
        ulong DiscordWebhookId { get; }
        string DiscordWebhookSecret { get; }
        string MegaPassword { get; }
        string MegaUsername { get; }
        Dictionary<string, string> MusicPlaylists { get; }
        Dictionary<string, string> VideoPlaylists { get; }
        string YoutubeApiKey { get; }
    }
}