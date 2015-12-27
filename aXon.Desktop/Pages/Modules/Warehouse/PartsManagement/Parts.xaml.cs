using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using aXon.Desktop.ViewModels.Modules.HR;
using aXon.Desktop.ViewModels.Modules.Warehouse;

namespace aXon.Desktop.Pages.Modules.Warehouse.PartsManagement
{
    /// <summary>
    /// Interaction logic for Parts.xaml
    /// </summary>
    public partial class Parts : AxonScreen
    {
        public Parts()
        {
            InitializeComponent();
            ViewModel = new PartsViewModel();
            ScreenName = "Parts Management";
            ModuleName = "Inventory";
            SourcePath = new Uri("/Pages/Modules/Warehouse/PartsManagement/Parts.xaml", UriKind.Relative);
            Loaded += Parts_Loaded;
        }
        private PartsViewModel _viewModel;
        public aXonEntities Entities { get; set; }
        public PartsViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (Equals(value, _viewModel)) return;
                _viewModel = value;
                OnPropertyChanged(nameof(ViewModel));
            }
        }
        void Parts_Loaded(object sender, RoutedEventArgs e)
        {
            Entities = new aXonEntities();
            ViewModel.MainData = new ObservableCollection<Part>(Entities.Parts);
            ViewModel.Companies = new ObservableCollection<Company>(Entities.Companies.Where(u => u.IsActiveRecord == true));
            ViewModel.Warehouses = new ObservableCollection<WareHouse>(Entities.WareHouses.Where(u => u.IsActiveRecord == true));
            ViewModel.Vendors = new ObservableCollection<Vendor>(Entities.Vendors.Where(u => u.IsActiveRecord == true));

            DataContext = ViewModel;
        }

        private void MainGrid_OnSelectionChanged_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null || e.AddedItems.Count == 0) return;
            try
            {
                ViewModel.SelectedRow = (Part)e.AddedItems[0];
                DataContext = ViewModel;
                ViewModel.EditMode = true;
            }
            catch
            {
            }
        }

        private void Add_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //ViewModel.SelectedRowAddress = new Address() { Id = Guid.NewGuid() };
            ViewModel.SelectedRow = new Part() { Id = Guid.NewGuid() };
            DataContext = ViewModel;
            ViewModel.EditMode = false;
            Globals.Toast.Message = "Ready to Add New Record!";
        }

        private void Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Entities = new aXonEntities();
            ViewModel.MainData = new ObservableCollection<Part>(Entities.Parts);
            Globals.Toast.Message = "List Refreshed!";

        }

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.SelectedRow.ModifiedBy = ViewModel.SelectedRow.Id;
            ViewModel.SelectedRow.LastEditDateTime = DateTime.Now;
            if (ViewModel.EditMode)
            {
                Entities.Parts.Attach(ViewModel.SelectedRow);
            }
            else
            {
                ViewModel.SelectedRow.CreatedBy = ViewModel.SelectedRow.Id;
                ViewModel.SelectedRow.ModifiedBy = ViewModel.SelectedRow.Id;
                ViewModel.SelectedRow.CreateDateTime = DateTime.Now;
                ViewModel.SelectedRow.LastEditDateTime = DateTime.Now;
                ViewModel.SelectedRow.IsActiveRecord = true;
                //ViewModel.SelectedRowAddress.CreatedBy = ViewModel.SelectedRowAddress.Id;
                //ViewModel.SelectedRowAddress.ModifiedBy = ViewModel.SelectedRowAddress.Id;
                //ViewModel.SelectedRowAddress.CreateDateTime = DateTime.Now;
                //ViewModel.SelectedRowAddress.LastEditDateTime = DateTime.Now;
                //Entities.Addresses.Add(ViewModel.SelectedRowAddress);
                Entities.Parts.Add(ViewModel.SelectedRow);
            }
            Entities.SaveChanges();
            Entities = new aXonEntities();
            ViewModel.MainData = new ObservableCollection<Part>(Entities.Parts);
            DataContext = ViewModel;
            Globals.Toast.Message = "Record Saved!";
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
