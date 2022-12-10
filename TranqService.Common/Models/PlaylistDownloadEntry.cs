namespace TranqService.Common.Models;

/// <summary>
/// Used by config. Should not changed or configs will break
/// </summary>
public class PlaylistDownloadEntry : NotificationObject
{
    /// <summary>
    /// 
    /// </summary>
    public Platform VideoPlatform
    {
        get => Get<Platform>(nameof(VideoPlatform));
        set => Set(nameof(VideoPlatform), value);
    }

    /// <summary>
    /// Platform independent playlist ID.
    /// </summary>
    public string PlaylistId
    {
        get => Get<string>(nameof(PlaylistId));
        set => Set(nameof(PlaylistId), value);
    }

    public DownloadFormat OutputAs
    {
        get => Get<DownloadFormat>(nameof(OutputAs));
        set => Set(nameof(OutputAs), value);
    }

    /// <summary>
    /// Wildcards ok. Directory does not need to exist yet.
    /// </summary>
    public string OutputDirectory
    {
        get => Get<string>(nameof(OutputDirectory));
        set => Set(nameof(OutputDirectory), value);
    }

    /// <summary>
    /// Check if the config has any obvious errors. 
    /// Does not check validity of playlist Ids or path existence.
    /// </summary>
    /// <returns></returns>
    public (bool IsValid, string? ValidationError) Validate()
    {
        if (VideoPlatform == Platform.Unspecified)
            return (false, "Platform was unspecified");

        if (string.IsNullOrEmpty(PlaylistId))
            return (false, "Playlist Id provided was not valid");

        if (OutputAs == DownloadFormat.Unspecified)
            return (false, "Output format was not specified");
        
        if (OutputDirectory.Contains(":/"))
            return (false, "Directory path must be a directory. Detected possible url");
        
        if (!Uri.IsWellFormedUriString(PathHelper.GetProcessedWildcardDirectory(OutputDirectory), UriKind.Absolute))
            return (false, "Directory path is not in a valid format");

        // Valid, return success
        return (true, null);
    }

    /// <summary>
    /// Attempts to extract a PlaylistDownloadEntry from a given URL.
    /// </summary>
    /// <param name="playlistUrl"></param>
    /// <param name="unformattedOutputDirectory">Wildcards acceptable, processed after.</param>
    /// <param name="outputAs"></param>
    /// <returns></returns>
    public static PlaylistDownloadEntry? ExtractFromUrl(
        string playlistUrl, string 
        unformattedOutputDirectory, 
        DownloadFormat outputAs)
    {
        PlaylistDownloadEntry entry = new PlaylistDownloadEntry()
        {
            OutputDirectory = unformattedOutputDirectory,
            OutputAs = outputAs
        };

        // Try youtube
        // Finds: https://youtube.com/playlist?list=PLRESTOFTHEID&somethingsomething
        Regex rYoutubeEntry = new Regex(@"\S+youtube\..+\/playlist?.+list=(PL[a-zA-Z0-9]+)(&.+)?");
        if (rYoutubeEntry.IsMatch(playlistUrl))
        {
            entry.VideoPlatform = Platform.YouTube;
            entry.OutputDirectory = unformattedOutputDirectory;
            return entry;
        }

        // Try other platforms here


        // No matches found, return null
        return null;
    }
}
