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
using aXon.Desktop.ViewModels.Modules.HR;
using aXon.Desktop.ViewModels.Modules.Security;

namespace aXon.Desktop.Pages.Modules.HR
{
    /// <summary>
    /// Interaction logic for EmployeesView.xaml
    /// </summary>
    public partial class EmployeesView : AxonScreen
    {
        private EmployeesViewModel _viewModel;
        public aXonEntities Entities { get; set; }
        public EmployeesViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (Equals(value, _viewModel)) return;
                _viewModel = value;
                OnPropertyChanged(nameof(ViewModel));
            }
        }
        public EmployeesView()
        {
            InitializeComponent();
            ViewModel = new EmployeesViewModel();
            ScreenName = "Employees";
            ModuleName = "Human Resources";
            SourcePath = new Uri("/Pages/Modules/HR/EmployeesView.xaml", UriKind.Relative);
            Loaded += EmployeesView_Loaded;
        }

        private void EmployeesView_Loaded(object sender, RoutedEventArgs e)
        {
            Entities = new aXonEntities();
            ViewModel.MainData = new ObservableCollection<Employee>(Entities.Employees.Where(u => u.IsActiveRecord == true));
            ViewModel.Companies = new ObservableCollection<Company>(Entities.Companies.Where(u => u.IsActiveRecord == true));
            DataContext = ViewModel;
        }

        private void MainGrid_OnSelectionChanged_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null || e.AddedItems.Count == 0) return;
            try
            {
                ViewModel.SelectedRow = (Employee)e.AddedItems[0];
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
            ViewModel.SelectedRow = new Employee() { Id = Guid.NewGuid()};
            DataContext = ViewModel;
            ViewModel.EditMode = false;
            Globals.Toast.Message = "Ready to Add New Record!";
        }

        private void Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Entities = new aXonEntities();
            ViewModel.MainData = new ObservableCollection<Employee>(Entities.Employees.Where(u => u.IsActiveRecord == true));
            Globals.Toast.Message = "Data Refreshed!";
            
        }

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.SelectedRow.ModifiedBy = ViewModel.SelectedRow.Id;
            ViewModel.SelectedRow.LastEditDateTime = DateTime.Now;
            if (ViewModel.EditMode)
            {
                Entities.Employees.Attach(ViewModel.SelectedRow);
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
                //Entities.Addresses.Add(ViewModel.SelectedRowAddress);
                Entities.Employees.Add(ViewModel.SelectedRow);
            }
            Entities.SaveChanges();
            Entities = new aXonEntities();
            ViewModel.MainData = new ObservableCollection<Employee>(Entities.Employees.Where(u => u.IsActiveRecord == true));
            DataContext = ViewModel;
            Globals.Toast.Message = "Record Saved";
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
