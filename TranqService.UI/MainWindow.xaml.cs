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
using TranqService.Common.Logic;

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Check initial config validity after registering event listener
            UpdateConfigValidityIndicators();
        }

        private void RefreshPlaylists()
        {
            PlaylistEntriesItemControl.ItemsSource = null;
            PlaylistEntriesItemControl.ItemsSource = FullContext.PlaylistSetupContext.PlaylistDownloadEntries;
        }

        private void UpdateConfigValidityIndicators()
        {
            // Adjust this if it changes
            int setupTabIndex = 1;

            // -- Force setup tab if config is not acceptable
            // Hacky delay to give config time to save to file
            Task.Delay(200).ContinueWith(_ =>
            {
                // Check if config is acceptable
                InstallationHealth.IsConfigAcceptableAsync()
                    .ContinueWith(task => Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        FullContext.IndicateConfigAcceptable = task.Result;
                        if (!FullContext.IndicateConfigAcceptable)
                            MainTabControl.SelectedIndex = 1;

                        // Adjust enabled-ness regardless
                        for (int i = 0; i < MainTabControl.Items.Count; i++)
                            if (MainTabControl.Items[i] is TabItem tabItem)
                            {
                                bool shouldEnable = FullContext.IndicateConfigAcceptable;
                                if (i == setupTabIndex) // Always enable setup tab
                                    tabItem.IsEnabled = true;
                                else tabItem.IsEnabled = shouldEnable;
                            }
                    })));
            });
            
            
            
        }

        private (bool UserSelected, string? Path) SelectDirectory(
            DownloadFormat selectedFormat,
            string processedKnownDir)
        {
            // Check known if selected directory is valid and exists
            if (!string.IsNullOrEmpty(processedKnownDir))
            {
                processedKnownDir = PathHelper.GetProcessedWildcardDirectory(processedKnownDir, skipDirectoryCreate: true);
                if (!Directory.Exists(processedKnownDir))
                    processedKnownDir = null;
            }

            // Select ideal directory for the defined settings
            string startingDir;
            if (PathHelper.IsValidDirectoryPath(processedKnownDir) && Directory.Exists(processedKnownDir))
                startingDir = processedKnownDir;
            else if (selectedFormat == DownloadFormat.Audio)
                startingDir = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            else if (selectedFormat == DownloadFormat.Video)
                startingDir = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            else startingDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // Request folder select
            var fbd = new WinForms.FolderBrowserDialog()
            {
                SelectedPath = startingDir,
                ShowNewFolderButton = true
            };
            WinForms.DialogResult result = fbd.ShowDialog();

            // Return selection if any
            return result == WinForms.DialogResult.OK ?
                (true, fbd.SelectedPath) : (false, null);
        }
        
        private void OpenDataDirectory_Click(object sender, RoutedEventArgs e)
            => Process.Start("explorer.exe", PathHelper.GetAppdataPath(false));

        private void SetupSave_Click(object sender, RoutedEventArgs e)
        {
            FullContext.SetupContext.Save();
            UpdateConfigValidityIndicators();
        }


        private void DownloadSourcesSave_Click(object sender, RoutedEventArgs e)
        {
            FullContext.PlaylistSetupContext.Save();
            UpdateConfigValidityIndicators();
        }
        

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
            {
                FullContext.AdvancedOptionsContext.Save();
                UpdateConfigValidityIndicators();
            }
        }

        private void GetYtApi_Click(object sender, RoutedEventArgs e)
            => Process.Start("explorer.exe", "https://console.cloud.google.com/apis/library/youtube.googleapis.com");

        private void SelectEditPlaylistDirectory_Click(object sender, RoutedEventArgs e)
        {
            // Grab the relevant playlist entry from the button's tag
            var pe = ((Button)sender).Tag as PlaylistDownloadEntry;
            (bool changed, string newDir) = SelectDirectory(pe.OutputAs, pe.OutputDirectory);
            if (changed) pe.OutputDirectory = newDir;
        }
        private void SelectCreationPlaylistDirectory_Click(object sender, RoutedEventArgs e)
        {
            // Grab the relevant playlist entry from the button's tag
            var pci = ((Button)sender).Tag as PlaylistCreationInfo;
            (bool changed, string newDir) = SelectDirectory(pci.OutputAs, pci.OutputDirectory);
            if (changed) pci.OutputDirectory = newDir;
        }

        private void AddPlaylist_Click(object sender, RoutedEventArgs e)
        {
            // Extract creation info from tag
            var pci = ((Button)sender).Tag as PlaylistCreationInfo;

            // Attempt to generate new playlist entry
            PlaylistDownloadEntry newEntry = PlaylistDownloadEntry.ExtractFromUrl(pci.PlaylistUrl, pci.OutputDirectory, pci.OutputAs);
            if (newEntry is null)
            {
                MessageBox.Show(
                    "Directory is undefined or playlist url could not be interpreted as any of the supported streaming platforms.", 
                    "Failed to extract playlist", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            (bool isValid, string validationError) = newEntry.Validate();
            if (!isValid)
            {
                MessageBox.Show(validationError, "Invalid input", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Add to playlist entries
            FullContext.PlaylistSetupContext.DownloadSourcesConfig.PlaylistDownloadEntries.Add(newEntry);
            FullContext.PlaylistSetupContext.Save();
            RefreshPlaylists();
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
