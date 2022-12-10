using System;
using System.Collections.Generic;
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
            => FullContext.AdvancedOptionsContext.Save();
    }
}
