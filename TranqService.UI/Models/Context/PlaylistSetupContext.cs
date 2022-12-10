using System.Windows.Controls;
using System.Windows;
using System.Linq;

namespace TranqService.UI.Models.Context;

public class PlaylistSetupContext : NotificationObject
{
    public PlaylistSetupContext()
    {
        DownloadSourcesConfig = DownloadSources.Get();
    }

    public DownloadSources DownloadSourcesConfig
    {
        get => Get<DownloadSources>(nameof(DownloadSourcesConfig));
        set => Set(nameof(DownloadSourcesConfig), value);
    }


    public bool CanSave
    {
        get => Get<bool>(nameof(CanSave), overrideDefault: true);
        set => Set(nameof(CanSave), value);
    }
    
    public void Save()
    {
        CanSave = false;
        DownloadSources.GetAsync().ContinueWith(task =>
        {
            // Load file first to ensure integrity of any other properties
            DownloadSources dlSrcs = task.Result;
            dlSrcs.PlaylistDownloadEntries = DownloadSourcesConfig.PlaylistDownloadEntries
                .OrderByDescending(x => x.DateAdded)
                .DistinctBy(x => new { x.PlaylistId, x.VideoPlatform, x.OutputAs})
                .ToList();

            // Update the file with the updated properties
            dlSrcs.SaveAsync().ContinueWith(task =>
            {
                CanSave = true;
            });
        });
    }


    // This black magic is required for WPF bindings
    public IEnumerable<PlaylistDownloadEntry> PlaylistDownloadEntries
    {
        get => DownloadSourcesConfig.PlaylistDownloadEntries;
        set => DownloadSourcesConfig.PlaylistDownloadEntries = value is null ? null : value.ToList();
    }

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
