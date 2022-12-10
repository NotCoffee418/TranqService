namespace TranqService.Common.Models.Configs;

[ConfigFile("DownloadSources.json")]
public class DownloadSources : ConfigBase<DownloadSources>
{
    public List<PlaylistDownloadEntry> PlaylistDownloadEntries { get; set; } = new();

}
