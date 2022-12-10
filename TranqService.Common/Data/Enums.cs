using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranqService.Common.Data
{
    public static class Enums
    {
        public enum Platform
        {
            Unspecified = 0,
            YouTube = 1,
        }

        public enum DownloadFormat
        {
            Unspecified = 0,
            Audio = 1, // mp3
            Video = 2, // mp4
        }
    }
}
