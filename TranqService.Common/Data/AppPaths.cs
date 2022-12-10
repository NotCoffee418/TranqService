namespace TranqService.Common.Data;

/// <summary>
/// Defines important paths for the application, can be overwritten by overrides.json
/// </summary>
public class AppPaths
{
    // -- Define default paths
    private string appSettingsPath = PathHelper.GetAppdataPath(true, "appsettings.json");
    private string databasePath = PathHelper.GetAppdataPath(true, "TranqService.sqlite");

    // -- Properties
    public string AppSettingsPath
    {
        get => appSettingsPath;
        set => appSettingsPath = value;
    }

    public string DatabasePath
    {
        get => databasePath;
        set => databasePath = value;
    }

    /// <summary>
    /// Load important paths from overrides.json or use valid defaults.
    /// </summary>
    /// <returns></returns>
    public static async Task<AppPaths> GetAppPathsAsync()
    {
        string overridesPath = PathHelper.GetAppdataPath(true, "overrides.json");
        if (!File.Exists(overridesPath))
            return new AppPaths();

        string json = await File.ReadAllTextAsync(overridesPath);
        return JsonSerializer.Deserialize<AppPaths>(json) ?? new AppPaths();
    }

    /// <summary>
    /// Updates the config.
    /// Ensure all properies are valid, or call GetAppPaths() and update it instead.
    /// </summary>
    /// <returns></returns>
    public async Task Save()
    {
        string overridesPath = PathHelper.GetAppdataPath(true, "overrides.json");
        string json = JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        await File.WriteAllTextAsync(overridesPath, json);
    }
}
