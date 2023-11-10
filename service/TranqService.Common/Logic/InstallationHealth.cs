namespace TranqService.Common.Logic;

public class InstallationHealth
{
    /// <summary>
    /// Checks that all required config items for the service to run are defined
    /// </summary>
    /// <returns></returns>
    public static async Task<bool> IsConfigAcceptableAsync()
    {
        ApiKeys apiKeys = await ApiKeys.GetAsync();

        // Ensure api key field is defined
        if (string.IsNullOrEmpty(apiKeys.YoutubeApiKey) || apiKeys.YoutubeApiKey.Length < 5)
            return false;


        // No issues found
        return true;
    }
}
