namespace TranqService.Shared.Models.ApplicationModels.YoutubeDownloaderService; 
public class YoutubeVideoModel
{
    public string VideoGuid { get; set; }
    public string PlaylistGuid { get; set; }
    public string Name { get; set; }
    public string Uploader { get; set; }
    public bool IsDownloaded { get; set; } = false;
    public bool IsDuplicate { get; set; } = false;

    /// <summary>
    /// Get filename with ext, no full path
    /// </summary>
    /// <param name="ext"></param>
    /// <returns></returns>
    public string GetFileName(string ext)
        => Name + '.' + ext;
}