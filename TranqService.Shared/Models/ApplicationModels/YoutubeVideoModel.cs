using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranqService.Shared.Models.ApplicationModels; 
public class YoutubeVideoModel
{
    public string VideoGuid { get; set; }
    public string PlaylistGuid { get; set; }
    public string Name { get; set; }
    public string Uploader { get; set; }
}