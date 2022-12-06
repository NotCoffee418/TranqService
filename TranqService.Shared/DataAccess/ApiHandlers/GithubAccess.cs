namespace TranqService.Shared.DataAccess.ApiHandlers;

public class GithubAccess : IGithubAccess
{
    private readonly ILogger _logger;

    public GithubAccess(
        ILogger logger)
    {
        _logger = logger;
    }

    public async Task<DateTime> GetLatestYtDlpVersionAsync()
    {
        try
        {
            // API Call to fetch releases info for yt-dlp
            // see: https://docs.github.com/en/rest/releases/releases?apiVersion=2022-11-28#list-releases
            // API calls require user agent or we get 403
            HttpRequestMessage req = new HttpRequestMessage(
                HttpMethod.Get, "https://api.github.com/repos/yt-dlp/yt-dlp/releases");
            req.Headers.UserAgent.Add(new("TranqService", "1.0"));
            HttpResponseMessage jsonResponse = await AppConstants.HTTPCLIENT.SendAsync(req);
            if (!jsonResponse.IsSuccessStatusCode)
            {
                string err = "Failed with http error " + jsonResponse.StatusCode.ToString();
                throw new Exception(err);
            }

            // Parse response as dynamic
            dynamic? dynResp = JsonNode.Parse(await jsonResponse.Content.ReadAsStringAsync());
            if (dynResp is null)
                throw new NullReferenceException("Parsed jsonResponse was null");

            // Get latest release data and return it
            return DateTime.Parse(dynResp[0]["published_at"].ToString());
        }
        catch (Exception ex)
        {
            _logger.Error("GetLatestYtDlpVersionAsync(): " + ex.Message);
            throw;
        }
    }
}
