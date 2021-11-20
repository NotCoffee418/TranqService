

namespace TranqService.Shared.Data;
public class Config : IConfig
{
    private IConfiguration _appSettings;
    private ILogger _logger;

    public Config(
        IConfiguration appSettings,
        ILogger logger)
    {
        _appSettings = appSettings;
        _logger = logger;
    }


    public string YoutubeApiKey
    {
        get => Get<string>("YoutubeApiKey");
    }

    public string MegaUsername
    {
        get => Get<string>("MegaUsername");
    }

    public string MegaPassword
    {
        get => Get<string>("MegaPassword");
    }

    public Dictionary<string, string> VideoPlaylists
    {
        get
        {
            return GetDictionary<string, string>("VideoPlaylists"); ;
        }
    }

    public Dictionary<string, string> MusicPlaylists
    {
        get
        {
            return GetDictionary<string, string>("MusicPlaylists"); ;
        }
    }


    private T Get<T>(string name)
    {
        // try to get value from environment variable 
        string? value = Environment.GetEnvironmentVariable(name);

        // Try to get value from appsettings
        if (value == null)
            value = _appSettings.GetSection($"Config:{name}").Value;

        if (value == null)
        {
            string exMsg = $"Environment variable or app setting for {name} is not defined.";
            _logger.Fatal(exMsg);
            throw new Exception(exMsg);
        }

        // Convert to desired type and return
        return (T)Convert.ChangeType(value, typeof(T));
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
            _logger.Fatal(exMsg, ex);
            throw new Exception(exMsg, ex);
        }
    }
}