using Microsoft.Extensions.Configuration;

namespace TranqService.Shared.Data;
public class Config : IConfig
{
    private IConfiguration _appSettings;

    public Config(IConfiguration appSettings)
    {
        _appSettings = appSettings;
    }


    public string YoutubeApiKey
    {
        get => Get<string>("YoutubeApiKey");
    }


    private T Get<T>(string name)
    {
        // try to get value from environment variable 
        string? value = Environment.GetEnvironmentVariable(name);

        // Try to get value from appsettings
        if (value == null)
            value = _appSettings.GetSection($"Config:{name}").Value;

        if (value == null)
            throw new Exception($"Environment variable or app setting for {name} is not defined.");

        // Convert to desired type and return
        return (T)Convert.ChangeType(value, typeof(T));
    }
}