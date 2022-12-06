namespace TranqService.Common.Data
{
    public interface IConfig
    {
        ulong DiscordWebhookId { get; }
        string DiscordWebhookSecret { get; }
        Dictionary<string, string> YoutubeMusicPlaylists { get; }
        string SqliteFilePath { get; }
        Dictionary<string, string> YoutubeVideoPlaylists { get; }
        string YoutubeApiKey { get; }
    }
}