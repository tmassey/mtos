using System;
using System.Windows;
using System.Windows.Controls;

namespace aXon.Desktop.Pages.Modules.Warehouse.PartsManagement
{
    /// <summary>
    /// Interaction logic for Parts.xaml
    /// </summary>
    public partial class Parts : AxonScreen, IDisposable
    {
        public Parts()
        {
            InitializeComponent();
            ScreenName = "Parts Management";
            ModuleName = "Inventory";
            SourcePath = new Uri("/Pages/Modules/Warehouse/PartsManagement/Parts.xaml", UriKind.Relative);
            Loaded += Parts_Loaded;
        }

        void Parts_Loaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        private void MainGrid_OnSelectionChanged_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
