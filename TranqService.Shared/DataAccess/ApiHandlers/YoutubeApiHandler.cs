using static Google.Apis.Requests.BatchRequest;

namespace TranqService.Shared.DataAccess.ApiHandlers;

public class YoutubeApiHandler : IYoutubeApiHandler
{
    private YouTubeService? _youTubeService;

    public YoutubeApiHandler()
    {
        Authenticate();
    }

    private void Authenticate()
    {
        _youTubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApplicationName = this.GetType().ToString(),
            ApiKey = ApiKeys.Get().YoutubeApiKey
        });
    }

    /// <summary>
    /// Gets all items in a youtube playlist as YoutubeVideoModel
    /// </summary>
    /// <returns></returns>
    public async Task<List<YoutubeVideoInfo>> GetAllPlaylistItemsAsync(string playlistId)
    {
        // Prepare request
        Repeatable<string> part = new Repeatable<string>(new string[] { "id", "snippet" });
        var request = _youTubeService.PlaylistItems.List(part);
        request.PlaylistId = playlistId;
        request.MaxResults = 50;

        List<YoutubeVideoInfo> result = new();

        // Get all items in playlist
        PlaylistItemListResponse? response = null;
        do
        {
            // Define page token and make request
            if (response != null)
                request.PageToken = response.NextPageToken;
            response = await request.ExecuteAsync();

            // Add to result
            result.AddRange(response.Items
                .Select(x => new YoutubeVideoInfo()
                {
                    VideoGuid = x.Snippet.ResourceId.VideoId,
                    Name = Escape(x.Snippet.Title),
                    Uploader = Escape(x.Snippet.VideoOwnerChannelTitle),
                    PlaylistGuid = playlistId
                }));
        } while (response.NextPageToken != null);

        return result.DistinctBy(x => x.VideoGuid).ToList();
    }

    public async Task<(bool IsValid, string Name, string? ErrorMessage)> GetPlaylistInfoAsync(string playlistId)
    {
        // Prepare request
        Repeatable<string> part = new Repeatable<string>(new string[] { "id", "snippet", "status" });
        var request = _youTubeService.Playlists.List(part);
        request.Id = playlistId;

        // Request playlist info
        var response = await request.ExecuteAsync();
        if (response.Items.Count == 0 || response.Items[0].Status.PrivacyStatus == "private")
            return (false, "Unknown", "Invalid playlist id or private playlist");

        // Extract name
        string plName = response.Items[0].Snippet.Title;
        string chName = response.Items[0].Snippet.ChannelTitle;
        return (true, $"{chName} - {plName}", null);
    }

    /// <summary>
    /// Cleans path for usage in filesystem
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public string Escape(string input)
    {
        if (input == null)
            return "NULL";

        foreach (char c in Path.GetInvalidFileNameChars())
        {
            input = input.Replace(c, '_');
        }
        return input;
    }
}
