namespace TranqService.Common.Models.Configs;

[ConfigFile("DownloadSources.json")]
public class DownloadSources : ConfigBase<DownloadSources>
{
    public List<PlaylistDownloadEntry> PlaylistDownloadEntries { get; set; } = new();

    /// <summary>
    /// Used by service to indicate next session should start early.
    /// Default value is MinValue to prevent it being used before it's ever actually set.
    /// </summary>
    public DateTime LastModifiedTimeUtc { get; set; } = DateTime.MinValue;
}
