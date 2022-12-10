using System.Windows.Controls;
using System.Windows;

namespace TranqService.UI.Models.Context;

public class PlaylistSetupContext : NotificationObject
{
    public PlaylistSetupContext()
    {
        DownloadSources.Get();
        PlaylistDownloadEntries = // playlistEntry.OutputDirectory = fbd.SelectedPath;

        // test entries
        new List<PlaylistDownloadEntry>() {
            new()
            {
                OutputAs = DownloadFormat.Audio,
                OutputDirectory = "C:\\Users\\username\\Downloads\\TranqService\\",
                VideoPlatform = Platform.YouTube,
                PlaylistId = "owo"
            }
        };
    }

    public DownloadSources DownloadSourcesConfig
    {
        get => Get<DownloadSources>(nameof(DownloadSourcesConfig));
        set => Set(nameof(DownloadSourcesConfig), value);
    }


    // This black magic is required for WPF bindings
    public IEnumerable<PlaylistDownloadEntry> PlaylistDownloadEntries
    {
        get => Get<List<PlaylistDownloadEntry>>(nameof(PlaylistDownloadEntries ));
        set => Set(nameof(PlaylistDownloadEntries), value);
    }
    
    //{
    //    get => DownloadSourcesConfig.PlaylistDownloadEntries;
    //    set => DownloadSourcesConfig.PlaylistDownloadEntries = value.ToList();
    //}

    public static readonly DependencyProperty PlaylistDownloadEntriesProperty =
        DependencyProperty.Register(
            nameof(PlaylistDownloadEntries),
            typeof(ItemCollection),
            typeof(MainWindow),
            new PropertyMetadata(default(ItemCollection), OnPlaylistDownloadEntriesPropertyChanged));

    private static async void OnPlaylistDownloadEntriesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        
    }
}
