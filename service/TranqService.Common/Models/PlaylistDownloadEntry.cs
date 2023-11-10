using System.Text.Json.Serialization;

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
    /// Indicates that this playlist had an error last attempt.
    /// Define through SetPlaylistAsError().
    /// </summary>
    public string? PlaylistError
    {
        get => Get<string?>(nameof(PlaylistError), null);
        set => Set(nameof(PlaylistError), value);
    }

    [JsonIgnore]
    public bool HasError { get => !string.IsNullOrEmpty(PlaylistError); }
    public string? DisplayName
    {
        get => Get<string?>(nameof(DisplayName), "Loading...");
        set => Set(nameof(DisplayName), value);
    }
    public DateTime LastDisplayNameCheck
    {
        get => Get<DateTime>(nameof(LastDisplayNameCheck), DateTime.UtcNow);
        set => Set(nameof(LastDisplayNameCheck), value);
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
    /// Marks a playlist as error if it's value is not null.
    /// Will show in the UI if applicable.
    /// Should be set to null again once error is resolved.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public async Task SetPlaylistAsErrorAsync(string error)
    {
        this.PlaylistError = error;
        await UpdateSingleEntryAsync();
    }

    public async Task SetDisplayNameAsync(string displayName)
    {
        this.DisplayName = displayName;
        this.LastDisplayNameCheck = displayName == null ? DateTime.MinValue : DateTime.UtcNow;
        await UpdateSingleEntryAsync();
    }

    private async Task UpdateSingleEntryAsync()
    {
        // Load current config and modify that
        DownloadSources dls = await DownloadSources.GetAsync(forceReload: true);
        for (int i = 0; i < dls.PlaylistDownloadEntries.Count; i++) // lazy search
            if (dls.PlaylistDownloadEntries[i].PlaylistId == this.PlaylistId)
            {
                dls.PlaylistDownloadEntries[i] = this;
                break;
            }
        await dls.SaveAsync();
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
