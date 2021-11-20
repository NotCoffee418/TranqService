namespace TranqServices.Shared.Data;
public class Config : IConfig
{
    public string YoutubeApiKey
    {
        get => Get<string>("YoutubeApiKey");
    }


    private T Get<T>(string name)
    {
        string? value = Environment.GetEnvironmentVariable(name);
        if (value == null)
            throw new Exception($"Environment variable {name} is not defined.");
        return (T)Convert.ChangeType(value, typeof(T));
    }
}