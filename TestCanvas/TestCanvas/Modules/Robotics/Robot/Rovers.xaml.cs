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
using aXon.Warehouse.BaseClasses;
using aXon.Warehouse.Enumerations;

namespace aXon.Warehouse.Desktop.Modules.Robotics.Robot
{
    /// <summary>
    /// Interaction logic for Rovers.xaml
    /// </summary>
    public partial class Rovers : AxonScreen
    {
        public Rovers()
        {
            InitializeComponent();
            ScreenName = "Rovers";
            ModuleName = "Robots;";
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

        }

       
    }
}
