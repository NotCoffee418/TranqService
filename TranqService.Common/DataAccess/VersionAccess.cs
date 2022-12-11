namespace TranqService.Common.DataAccess;

public static class VersionAccess
{
    public static async Task<bool> IsServiceUpdateAvailableAsync()
    {
        var latestVersionT = GetRemoteServiceVersionTimeAsync();
        var currentVersionT = GetRemoteUiVersionTimeAsync();
        return await latestVersionT > await currentVersionT;
    }

    public static async Task<bool> IsUiServiceUpdateAvailableAsync()
    {
        var latestVersionT = GetRemoteServiceVersionTimeAsync();
        var currentVersionT = GetRemoteUiVersionTimeAsync();
        return await latestVersionT > await currentVersionT;
    }

    public static Task<DateTime?> GetRemoteServiceVersionTimeAsync()
        => GetRemoteFileVersionTime(AppConstants.LatestServiceVersionUrl);

    public static Task<DateTime?> GetRemoteUiVersionTimeAsync()
        => GetRemoteFileVersionTime(AppConstants.LatestUiVersionUrl);

    public static async Task<DateTime?> GetInstalledServiceVersionTimeAsync()
        => (await AppVersionInfo.GetAsync()).InstalledServiceVersionTime;
    
    public static async Task<DateTime?> GetInstalledUiVersionTimeAsync()
        => (await AppVersionInfo.GetAsync()).InstalledServiceVersionTime;

    private static async Task<DateTime?> GetRemoteFileVersionTime(string url)
    {
        try
        {
            HttpResponseMessage resp = await AppConstants.HTTPCLIENT.GetAsync(
                url, HttpCompletionOption.ResponseHeadersRead);
            
            if (!resp.IsSuccessStatusCode || resp.Content.Headers.LastModified is null)
                return null;

            return resp.Content.Headers.LastModified.Value.UtcDateTime;
        }
        catch
        {
            return null;
        }
    }
    
    
}
