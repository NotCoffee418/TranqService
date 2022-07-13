namespace TranqService.Database.Queries;
public class YoutubeVideoInfoQueries : IYoutubeVideoInfoQueries
{
    private IDb _db;

    public YoutubeVideoInfoQueries(IDb db)
    {
        _db = db;
    }

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
    public async Task MarkVideoAsDownloadedAsync(YoutubeVideoInfo videoInfo)
    {
        using var context = _db.GetContext();
        context.YoutubeVideoInfos.Add(videoInfo);
        await context.SaveChangesAsync();
    }

    public async Task<int> CountDownloadedVideosAsync()
    {
        using var context = _db.GetContext();
        return await context.YoutubeVideoInfos
            .CountAsync();
    }
}