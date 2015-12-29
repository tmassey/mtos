using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
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
using aXon.Data;
using aXon.Desktop.ViewModels.Modules.Security;
using aXon.Desktop.ViewModels.Modules.Warehouse;

namespace aXon.Desktop.Pages.Modules.Warehouse
{
    /// <summary>
    /// Interaction logic for Warehouses.xaml
    /// </summary>
    public partial class Warehouses : AxonScreen
    {
        private WarehousesViewModel _viewModel;
        public aXonEntities Entities { get; set; }

        public WarehousesViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (Equals(value, _viewModel)) return;
                _viewModel = value;
                OnPropertyChanged(nameof(ViewModel));
            }
        }
        public Warehouses()
        {
            InitializeComponent();
            ScreenName = "Warehouses";
            ModuleName = "Security";
            SourcePath = new Uri("/Pages/Modules/Warehouse/Warehouses.xaml", UriKind.Relative);
            ViewModel = new WarehousesViewModel();
            Loaded += Warehouses_Loaded;
            Details.SizeChanged += Details_SizeChanged;
        }

        private void Details_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Map.Width = Details.ActualWidth;
            Map.Height = 500;
        }

        private void Warehouses_Loaded(object sender, RoutedEventArgs e)
        {
            Entities = new aXonEntities();
            ViewModel.MainData = new ObservableCollection<WareHouse>(Entities.WareHouses.Where(u => u.IsActiveRecord == true));
            ViewModel.Companies = new ObservableCollection<Company>(Entities.Companies.Where(u => u.IsActiveRecord == true));
            DataContext = ViewModel;
            Map.WarehouseId = ViewModel.MainData.FirstOrDefault().Id;
            //Map.DrawMap();
        }

        private void MainGrid_OnSelectionChanged_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null || e.AddedItems.Count == 0) return;
            try
            {
                ViewModel.SelectedRow = (WareHouse)e.AddedItems[0];
                DataContext = ViewModel;
                Map.WarehouseId = ViewModel.SelectedRow.Id;
                ViewModel.EditMode = true;
                Map.DrawMap();
            }
            catch
            {
            }
        }

        private void Add_Click(object sender, System.Windows.RoutedEventArgs e)
        {
           ViewModel.SelectedRow = new WareHouse() { Id = Guid.NewGuid()};
            DataContext = ViewModel;
            ViewModel.EditMode = false;
        }

        private void Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RefreshData();
        }

        private void RefreshData()
        {
            var context = ((IObjectContextAdapter) Entities).ObjectContext;
            context.Refresh(RefreshMode.StoreWins, Entities.WareHouses);
            context.Refresh(RefreshMode.StoreWins, Entities.Companies);
            ViewModel.MainData = new ObservableCollection<WareHouse>(Entities.WareHouses.Where(u => u.IsActiveRecord == true));
            ViewModel.Companies = new ObservableCollection<Company>(Entities.Companies.Where(u => u.IsActiveRecord == true));
        }

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.SelectedRow.ModifiedBy = ViewModel.SelectedRow.Id;
            ViewModel.SelectedRow.LastEditDateTime = DateTime.Now;
            if (ViewModel.EditMode)
            {
               // Entities.WareHouses.Attach(ViewModel.SelectedRow);
            }
            else
            {
                ViewModel.SelectedRow.CreatedBy = ViewModel.SelectedRow.Id;
                ViewModel.SelectedRow.ModifiedBy = ViewModel.SelectedRow.Id;
                ViewModel.SelectedRow.CreateDateTime = DateTime.Now;
                ViewModel.SelectedRow.LastEditDateTime = DateTime.Now;
                //ViewModel.SelectedRowAddress.CreatedBy = ViewModel.SelectedRowAddress.Id;
                //ViewModel.SelectedRowAddress.ModifiedBy = ViewModel.SelectedRowAddress.Id;
                //ViewModel.SelectedRowAddress.CreateDateTime = DateTime.Now;
                //ViewModel.SelectedRowAddress.LastEditDateTime = DateTime.Now;
              
                Entities.WareHouses.Add(ViewModel.SelectedRow);
            }
            Entities.SaveChanges();
            RefreshData();
            DataContext = ViewModel;
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Entities.WareHouses.Remove(ViewModel.SelectedRow);
            Entities.SaveChanges();
            RefreshData();
            DataContext = ViewModel;
        }
    }
}
