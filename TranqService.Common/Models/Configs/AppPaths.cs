namespace TranqService.Common.Models.Configs;

/// <summary>
/// Defines important paths for the application, can be overwritten by overrides.json
/// </summary>
[ConfigFile("AppPaths.json", forceRootLocalAppData: true)]
public class AppPaths : ConfigBase<AppPaths>
{
    // -- Define default paths
    private string appSettingsDir = PathHelper.GetAppdataPath(false, "config");
    private string databasePath = PathHelper.GetAppdataPath(true, "TranqService.sqlite");


    // -- Properties
    public string AppSettingsDir
    {
        get => appSettingsDir;
        set => appSettingsDir = value;
    }

    public string DatabaseFile
    {
        get => databasePath;
        set => databasePath = value;
    }
}
