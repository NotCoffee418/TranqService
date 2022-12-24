using System.Windows.Controls;
using TranqService.Database.Models;

namespace TranqService.UI.Models;

public class FailedDatatableModel
{
    // Additional constructors can be made for other platforms
    
	public FailedDatatableModel(YoutubeVideoInfo ytVideoInfo)
	{
        VideoName = ytVideoInfo.Name;
        VideoUrl = $"https://youtu.be/playlist?list={ytVideoInfo.PlaylistGuid}&v={ytVideoInfo.VideoGuid}";
        ErrorMessage = ytVideoInfo.ErrorMessage;
        PlaylistId = ytVideoInfo.PlaylistGuid;
        VideoId = ytVideoInfo.VideoGuid;
    }

    public string VideoName { get; }
    public string VideoUrl { get; }
    public string ErrorMessage { get; }
    public string PlaylistId { get; }
    public string VideoId { get; }
}
