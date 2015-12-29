using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using aXon.Rover.Annotations;
using aXon.Rover.Enumerations;

namespace aXon.Desktop.Pages.Modules.Robotics.Robot
{
    /// <summary>
    /// Interaction logic for AddWarehouseRobotView.xaml
    /// </summary>
    public partial class AddWarehouseRobotView : ModernDialog,INotifyPropertyChanged
    {
        private AddWarehouseRobotViewModel _viewModel;
        private bool _validated = false;
        public AddWarehouseRobotView()
        {
            InitializeComponent();

            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton, this.CancelButton };
            this.OkButton.Click += OkButton_Click;
            this.CloseButton.Click += CloseButton_Click;
            this.Closing += AddWarehouseRobotView_Closing;
            Loaded += AddWarehouseRobotView_Loaded;
        }
        public aXonEntities DataService { get; set; }
        private void AddWarehouseRobotView_Loaded(object sender, RoutedEventArgs e)
        {
            DataService= new aXonEntities();
            ViewModel = new AddWarehouseRobotViewModel
                        {
                            Warehouses =
                                new ObservableCollection<WareHouse>(
                                DataService.WareHouses.Where(
                                    w =>
                                w.IsActiveRecord &&
                                w.CompanyId == Globals.CurrentUser.CompanyId))
                        };
            DataContext = ViewModel;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _validated = true;
        }

        private void AddWarehouseRobotView_Closing(object sender, CancelEventArgs e)
        {
            if (!_validated)
                e.Cancel = true;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            bool existsContinue = false;
            bool exists = false;
            var robot = DataService.aXonRobots.FirstOrDefault(r => r.SerialNumber == ViewModel.SerialNumber);
            if (robot == null) return;
            var wrobot = DataService.WarehouseRobots.FirstOrDefault(w => w.SerialNumber == ViewModel.SerialNumber);
            if (wrobot != null)
            {
                exists = true;
                if (wrobot.CompanyId != Globals.CurrentUser.CompanyId)
                {
                    MessageBox.Show("Robot Registred to another Company Contact aXon Electronics for assistance!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    existsContinue = false;
                    _validated = false;
                }
                else if (wrobot.WarehouseId != ViewModel.WarehouseId)
                {
                    var warehouse = DataService.WareHouses.FirstOrDefault(w => w.Id == ViewModel.WarehouseId);
                    var result=MessageBox.Show(
                        "The Robot with Serial: " + ViewModel.SerialNumber + " Is already Registered to Warehouse: " +
                        wrobot.WareHouse.Name + " Do You wish to transfer to warehouse " + warehouse.Name + "?",
                        "Already Registered", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                        existsContinue = true;
                    else
                        existsContinue = false;
                    
                }
            }
            var position = DataService.WarehousePositions.FirstOrDefault(p => p.X == ViewModel.X && p.Y == ViewModel.Y && p.WarehouseId == ViewModel.WarehouseId);
            if (position == null)
            {
                MessageBox.Show("Not a Valid Position for Warehouse!", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                _validated = false;
                return;
            }
            ViewModel.RegistrationKey = ViewModel.RegistrationKey1 + ViewModel.RegistrationKey2 + ViewModel.RegistrationKey3 +ViewModel.RegistrationKey4;

            if (robot.RegistrationKey == ViewModel.RegistrationKey)
            {
                if (!exists)
                {
                    ViewModel.Robot = new WarehouseRobot
                                      {
                                          Id = robot.Id,
                                          CompanyId = Globals.CurrentUser.CompanyId,
                                          WarehouseId = ViewModel.WarehouseId,
                                          CreateDateTime = DateTime.Now,
                                          LastEditDateTime = DateTime.Now,
                                          ModifiedBy = Globals.CurrentUser.Id,
                                          CreatedBy = Globals.CurrentUser.Id,
                                          IsActiveRecord = true,
                                          CurrentMode = (int) RoverMode.Offline,
                                          SerialNumber=robot.SerialNumber,
                                          CurrentPostionId=position.Id
                                      };
                    DataService.WarehouseRobots.Add(ViewModel.Robot);
                }
                else if (exists && existsContinue)
                {
                    wrobot.WarehouseId = ViewModel.WarehouseId;
                    wrobot.LastEditDateTime = DateTime.Now;
                    wrobot.ModifiedBy = Globals.CurrentUser.Id;
                }
                else
                {
                    _validated = false;
                    return;
                }
                DataService.SaveChanges();
                _validated = true;
            }
        }

        public AddWarehouseRobotViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (Equals(value, _viewModel)) return;
                _viewModel = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
