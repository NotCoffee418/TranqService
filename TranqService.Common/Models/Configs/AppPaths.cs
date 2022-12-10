namespace TranqService.Common.Models.Configs;

/// <summary>
/// Defines important paths for the application, can be overwritten by overrides.json
/// </summary>
[ConfigFile("AppPaths.json")]
public class AppPaths : ConfigBase<AppPaths>
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
}
