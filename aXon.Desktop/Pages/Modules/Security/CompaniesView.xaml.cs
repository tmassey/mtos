using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace aXon.Desktop.Pages.Modules.Security
{
    /// <summary>
    /// Interaction logic for CompaniesView.xaml
    /// </summary>
    public partial class CompaniesView : AxonScreen
    {

        private CompaniesViewModel _viewModel;
        public aXonEntities Entities { get; set; }

        public CompaniesViewModel  ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (Equals(value, _viewModel)) return;
                _viewModel = value;
                OnPropertyChanged(nameof(ViewModel));
            }
        }
        public CompaniesView()
        {
            InitializeComponent();
            ScreenName = "Companies";
            ModuleName = "Security";
            SourcePath = new Uri("/Pages/Modules/Security/CompaniesView.xaml", UriKind.Relative);
            ViewModel = new CompaniesViewModel();
            Loaded += CompaniesView_Loaded;
        }

        private void CompaniesView_Loaded(object sender, RoutedEventArgs e)
        {
            Entities = new aXonEntities();
            ViewModel.MainData = new ObservableCollection<Company>(Entities.Companies.Where(u => u.IsActiveRecord == true));
            DataContext = ViewModel;
        }

        private void MainGrid_OnSelectionChanged_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null || e.AddedItems.Count == 0) return;
            try
            {
                ViewModel.SelectedRow = (Company)e.AddedItems[0];
                DataContext = ViewModel;
                ViewModel.EditMode = true;
            }
            catch
            {
            }
        }

        private void Add_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.SelectedRowAddress = new Address() { Id = Guid.NewGuid() };
            ViewModel.SelectedRow = new Company() { Id = Guid.NewGuid() ,HeadquartersAddressId = ViewModel.SelectedRowAddress.Id};            
            DataContext = ViewModel;
            ViewModel.EditMode = false;
        }

        private void Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Entities = new aXonEntities();
            ViewModel.MainData = new ObservableCollection<Company>(Entities.Companies.Where(u => u.IsActiveRecord == true));
        }

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.SelectedRow.ModifiedBy = ViewModel.SelectedRow.Id;
            ViewModel.SelectedRow.LastEditDateTime = DateTime.Now;
            if (ViewModel.EditMode)
            {
                Entities.Companies.Attach(ViewModel.SelectedRow);
            }
            else
            {
                ViewModel.SelectedRow.CreatedBy = ViewModel.SelectedRow.Id;
                ViewModel.SelectedRow.ModifiedBy = ViewModel.SelectedRow.Id;
                ViewModel.SelectedRow.CreateDateTime = DateTime.Now;
                ViewModel.SelectedRow.LastEditDateTime = DateTime.Now;
                ViewModel.SelectedRowAddress.CreatedBy = ViewModel.SelectedRowAddress.Id;
                ViewModel.SelectedRowAddress.ModifiedBy = ViewModel.SelectedRowAddress.Id;
                ViewModel.SelectedRowAddress.CreateDateTime = DateTime.Now;
                ViewModel.SelectedRowAddress.LastEditDateTime = DateTime.Now;
                Entities.Addresses.Add(ViewModel.SelectedRowAddress);
                Entities.Companies.Add(ViewModel.SelectedRow);                
            }
            Entities.SaveChanges();
            Entities = new aXonEntities();
            ViewModel.MainData = new ObservableCollection<Company>(Entities.Companies.Where(u => u.IsActiveRecord == true));
            DataContext = ViewModel;
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
