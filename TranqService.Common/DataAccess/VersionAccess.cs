namespace TranqService.Common.DataAccess;

public static class VersionAccess
{
    static SemaphoreSlim VersionUpdateSemaphore { get; } = new SemaphoreSlim(1, 1);

    public static async Task<(bool HasUpdate, DateTime? VersionTime)> IsServiceUpdateAvailableAsync()
    {
        var latestVersionT = GetRemoteServiceVersionTimeAsync();
        var localVersionT = GetInstalledServiceVersionTimeAsync();
        return (latestVersionT is not null && 
            (await localVersionT is null || await latestVersionT > await localVersionT), 
            await latestVersionT);
    }

    public static async Task<(bool HasUpdate, DateTime? VersionTime)> IsUiUpdateAvailableAsync()
    {
        var latestVersionT = GetRemoteUiVersionTimeAsync();
        var localVersionT = GetInstalledUiVersionTimeAsync();
        return (await latestVersionT is not null && 
            (await localVersionT is null || await latestVersionT > await localVersionT), 
            await latestVersionT);
    }

    public static Task<DateTime?> GetRemoteServiceVersionTimeAsync()
        => GetRemoteFileVersionTime(AppConstants.LatestServiceVersionUrl);

    public static Task<DateTime?> GetRemoteUiVersionTimeAsync()
        => GetRemoteFileVersionTime(AppConstants.LatestUiVersionUrl);

    public static async Task<DateTime?> GetInstalledServiceVersionTimeAsync()
        => (await AppVersionInfo.GetAsync()).InstalledServiceVersionTime;
    
    public static async Task<DateTime?> GetInstalledUiVersionTimeAsync()
        => (await AppVersionInfo.GetAsync()).InstalledUiVersionTime;


    public static async Task UpdateUiVersionAsync(DateTime versionTime)
    {
        await VersionUpdateSemaphore.WaitAsync();
        AppVersionInfo vInfo = await AppVersionInfo.GetAsync();
        vInfo.InstalledUiVersionTime = versionTime;
        await vInfo.SaveAsync();
        VersionUpdateSemaphore.Release();
    }


    public static async Task UpdateServiceVersionAsync(DateTime versionTime)
    {
        await VersionUpdateSemaphore.WaitAsync();
        AppVersionInfo vInfo = await AppVersionInfo.GetAsync();
        vInfo.InstalledServiceVersionTime = versionTime;
        await vInfo.SaveAsync();
        VersionUpdateSemaphore.Release();
    }
    
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
