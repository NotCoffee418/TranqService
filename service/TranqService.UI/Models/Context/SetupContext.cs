namespace TranqService.UI.Models.Context;

public class SetupContext : NotificationObject
{
    public SetupContext()
    {
        // Load known values from config
        ApiKeys apiKeys = ApiKeys.Get();
        YoutubeApiKey = apiKeys.YoutubeApiKey;
        DiscordWebhookId = apiKeys.DiscordWebhookId;
        DiscordWebhookSecret = apiKeys.DiscordWebhookSecret;
    }

    public bool CanSave
    {
        get => Get<bool>(nameof(CanSave), overrideDefault: true);
        set => Set(nameof(CanSave), value);
    }
    
    public string YoutubeApiKey
    {
        get => Get<string>(nameof(YoutubeApiKey));
        set => Set(nameof(YoutubeApiKey), value);
    }
    public ulong DiscordWebhookId
    {
        get => Get<ulong>(nameof(DiscordWebhookId));
        set => Set(nameof(DiscordWebhookId), value);
    }
    public string DiscordWebhookSecret
    {
        get => Get<string>(nameof(DiscordWebhookSecret));
        set => Set(nameof(DiscordWebhookSecret), value);
    }

    public void Save()
    {
        CanSave = false;
        ApiKeys.GetAsync().ContinueWith(task =>
        {
            // Load file first to ensure integrity of any other properties
            ApiKeys apiKeys = task.Result;
            apiKeys.YoutubeApiKey = YoutubeApiKey;
            apiKeys.DiscordWebhookId = DiscordWebhookId;
            apiKeys.DiscordWebhookSecret = DiscordWebhookSecret;

            // Update the file with the updated properties
            apiKeys.SaveAsync().ContinueWith(task =>
            {
                CanSave = true;
            });
        });
    }
}
