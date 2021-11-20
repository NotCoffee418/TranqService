namespace TranqService.Shared.ApiHandlers
{
    public class YoutubeApiHandler : IYoutubeApiHandler
    {
        IConfig _config;

        private YouTubeService? _youTubeService;

        public YoutubeApiHandler(IConfig config)
        {
            _config = config;

            Authenticate();
        }

        private void Authenticate()
        {
            _youTubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApplicationName = this.GetType().ToString(),
                ApiKey = _config.YoutubeApiKey
            });
        }

        /// <summary>
        /// Gets all items in a youtube playlist as YoutubeVideoModel
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<YoutubeVideoModel>> GetAllPlaylistItemsAsync()
        {
            // Prepare request
            Repeatable<string> part = new Repeatable<string>(new string[] { "id", "snippet" });
            var request = _youTubeService.PlaylistItems.List(part);
            request.PlaylistId = "PLosVD5wwGC2vJPYxoZlQKpvXMOTBKfzLY";

            // execute and return
            var response = await request.ExecuteAsync();
            return response.Items
                .Select(x => new YoutubeVideoModel()
                {
                    VideoId = x.Id,
                    Name = x.Snippet.Title,
                    Uploader = x.Snippet.ChannelTitle
                });
        }
    }
}
