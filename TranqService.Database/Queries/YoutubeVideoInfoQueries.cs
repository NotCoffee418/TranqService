namespace TranqService.Database.Queries;

[DependencyScope(Scope.Single)]
public class YoutubeVideoInfoQueries : IYoutubeVideoInfoQueries
{
    private IDb _db;

    public YoutubeVideoInfoQueries(IDb db)
    {
        _db = db;
    }

    static SemaphoreSlim markVideosAsDownloadedSemaphore = new(1, 1);

    public async Task<List<string>> GetDownloadedVideoGuidsInPlaylistAsync(string playlistGuid)
    {
        using var context = _db.GetContext();
        return await context.YoutubeVideoInfos
            .Where(x => x.PlaylistGuid.ToLower() == playlistGuid.ToLower())
            .Select(x => x.VideoGuid)
            .ToListAsync();
    }

    /// <summary>
    /// Mark a video as downloaded from this playlist in the database
    /// </summary>
    /// <param name="videoGuid"></param>
    /// <param name="playlistGuid"></param>
    /// <returns></returns>
    public async Task MarkVideosAsDownloadedAsync(params YoutubeVideoInfo[] videoInfos)
    {
        // Filter out double inserts
        // Can happen on first run with duplicate items in a playlist
        videoInfos = videoInfos
            .DistinctBy(x => new { x.VideoGuid, x.PlaylistGuid })
            .ToArray();

        // This was causing some errors since .NET 7, no info about it yet, semaphore should fix
        await markVideosAsDownloadedSemaphore.WaitAsync();
        try
        {
            using var context = _db.GetContext();
            context.YoutubeVideoInfos.AddRange(videoInfos);
            await context.SaveChangesAsync();
        }
        finally { markVideosAsDownloadedSemaphore.Release(); }
    }

    public async Task<int> CountDownloadedVideosAsync()
    {
        using var context = _db.GetContext();
        return await context.YoutubeVideoInfos
            .CountAsync();
    }

    public async Task<int> CountVideosInPlaylist(string playlistId)
    {
        using var context = _db.GetContext();
        return await context.YoutubeVideoInfos
            .Where(x => x.PlaylistGuid == playlistId)
            .CountAsync();
    }

    /// <summary>
    /// Removes all videos in a playlist from the datanase
    /// </summary>
    /// <param name="playlistId"></param>
    /// <returns></returns>
    public async Task UnregisterVideosInPlaylist(string playlistId)
    {
        using var context = _db.GetContext();
        var deletables = await context.YoutubeVideoInfos
            .Where(x => x.PlaylistGuid == playlistId)
            .ToListAsync();
        context.YoutubeVideoInfos.RemoveRange(deletables);
        await context.SaveChangesAsync();
    }
}