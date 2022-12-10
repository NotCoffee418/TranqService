namespace TranqService.Common.DataAccess;

// Called before DI. Don't make interface
public static class PathHelper
{
    /// <summary>
    /// Gets a path in the applications local appdata folder.
    /// Generates directory structure as needed
    /// </summary>
    /// <param name="isPathFile">file or directory?</param>
    /// <param name="subPaths">Optional path with subdirectory (if any)</param>
    /// <returns></returns>
    public static string GetAppdataPath(bool isPathFile, params string[]? subPaths)
    {
        List<string> parts = new() {
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TranqService"
        };
        if (subPaths is not null)
            parts.AddRange(subPaths);
        string fullPath = Path.Join(parts.ToArray());

        // Ensure directory exists
        string dirToEnsure = isPathFile ? Path.GetDirectoryName(fullPath) : fullPath;
        if (!Directory.Exists(dirToEnsure))
            Directory.CreateDirectory(dirToEnsure);

        // Return final desired path
        return fullPath;
    }

    /// <summary>
    /// Get path to a config file respecting override settings.
    /// </summary>
    /// <param name="configFileName"></param>
    /// <param name="forceRootLocalAppData">Reads config file from GetAppdataPath regardless of any override settings</param>
    /// <returns></returns>
    public static string GetConfigFilePath(string configFileName, bool forceRootLocalAppData = false)
    {
        // Load configs with forced location in appdata
        if (forceRootLocalAppData) 
            return GetAppdataPath(true, configFileName);

        // Determine config file location
        return Path.Combine(AppPaths.Get().AppSettingsDir, configFileName);
    }
}
