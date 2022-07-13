namespace TranqService.Common.Data
{
    public interface IConfig
    {
        ulong DiscordWebhookId { get; }
        string DiscordWebhookSecret { get; }
        Dictionary<string, string> MusicPlaylists { get; }
        string SqliteFilePath { get; }
        Dictionary<string, string> VideoPlaylists { get; }
        string YoutubeApiKey { get; }
    }
}