using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace aXon.Desktop.Pages.Modules.Security
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : AxonScreen
    {
        public aXonEntities Entities { get; set; }
        public ObservableCollection<User> MainData { get; set; }
        public User SelectedRow { get; set; }
        public Users()
        {
            InitializeComponent();
            ScreenName = "Users";
            ModuleName = "Security";
            SourcePath = new Uri("/Pages/Modules/Security/Users.xaml", UriKind.Relative);
            Loaded += Users_Loaded;
        }

        private void Users_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Entities = new aXonEntities();
            MainData = new ObservableCollection<User>(Entities.Users.Where(u=>u.IsActiveRecord==true));
            DataContext = this;
        }

        private void MainGrid_OnSelectionChanged_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedRow=(User)e.AddedItems[0];
            DataContext = this;
        }

        private void Add_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SelectedRow = new User() { Id=Guid.NewGuid()};
            DataContext = this;
        }

        private void Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MainData = new ObservableCollection<User>(Entities.Users.Where(u => u.IsActiveRecord == true));
        }

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Entities.Users.Attach(SelectedRow);
            Entities.SaveChanges();
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
