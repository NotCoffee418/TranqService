using static TranqService.Common.Data.Enums;

namespace TranqService.Database.Models; 
public class YoutubeVideoInfo
{
    public int Id { get; set; }

    [Required]
    public string VideoGuid { get; set; }

    [Required]
    public string PlaylistGuid { get; set; }

    public string Name { get; set; } = null;
    public string Uploader { get; set; } = null;

    /// <summary>
    /// If an error message occurred, we still mark it as complete.
    /// But we note the message so user can retry later.
    /// </summary>
    public string ErrorMessage { get; set; } = null;


    public string GetFileName(string outputFormat)
        => $"{Name}.{outputFormat}";
}