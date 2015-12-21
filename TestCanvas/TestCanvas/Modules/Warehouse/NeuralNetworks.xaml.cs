using System;
using System.Collections.Generic;
using System.IO;
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
using RabbitMQ.Client;
using aXon.Rover;
using aXon.Rover.Models;
using Encog.Neural.Networks;
using Encog.Persist;
using MongoDB.Driver.Builders;
using aXon.TaskTransport;
using aXon.TaskTransport.Messages;

namespace aXon.Warehouse.Desktop.Modules.Warehouse
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
            var networks = DataService.GetCollectionQueryModel<NeuralNetwork>();
            Networks.ItemsSource = networks;
        }

        private void RunNetwork(object sender, RoutedEventArgs e)
        {
            //DoEvents();
            //BasicNetwork network = null;
            var id = (Guid)((Button)e.Source).Tag;
            Shell.RunNetwork(id);
            //var fn = id.ToString();
            //var net = DataService.GetCollectionQueryModel<NeuralNetwork>(Query.EQ("_id", id)).FirstOrDefault();
            //if (net != null)
            //{
            //    Shell.LoadScreen("");
            //    Shell.SourceLocation = new Position(net.StartPosition.X, net.StartPosition.Y);
            //    Shell.DestLocation = new Position(net.EndPosition.X, net.EndPosition.Y);

            //    lock (RobotContol.NetworkLock)
            //    {
            //        var rawbytes = DataService.OpenFile(id);

            //        File.WriteAllBytes(fn, rawbytes);
            //        network = (BasicNetwork)EncogDirectoryPersistence.LoadObject(new FileInfo(fn));
            //    }
            //    var pilot = new NeuralRobot(network, true, Shell.SourceLocation, Shell.DestLocation);
            //    SourceData.Simulation = pilot.sim;
            //    DataContext = SourceData;
            //    DrawMap();
            //    SourceData.Simulation.PositionChanged += Simulation_PositionChanged;
            //    pilot.ScorePilot();
            //    File.Delete(fn);

            //}

            //DoEvents();
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
