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

    public string GetFileName(string outputFormat)
        => $"{Name}.{outputFormat}";
}