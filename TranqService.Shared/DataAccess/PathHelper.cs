namespace TranqService.Shared.DataAccess;

public class PathHelper : IPathHelper
{
    /// <summary>
    /// Gets a path in the applications local appdata folder.
    /// Generates directory structure as needed
    /// </summary>
    /// <param name="isPathFile">file or directory?</param>
    /// <param name="subPaths">Optional path with subdirectory (if any)</param>
    /// <returns></returns>
    public string GetAppdataPath(bool isPathFile, params string[]? subPaths)
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
}
