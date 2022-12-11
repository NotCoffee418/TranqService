using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TranqService.Common.DataAccess;
using TranqService.UI.Models.Context;
using TranqService.Common.Data;
using System.IO;

namespace TranqService.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitContext();
            InitializeComponent();
        }


        public FullContext FullContext { get; set; }

        // Binding enum manually because WPF doesn't like to play nice with me
        public static List<DownloadFormat> DownloadFormatEnumData {
            get => Enum.GetValues(typeof(DownloadFormat)).Cast<DownloadFormat>()
                .Where(x => x > 0) // No Unspecified or dev items
                .ToList();
                //.Select(x => new ComboBoxItem() { Content = x }).ToList();
        }

        private void InitContext()
        {
            
            FullContext = new()
            {
                AdvancedOptionsContext = new(),
                SetupContext = new(),
                PlaylistSetupContext = new(),
            };
            DataContext = FullContext;
        }

        private void RefreshPlaylists()
        {
            PlaylistEntriesItemControl.ItemsSource = null;
            PlaylistEntriesItemControl.ItemsSource = FullContext.PlaylistSetupContext.PlaylistDownloadEntries;
        }
        
        private void OpenDataDirectory_Click(object sender, RoutedEventArgs e)
            => Process.Start("explorer.exe", PathHelper.GetAppdataPath(false));

        private void SetupSave_Click(object sender, RoutedEventArgs e)
            => FullContext.SetupContext.Save();
        private void DownloadSourcesSave_Click(object sender, RoutedEventArgs e)
            => FullContext.PlaylistSetupContext.Save();
        

        private void AdvancedSettingsSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmation = MessageBox.Show(
                "Touching these settings can break the app. " + Environment.NewLine +
                "Are you sure you want to save advanced settings?",
                "Save Advanced Settings",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);
            if (confirmation == MessageBoxResult.Yes)
                FullContext.AdvancedOptionsContext.Save();
        }

        private void GetYtApi_Click(object sender, RoutedEventArgs e)
            => Process.Start("explorer.exe", "https://console.cloud.google.com/apis/library/youtube.googleapis.com");

        private void SelectPlaylistDirectory_Click(object sender, RoutedEventArgs e)
        {
            // Grab the relevant playlist entry from the button's tag
            var playlistEntry = ((Button)sender).Tag as PlaylistDownloadEntry;

            // Check if selected directory is valid and exists
            string? processedKnownDir = null;
            if (!string.IsNullOrEmpty(playlistEntry.OutputDirectory))
            {
                processedKnownDir = PathHelper.GetProcessedWildcardDirectory(playlistEntry.OutputDirectory, skipDirectoryCreate: true);
                if (!Directory.Exists(processedKnownDir))
                    processedKnownDir = null;
            }

            // Select ideal directory for the defined settings
            string startingDir;
            if (!string.IsNullOrEmpty(processedKnownDir))
                startingDir = processedKnownDir;
            else if (playlistEntry.OutputAs == DownloadFormat.Audio)
                startingDir = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            else if (playlistEntry.OutputAs == DownloadFormat.Video)
                startingDir = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            else startingDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // Request folder select
            var fbd = new WinForms.FolderBrowserDialog()
            {
                SelectedPath = startingDir,
                ShowNewFolderButton = true
            };
            WinForms.DialogResult result = fbd.ShowDialog();

            // Update if selected
            if (result == WinForms.DialogResult.OK)
                playlistEntry.OutputDirectory = fbd.SelectedPath;            
        }
        
        private void RemovePlaylist_Click(object sender, RoutedEventArgs e)
        {
            // Request confirmation
            MessageBoxResult result = MessageBox.Show("Are you sure you want to remove this playlist from downloading any new additions?", 
                "Remove Playlist",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning, 
                MessageBoxResult.No);
            if (result != MessageBoxResult.Yes)
                return;

            // Grab the relevant playlist entry from the button's tag
            var entry = ((Button)sender).Tag as PlaylistDownloadEntry;

            // Find and remove the entry
            FullContext.PlaylistSetupContext.DownloadSourcesConfig.PlaylistDownloadEntries
                .RemoveAll(x => 
                x.PlaylistId == entry.PlaylistId && 
                x.OutputAs == entry.OutputAs && 
                x.VideoPlatform == entry.VideoPlatform && 
                x.DateAdded == entry.DateAdded);
            FullContext.PlaylistSetupContext.Save();

            // Refresh UI
            RefreshPlaylists();
        }
    }
}
