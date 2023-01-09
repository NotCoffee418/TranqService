namespace TranqService.Common.Models.Configs;
public abstract class ConfigBase<T>
    where T : class, new()
{
    private static T instance = null;

    /// <summary>
    /// Gets (and creates) the instance of the config (sync)
    /// </summary>
    /// <returns></returns>
    public static T Get() => GetAsync().Result;

    /// <summary>
    /// Gets (and creates) the instance of the config (async)
    /// </summary>
    /// <returns></returns>
    public static async Task<T> GetAsync(bool forceReload = false)
    {
        if (forceReload || instance is null)
        {
            instance = new T();
            await (instance as ConfigBase<T>).ReloadInstanceAsync();
        }
        return instance;
    }

    /// <summary>
    /// Load important paths from the json file or use valid defaults.
    /// </summary>
    /// <returns></returns>
    public async Task<T> ReloadInstanceAsync()
    {
        // Create config if doesn't exist yet (using defaults specified in the parent config class)
        string configPath = ConfigFileAttribute.GetConfigFilePath<T>();
        if (!File.Exists(configPath))
            await SaveAsync();

        // Reload existing config file
        try
        {
            // Causes deadlock on async call, so.. just don't.
            string json = File.ReadAllText(configPath);
            instance = JsonSerializer.Deserialize<T>(json) ?? new T();
        }
        catch (Exception ex)
        {
            throw new Exception("There was an error parsing config file at '{configPath}'", ex);
        }
        return instance;
    }

    /// <summary>
    /// Updates the config.
    /// Ensure all properies are valid, or call GetAppPaths() and update it instead.
    /// </summary>
    /// <returns></returns>
    public async Task SaveAsync()
    {
        string configPath = ConfigFileAttribute.GetConfigFilePath<T>();
        string json = JsonSerializer.Serialize(this as T, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        await File.WriteAllTextAsync(configPath, json);
    }
}