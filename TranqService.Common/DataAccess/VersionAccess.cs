namespace TranqService.Common.DataAccess;

public static class VersionAccess
{
    public static async Task<bool> IsServiceUpdateAvailableAsync()
    {
        var latestVersionT = GetRemoteServiceVersionTimeAsync();
        var localVersionT = GetInstalledServiceVersionTimeAsync();
        return await latestVersionT > await localVersionT;
    }

    public static async Task<bool> IsUiUpdateAvailableAsync()
    {
        var latestVersionT = GetRemoteUiVersionTimeAsync();
        var localVersionT = GetInstalledUiVersionTimeAsync();
        return await latestVersionT > await localVersionT;
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
