namespace TranqService.UI.Models.Context
{
    public class PlaylistCreationInfo : NotificationObject
    {
        public string PlaylistUrl
        {
            get => Get<string>(nameof(PlaylistUrl));
            set => Set(nameof(PlaylistUrl), value);
        }

        public string OutputDirectory
        {
            get => Get<string>(nameof(OutputDirectory));
            set => Set(nameof(OutputDirectory), value);
        }
        public DownloadFormat OutputAs
        {
            get => Get<DownloadFormat>(nameof(OutputAs), DownloadFormat.Audio);
            set => Set(nameof(OutputAs), value);
        }
    }
}