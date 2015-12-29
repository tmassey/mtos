using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using aXon.Data;
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
            ModuleName = "Inventory";
            SourcePath = new Uri("/Pages/Modules/Robotics/Robot/Rovers.xaml", UriKind.Relative);
            DataService = new aXonEntities();
            Loaded += Rovers_Loaded;
        }

        private void Rovers_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        private void MainGrid_OnSelectionChanged_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddWarehouseRobotView view = new AddWarehouseRobotView();
            if (view.ShowDialog()==true)
            {
                
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            //TODO: need to know Current Warehouse
            RefreshData();
        }

        private void RefreshData()
        {
            var networks =
                new ObservableCollection<WarehouseRobot>(DataService.WarehouseRobots.Where(n => n.IsActiveRecord == true));
            MainGrid.ItemsSource = networks;
        }
    }
}
