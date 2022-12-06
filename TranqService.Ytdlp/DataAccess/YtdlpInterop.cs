namespace TranqService.Ytdlp.DataAccess;

public class YtdlpInterop
{
    private YtdlpPaths _ytdlpPaths;

    public YtdlpInterop(YtdlpPaths ytdlpPaths)
    {
        _ytdlpPaths = ytdlpPaths;
    }

    private async Task<string> RunYtdlpCommandAsync(string ytdlParams)
    {
        string fullCmd = $"\"{_ytdlpPaths.GetYtdlpExePath()}\" {ytdlParams}";
        throw new NotImplementedException();
    }
}
