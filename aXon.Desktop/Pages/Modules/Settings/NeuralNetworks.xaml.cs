using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using aXon.Desktop.ViewModels.Modules.Settings;
using aXon.Desktop.ViewModels.Modules.Warehouse;
using aXon.Rover;
using aXon.Rover.Models;
using aXon.TaskTransport;
using aXon.TaskTransport.Messages;
using RabbitMQ.Client;

namespace aXon.Desktop.Pages.Modules.Settings
{
    /// <summary>
    /// Interaction logic for NeuralNetworks.xaml
    /// </summary>
    public partial class NeuralNetworks : AxonScreen
    {

        private NeuralNetworksViewModel _viewModel;
        public aXonEntities Entities { get; set; }

        public NeuralNetworks()
        {
            InitializeComponent();
            ScreenName = "Neural Networks";
            ModuleName = "Settings";
            SourcePath = new Uri("/Pages/Modules/Settings/NeuralNetworks.xaml", UriKind.Relative);
            ViewModel = new NeuralNetworksViewModel();
            Loaded += NeuralNetworks_Loaded;
            Details.SizeChanged += Details_SizeChanged;
        }
        public NeuralNetworksViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (Equals(value, _viewModel)) return;
                _viewModel = value;
                OnPropertyChanged(nameof(ViewModel));
            }
        }
        private void Details_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Map.Width = Details.ActualWidth;
            Map.Height = 500;
            Map.DrawMap();
        }

        void NeuralNetworks_Loaded(object sender, RoutedEventArgs e)
        {
            Entities = new aXonEntities();
            InitConnection();
            _messageQueue = new MessageQueue<RobotJobMessage>(false, _Connection) { GetNext = false };
            _messageQueue.OnReceivedMessage += _messageQueue_OnReceivedMessage;
            ViewModel.MainData= new ObservableCollection<WarehouseNeuralNetwork>(Entities.WarehouseNeuralNetworks.Where(u => u.IsActiveRecord == true));
            DataContext = ViewModel;            
        }

        private void _messageQueue_OnReceivedMessage(object sender, RobotJobMessage args)
        {            
        }

        private MessageQueue<RobotJobMessage> _messageQueue;
        private static IConnection _Connection;
        private void MainGrid_OnSelectionChanged_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null || e.AddedItems.Count == 0) return;
            try
            {
                ViewModel.SelectedRow = (WarehouseNeuralNetwork)e.AddedItems[0];
                DataContext = ViewModel;
                ViewModel.EditMode = true;
            }
            catch
            {
            }
        }

        private void Add_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.SelectedRow = new WarehouseNeuralNetwork() { Id = Guid.NewGuid() };
            DataContext = ViewModel;
            ViewModel.EditMode = false;
        }

        private void Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Entities = new aXonEntities();
            ViewModel.MainData = new ObservableCollection<WarehouseNeuralNetwork>(Entities.WarehouseNeuralNetworks.Where(u => u.IsActiveRecord == true));           
        }

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.SelectedRow.ModifiedBy = ViewModel.SelectedRow.Id;
            ViewModel.SelectedRow.LastEditDateTime = DateTime.Now;
            if (ViewModel.EditMode)
            {
                Entities.WarehouseNeuralNetworks.Attach(ViewModel.SelectedRow);
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

                Entities.WarehouseNeuralNetworks.Add(ViewModel.SelectedRow);
            }
            Entities.SaveChanges();
            Entities = new aXonEntities();
            ViewModel.MainData = new ObservableCollection<WarehouseNeuralNetwork>(Entities.WarehouseNeuralNetworks.Where(u => u.IsActiveRecord == true));
            DataContext = ViewModel;
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
        private static void InitConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = "192.169.164.138",
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
            _Connection = factory.CreateConnection();
        }
        private void RefreshNetworks(object sender, RoutedEventArgs e)
        {
            if (DataService == null)
            {
                DataService = new MongoDataService();
            }
            var networks = DataService.GetCollectionQueryModel<NeuralNetwork>();
                Networks.ItemsSource = networks;
            
        }

        private void RunNetwork(object sender, RoutedEventArgs e)
        {
            var id = ViewModel.SelectedRow.Id;
            //TODO: Fix this
            Map.RunNetwork(id);
            
        }

        private void ExecuteOnRobot(object sender, RoutedEventArgs e)
        {
            var id = (Guid)((Button)e.Source).Tag;
            RobotJobMessage msg = new RobotJobMessage();
            msg.JobId = Guid.NewGuid();
            msg.MessageId = Guid.NewGuid();
            msg.NetworkId = id;
            msg.RobotSerial = "000001";
            _messageQueue.Publish(msg);
            MessageBox.Show("Job Sent to Pallet Bot: " + msg.RobotSerial);
        }
    }
}
