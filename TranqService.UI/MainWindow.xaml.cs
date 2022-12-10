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


        private void InitContext()
        {
            FullContext = new()
            {
                AdvancedOptionsContext = new()
            };
            DataContext = FullContext;
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
                FullContext.AdvancedOptionsContext.Save();
        }

        
        private void OpenDataDirectory_Click(object sender, RoutedEventArgs e)
            => Process.Start("explorer.exe", PathHelper.GetAppdataPath(false))
    }
}
