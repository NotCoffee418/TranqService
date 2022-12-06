﻿namespace TranqService.Common.Data;
public class Config : IConfig
{
    private IConfiguration _appSettings;

    public Config(
        IConfiguration appSettings)
    {
        _appSettings = appSettings;
    }


    public string YoutubeApiKey
    {
        get => Get<string>("YoutubeApiKey");
    }
    public ulong DiscordWebhookId
    {
        get => Get<ulong>("DiscordWebhookId");
    }

    public string DiscordWebhookSecret
    {
        get => Get<string>("DiscordWebhookSecret");
    }

    public Dictionary<string, string> YoutubeVideoPlaylists
    {
        get
        {
            // Don't rename the appsettings.json version, compatability
            return GetDictionary<string, string>("VideoPlaylists"); ;
        }
    }

    public Dictionary<string, string> YoutubeMusicPlaylists
    {
        get
        {
            // Don't rename the appsettings.json version, compatability
            return GetDictionary<string, string>("MusicPlaylists"); ;
        }
    }

    /// <summary>
    /// Should be stored in mega drive ideally
    /// </summary>
    public string SqliteFilePath
    {
        get => Get<string>("SqliteFilePath");
    }


    private T Get<T>(string name)
    {
        // Extract config value
        string? value = _appSettings.GetSection($"Config:{name}").Value;
        if (value == null)
        {
            string exMsg = $"Environment variable or app setting for {name} is not defined.";
            throw new Exception(exMsg);
        }

        // Convert to desired type and return
        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch(Exception ex)
        {
            throw new Exception($"Failed to parse config value {name} as {typeof(T).Name}");
        }
    }

    private Dictionary<T, U> GetDictionary<T, U>(string name)
        where T : notnull
    {
        var section = _appSettings.GetSection($"Config:{name}");

        try
        {
            return section.GetChildren()
            .ToDictionary(
                x => (T)Convert.ChangeType(x.Key, typeof(T?)),
                x => (U)Convert.ChangeType(x.Value, typeof(U?)));
        }
        catch (Exception ex)
        {
            string exMsg = "Failed to find or parse data for {name} into dictionary.";
            throw new Exception(exMsg, ex);
        }
    }
}