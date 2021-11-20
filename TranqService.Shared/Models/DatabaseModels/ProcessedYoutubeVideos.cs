using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranqService.Shared.Models.DatabaseModels
{
    public class ProcessedYoutubeVideos
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string VideoId { get; set; }
    }
}
