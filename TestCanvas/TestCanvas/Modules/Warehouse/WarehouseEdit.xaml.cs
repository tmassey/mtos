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
using MongoDB.Driver;

namespace aXon.Warehouse.Desktop.Modules.Warehouse
{
    /// <summary>
    /// Interaction logic for WarehouseEdit.xaml
    /// </summary>
    public partial class WarehouseEdit : AxonScreen
    {
        private DataSource _sourceData;

        public WarehouseEdit()
        {
            InitializeComponent();
            ScreenName = "Warehouse Edit";
            ModuleName = "Warehouse";
        }
        public override DataSource SourceData
        {
            get { return _sourceData; }
            set
            {
                if (Equals(value, _sourceData)) return;
                _sourceData = value;
                OnPropertyChanged();
            }
        }

        private void CommitWarehouse(object sender, RoutedEventArgs e)
        {
            SourceData.Warehouse.GridLength = SourceData.Warehouse.Length / 4;
            SourceData.Warehouse.GridWidth = SourceData.Warehouse.Width / 4;
            var col = DataService.DataBase.GetCollection<Rover.Models.Warehouse>("Warehouse");
            SafeModeResult safeModeResult = col.Save(SourceData.Warehouse);

            MessageBox.Show("Success!");
        }

        private void RunNetworks(object sender, RoutedEventArgs e)
        {
            Shell.RunNetworks();
        }
    }
}
