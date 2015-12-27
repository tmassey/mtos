using System;
using System.Windows;
using MongoDB.Driver;

namespace aXon.Desktop.Pages.Modules.Warehouse
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
            ScreenName = "Warehouse Details";
            ModuleName = "Settings";
            SourcePath = new Uri("/Pages/Modules/Warehouse/WarehouseEdit.xaml", UriKind.Relative);
            
        }
        public override DataSource SourceData
        {
            get { return _sourceData; }
            set
            {
                if (Equals(value, _sourceData)) return;
                if (Equals(value, _sourceData)) return;
                _sourceData = value;
                OnPropertyChanged("SourceData");
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
            //TODO: Fix this
            //Shell.RunNetworks();
        }
    }
}
