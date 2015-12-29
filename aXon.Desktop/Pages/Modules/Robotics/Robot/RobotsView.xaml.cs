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
using aXon.Desktop.ViewModels.Modules.Robotics;
using aXon.Desktop.ViewModels.Modules.Warehouse;

namespace aXon.Desktop.Pages.Modules.Robotics.Robot
{
    /// <summary>
    /// Interaction logic for RobotsView.xaml
    /// </summary>
    public partial class RobotsView : AxonScreen
    {
        private RobotsViewModel _viewModel;
        public aXonEntities Entities { get; set; }
        public RobotsViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (Equals(value, _viewModel)) return;
                _viewModel = value;
                OnPropertyChanged(nameof(ViewModel));
            }
        }
        public RobotsView()
        {
            InitializeComponent();
            ScreenName = "aXon Robots";
            ModuleName = "Inventory";
            SourcePath = new Uri("/Pages/Modules/Robotics/Robot/RobotsView.xaml", UriKind.Relative);
            ViewModel = new RobotsViewModel();
            Loaded += RobotsView_Loaded;
        }

        private void RobotsView_Loaded(object sender, RoutedEventArgs e)
        {

            Entities = new aXonEntities();
            ViewModel.MainData = new ObservableCollection<aXonRobot>(Entities.aXonRobots.Where(u => u.IsActiveRecord == true));            
            DataContext = ViewModel;
        }

        private void MainGrid_OnSelectionChanged_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null || e.AddedItems.Count == 0) return;
            try
            {
                ViewModel.SelectedRow = (aXonRobot)e.AddedItems[0];
                DataContext = ViewModel;
                ViewModel.EditMode = true;
            }
            catch
            {
            }
        }

        private void Add_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.SelectedRow = new aXonRobot() { Id = Guid.NewGuid() };
            DataContext = ViewModel;
            ViewModel.EditMode = false;
        }

        private void Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RefreshData();
        }

        private void RefreshData()
        {
            var context = ((IObjectContextAdapter)Entities).ObjectContext;
            context.Refresh(RefreshMode.StoreWins, Entities.WareHouses);
            context.Refresh(RefreshMode.StoreWins, Entities.Companies);
            ViewModel.MainData = new ObservableCollection<aXonRobot>(Entities.aXonRobots.Where(u => u.IsActiveRecord == true));            
        }

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.SelectedRow.ModifiedBy = ViewModel.SelectedRow.Id;
            ViewModel.SelectedRow.LastEditDateTime = DateTime.Now;
            if (ViewModel.EditMode)
            {
                Entities.aXonRobots.Attach(ViewModel.SelectedRow);
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
                
                Entities.aXonRobots.Add(ViewModel.SelectedRow);
            }
            Entities.SaveChanges();
            RefreshData();
            DataContext = ViewModel;
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
