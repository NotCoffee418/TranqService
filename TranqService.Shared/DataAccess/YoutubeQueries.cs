namespace TranqService.Shared.DataAccess;
public class YoutubeQueries : IYoutubeQueries
{
    private IDatabaseConnection _databaseConnection;

    public YoutubeQueries(
        IDatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<IEnumerable<string>> GetDownloadedVideoIdsInPlaylistAsync(string playlistGuid)
    {
        using var db = await _databaseConnection.GetConnectionAsync();
        return await db.QueryAsync<string>(
            "SELECT youtube_processed_videos.videoguid " +
            "FROM youtube_processed_videos " +
            "JOIN youtube_playlists ON youtube_processed_videos.playlistid = youtube_playlists.id " +
            "WHERE youtube_playlists.playlistguid = @PlaylistStringId",
            new { PlaylistStringId = playlistGuid });
    }

    /// <summary>
    /// Finds or creates a playlist id
    /// </summary>
    /// <returns></returns>
    public async Task<int> GetPlaylistIdAsync(string playlistGuid)
    {
        using var db = await _databaseConnection.GetConnectionAsync();
        var input = new { PlaylistGuid = playlistGuid };

        // Attempt to find playlist
        int? result = await db.QueryFirstOrDefaultAsync<int>(
            "SELECT id FROM youtube_playlists WHERE playlistguid = @PlaylistGuid", input);

        // Insert and get value if doesnt exist yet
        result = await db.QueryFirstAsync<int>(
            "INSERT INTO youtube_playlists (playlistguid) VALUES (@PlaylistGuid) RETURNING id", input);

        return result.Value;
    }

    /// <summary>
    /// Mark a video as downloaded from this playlist in the database
    /// </summary>
    /// <param name="videoGuid"></param>
    /// <param name="playlistGuid"></param>
    /// <returns></returns>
    public async Task MarkVideoAsDownloadedAsync(string videoGuid, string playlistGuid)
    {
        // Get playlist id
        int playlistId = await GetPlaylistIdAsync(playlistGuid);

        // Nark as downloaded
        using var db = await _databaseConnection.GetConnectionAsync();
        await db.ExecuteAsync("INSERT INTO youtube_processed_videos (videoguid, playlistid) " +
            "VALUES (@VideoGuid, @PlaylistId) ON CONFLICT DO NOTHING");
    }
}