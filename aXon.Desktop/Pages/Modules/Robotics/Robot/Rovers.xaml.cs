using System;
using System.Windows;
using System.Windows.Controls;
using aXon.Rover;

namespace aXon.Desktop.Pages.Modules.Robotics.Robot
{
    /// <summary>
    /// Interaction logic for Rovers.xaml
    /// </summary>
    public partial class Rovers : AxonScreen
    {
        public Rovers()
        {
            InitializeComponent();
            ScreenName = "Robots";
            ModuleName = "Robotics";
            SourcePath = new Uri("/Pages/Modules/Robotics/Robot/Rovers.xaml", UriKind.Relative);
        }

        private void MainGrid_OnSelectionChanged_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (DataService == null)
            {
                DataService = new MongoDataService();
            }
            var networks = DataService.GetCollectionQueryModel<aXon.Warehouse.Modules.Robotics.Robot.Models.Robot>();
            MainGrid.ItemsSource = networks;
        }

       
    }
}
