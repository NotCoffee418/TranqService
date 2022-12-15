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
    /// Wildcards ok. Directory does not need to exist yet.
    /// </summary>
    public DateTime DateAdded
    {
        get => Get<DateTime>(nameof(DateAdded), DateTime.UtcNow);
        set => Set(nameof(DateAdded), value);
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

        if (string.IsNullOrEmpty(OutputDirectory))
            return (false, "Save directory must be defined");
        
        if (OutputDirectory.Contains(":/"))
            return (false, "Directory path must be a directory. Detected possible url");
        
        if (!PathHelper.IsValidDirectoryPath(OutputDirectory))
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
        // Validate inputs
        if (string.IsNullOrEmpty(playlistUrl) || string.IsNullOrEmpty(unformattedOutputDirectory))
            return null;

        // Prepare base entry
        PlaylistDownloadEntry entry = new PlaylistDownloadEntry()
        {
            OutputDirectory = unformattedOutputDirectory,
            OutputAs = outputAs
        };

        // Try youtube
        // Finds: https://youtube.com/playlist?list=PLRESTOFTHEID&somethingsomething
        // Also finds plain id! this can be a hazard maybe
        Regex rYoutubeEntry = new Regex(@"(\S+youtu.+\/playlist?.+list=)?(PL[a-zA-Z0-9\-_]+)(&.+)?");
        if (rYoutubeEntry.IsMatch(playlistUrl))
        {
            entry.VideoPlatform = Platform.YouTube;
            entry.OutputDirectory = unformattedOutputDirectory;
            entry.PlaylistId = rYoutubeEntry.Match(playlistUrl).Groups[2].Value;
            return entry;
        }

        // Try other platforms here


        // No matches found, return null
        return null;
    }

    public static string? ExtractVideoGuidFromUrl(string videoUrl)
    {
        if (string.IsNullOrEmpty(videoUrl))
            return null;
        
        // Match YouTube url
        Regex rYoutubeVideo = new Regex(@"\S+((youtube.+\/watch?.+?v=)|(youtu.be\/))([a-zA-Z0-9\-_]+)(&.+)?");
        if (rYoutubeVideo.IsMatch(videoUrl))
        {
            return rYoutubeVideo.Match(videoUrl).Groups[4].Value;
        }

        // No matches. Return null
        return null;
    }
}
