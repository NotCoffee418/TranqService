namespace TranqService.Common.Models.Configs;

[ConfigFile("ApiKeys.json")]
public class ApiKeys : ConfigBase<ApiKeys>
{
    public ulong DiscordWebhookId { get; set; }
    public string DiscordWebhookSecret { get; set; }
    public string YoutubeApiKey { get; set; }
}
