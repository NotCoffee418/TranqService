namespace TranqService.Ytdlp.DataAccess;

[DependencyScope(Scope.Single)]
public class YtdlpPaths : IYtdlpPaths
{
    private IPathHelper _pathHelper;

    public YtdlpPaths(
        IPathHelper pathHelper)
    {
        _pathHelper = pathHelper;
    }

    public string GetYtdlpExePath()
        => _pathHelper.GetAppdataPath(true, "yt-dlp", "yt-dlp.exe");
    public string GetYtdlpVersionFilePath()
        => _pathHelper.GetAppdataPath(true, "yt-dlp", "version.txt");
}
