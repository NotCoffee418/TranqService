using TranqService.Common.DataAccess;

namespace TranqService.Shared.DataAccess.Ytdlp;

[DependencyScope(Scope.Single)]
public class YtdlpPaths : IYtdlpPaths
{
    public string GetYtdlpExePath()
        => PathHelper.GetAppdataPath(true, "yt-dlp", "yt-dlp.exe");
    public string GetYtdlpVersionFilePath()
        => PathHelper.GetAppdataPath(true, "yt-dlp", "version.txt");
}
