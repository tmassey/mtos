using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using aXon.Data;
using aXon.Desktop.ViewModels.Modules.Security;

namespace aXon.Desktop.Pages.Modules.Security
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : AxonScreen
    {
        private UsersViewModel _viewModel;
        public aXonEntities Entities { get; set; }

        public UsersViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (Equals(value, _viewModel)) return;
                _viewModel = value;
                OnPropertyChanged(nameof(ViewModel));
            }
        }

        public Users()
        {
            InitializeComponent();
            ScreenName = "Users";
            ModuleName = "Security";
            SourcePath = new Uri("/Pages/Modules/Security/Users.xaml", UriKind.Relative);
            ViewModel= new UsersViewModel();
            Loaded += Users_Loaded;
        }

        private void Users_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Entities = new aXonEntities();
            ViewModel.MainData = new ObservableCollection<User>(Entities.Users.Where(u=>u.IsActiveRecord==true));
            DataContext = ViewModel;
        }

        private void MainGrid_OnSelectionChanged_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null || e.AddedItems.Count == 0) return;
            try
            {
                ViewModel.SelectedRow = (User) e.AddedItems[0];
                DataContext = ViewModel;
                ViewModel.EditMode = true;
            }
            catch
            {
            }
        }

        private void Add_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.SelectedRow = new User() { Id=Guid.NewGuid()};
            DataContext = ViewModel;
            ViewModel.EditMode = false;
        }

        private void Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Entities = new aXonEntities();
            ViewModel.MainData = new ObservableCollection<User>(Entities.Users.Where(u => u.IsActiveRecord == true));
        }

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.SelectedRow.CreatedBy = ViewModel.SelectedRow.Id;
            ViewModel.SelectedRow.ModifiedBy = ViewModel.SelectedRow.Id;
            ViewModel.SelectedRow.CreateDateTime = DateTime.Now;
            ViewModel.SelectedRow.LastEditDateTime = DateTime.Now;
            if(ViewModel.EditMode)
                Entities.Users.Attach(ViewModel.SelectedRow);
            else
                Entities.Users.Add(ViewModel.SelectedRow);
            Entities.SaveChanges();
            Entities = new aXonEntities();
            ViewModel.MainData = new ObservableCollection<User>(Entities.Users.Where(u => u.IsActiveRecord == true));
            DataContext = ViewModel;
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
