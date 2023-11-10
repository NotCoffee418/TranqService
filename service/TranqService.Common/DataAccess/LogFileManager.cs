namespace TranqService.Common.DataAccess;

// Called before DI. Don't make interface
public static class LogFileManager
{
    const string LogFileFormat = "yyyy-MM-dd-HH-mm";

    /// <summary>
    /// Cleans up old log file and returns a new log file path.
    /// Should be called in program.cs
    /// </summary>
    /// <returns></returns>
    public static string CleanupAndGetNewLogFilePath()
    {
        string logBaseDir = Path.Combine(PathHelper.GetAppdataPath(false, "logs"));

        // Cleanup old logs
        DateTime expiredLogThreshold = DateTime.UtcNow.AddMonths(-1);
        string?[] expiredLogFiles = Directory.GetFiles(logBaseDir)
            // Extract valid but expired log file paths
            .Select(x =>
            {
                if (!x.EndsWith(".log"))
                    return null;

                if (!DateTime.TryParseExact(
                    Path.GetFileNameWithoutExtension(x),
                    LogFileFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime logTime))
                    return null;

                return logTime < expiredLogThreshold ? x : null;

            })
            .ToArray();
        foreach (string expLogFile in expiredLogFiles)
            if (expLogFile is not null)
                File.Delete(expLogFile);

        // Generate new log file path
        return Path.Combine(logBaseDir, DateTime.UtcNow.ToString(LogFileFormat, CultureInfo.InvariantCulture) + ".log");
    }
}