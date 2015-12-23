using System;
using System.Windows;
using System.Windows.Controls;
using RabbitMQ.Client;
using aXon.Rover;
using aXon.Rover.Models;
using aXon.TaskTransport;
using aXon.TaskTransport.Messages;

namespace aXon.Desktop.Pages.Modules.Warehouse
{
    /// <summary>
    /// Interaction logic for NeuralNetworks.xaml
    /// </summary>
    public partial class NeuralNetworks : AxonScreen
    {
        public NeuralNetworks()
        {
            InitializeComponent();
            ScreenName = "Neural Networks";
            ModuleName = "Warehouse";
            SourcePath = new Uri("/Pages/Modules/Warehouse/NeuralNetworks.xaml", UriKind.Relative);
            Loaded += NeuralNetworks_Loaded;
        }

        void NeuralNetworks_Loaded(object sender, RoutedEventArgs e)
        {
            InitConnection();
            _messageQueue = new MessageQueue<RobotJobMessage>(false, _Connection) { GetNext = false };
            _messageQueue.OnReceivedMessage += _messageQueue_OnReceivedMessage;
        }

        private void _messageQueue_OnReceivedMessage(object sender, RobotJobMessage args)
        {
            
        }

        private MessageQueue<RobotJobMessage> _messageQueue;
        private static IConnection _Connection;
        private void Networks_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
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
            var id = (Guid)((Button)e.Source).Tag;
            //TODO: Fix this
            //Shell.RunNetwork(id);
            
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
