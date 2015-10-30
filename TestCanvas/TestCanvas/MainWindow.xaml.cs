using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Encog.Neural.Networks;
using Encog.Persist;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RabbitMQ.Client;
using aXon.Models.Enumerations;
using aXon.Models.JobModels;
using aXon.Rover;
using aXon.Rover.Enumerations;
using aXon.Rover.Models;
using aXon.TaskTransport;
using aXon.TaskTransport.Messages;
using TaskStatus = aXon.Models.Enumerations.TaskStatus;

namespace aXon.Warehouse.Desktop
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static IConnection _Connection;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            
            KeyDown += MainWindow_KeyDown;
            Canvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
        }

        public MongoDataService Mds { get; set; }

        public DataSource SourceData { get; set; }
        public RobotContol RC { get; set; }
        public static Position SourceLocation { get; set; }
        public static Position DestLocation { get; set; }
        protected static List<double> Scores { get; set; }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(Canvas);
            int x = (int) pos.X/((int) (Canvas.ActualHeight/SourceData.Warehouse.GridWidth) - 1);
            int y = (int) pos.Y/((int) (Canvas.ActualWidth/SourceData.Warehouse.GridLength) - 1);
            Rectangle p = AddLocation(false);
            switch (SourceData.ModeMap)
            {
                case MapMode.ChargeMode:
                    p.Fill = new SolidColorBrush(Color.FromRgb(220, 20, 60));
                    break;
                case MapMode.ObstructionMode:
                    p.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    break;
                case MapMode.PersonMode:
                    p.Fill = new SolidColorBrush(Color.FromRgb(255, 165, 0));
                    break;
                case MapMode.PickupMode:
                    p.Fill = new SolidColorBrush(Color.FromRgb(138, 43, 226));
                    break;
                case MapMode.ShipMode:
                    p.Fill = new SolidColorBrush(Color.FromRgb(127, 255, 0));
                    break;
                case MapMode.StorageMode:
                    p.Fill = new SolidColorBrush(Color.FromRgb(165, 42, 42));
                    break;
                case MapMode.PathMode:
                    p.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                    break;
            }
            if (SourceData.Warehouse.Positions == null)
                SourceData.Warehouse.Positions = new ObservableCollection<Position>();
            Position l =
                (from loc in SourceData.Warehouse.Positions where loc.X == x && loc.Y == y select loc).FirstOrDefault();
            if (l != null)
            {
                l.MapMode = SourceData.ModeMap;
            }
            else
            {
                l = new Position(x, y) {MapMode = SourceData.ModeMap};
                SourceData.Warehouse.Positions.Add(l);
            }
            MongoCollection<Rover.Models.Warehouse> col = Mds.DataBase.GetCollection<Rover.Models.Warehouse>("Warehouse");
            SafeModeResult safeModeResult = col.Save(SourceData.Warehouse);
            Canvas.SetLeft(p, x*((int) (Canvas.ActualHeight/SourceData.Warehouse.GridWidth) - 1));
            Canvas.SetTop(p, y*((int) (Canvas.ActualWidth/SourceData.Warehouse.GridLength) - 1));
            Canvas.Children.Add(p);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            MoveToPosition(e);
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            MoveToPosition(e);
        }

        private void MoveToPosition(KeyEventArgs e)
        {
            Rectangle pos = AddLocation(false);
            pos.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 255));
            switch (e.Key)
            {
                case Key.NumPad8: //foward
                    SourceData.Simulation.Turn(CommandDirection.MoveForward);
                    break;
                case Key.NumPad4: //Left
                    SourceData.Simulation.Turn(CommandDirection.TurnLeft);
                    break;
                case Key.NumPad6: //Right
                    SourceData.Simulation.Turn(CommandDirection.TurnRight);
                    break;
                case Key.NumPad2: //Back
                    SourceData.Simulation.Turn(CommandDirection.MoveInReverse);
                    break;
            }
            Canvas.SetLeft(pos, SourceData.Simulation.Position[1]*10);
            Canvas.SetTop(pos, SourceData.Simulation.Position[0]*10);
            Canvas.Children.Add(pos);
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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetWindowTitle();
            Mds = new MongoDataService();
            InitConnection();
            RC = new RobotContol(DateTime.Now,Guid.Empty);
            SourceData = new DataSource();
            SourceData.ModeMap = MapMode.ShipMode;
            SourceData.Simulation = new RobotSimulator(new Position(0, 0), new Position(40, 60));
            SourceData.Warehouse = Mds.GetCollectionQueryModel<Rover.Models.Warehouse>().FirstOrDefault();
            if (SourceData.Warehouse == null)
                SourceData.Warehouse = new Rover.Models.Warehouse
                    {
                        Id = Guid.NewGuid(),
                        Positions = new ObservableCollection<Position>()
                    };
            else
                DrawMap();
            DataContext = SourceData;
            ModeGrid.DataContext = SourceData;
        }

        private void SetWindowTitle()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version version = assembly.GetName().Version;
            Title = "aXon Warehouse Desktop Version: " + version;
        }

        private void BuildNetwork(double slat, double slon, double lat, double lon)
        {
            BasicNetwork network = null;
            NeuralNetwork nn = Mds.GetCollectionQueryModel<NeuralNetwork>(Query.And(Query.EQ("StartPosition.X", slat),Query.EQ("StartPosition.Y", slon),Query.EQ("EndPosition.X", lat),Query.EQ("EndPosition.Y", lon))).FirstOrDefault();
            if (nn == null)
            {
                var t = new TaskMessage
                    {
                        LogReportingLevel = LogLevel.Verbatium,
                        MessageId = Guid.NewGuid(),
                        ScriptType = TaskScriptType.LoadWorker,
                        TaskScript = new aXon.Worker.RoverWorker().GetType().ToString(),
                        TransmisionDateTime = DateTime.Now,
                        TaskId = Guid.NewGuid()
                    };                
                var col = Mds.DataBase.GetCollection<RoverTask>("RoverTask");
                var task = new RoverTask
                    {
                        Status=TaskTransport.TaskStatus.Pending,
                        TaskType = RoverTaskType.Train,
                        Id = t.TaskId,
                        TrainingProperties =
                            new RoverTrainProperties
                                {
                                    Id = Guid.NewGuid(),
                                    StartPosition = new Position(slat, slon),
                                    DestinationPosition = new Position(lat, lon)
                                }
                    };
                col.Save(task);
                var q = new MessageQueue<TaskMessage>(false, _Connection);
                q.Publish(t);                
                DoEvents();
            }
            else
            {
                string fn = nn.Id.ToString();
                lock (RobotContol.NetworkLock)
                {
                    byte[] rawbytes = Mds.OpenFile(nn.Id);

                    File.WriteAllBytes(fn, rawbytes);
                    network = (BasicNetwork) EncogDirectoryPersistence.LoadObject(new FileInfo(fn));
                }
                var pilot = new NeuralRobot(network, true, SourceLocation, DestLocation);
                SourceData.Simulation = pilot.sim;
                DataContext = SourceData;
                DrawMap();
                SourceData.Simulation.PositionChanged += Simulation_PositionChanged;
                pilot.ScorePilot();
                File.Delete(fn);                
            }
        }

        public void DoEvents()
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                                                     new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        public object ExitFrame(object f)
        {
            ((DispatcherFrame) f).Continue = false;

            return null;
        }

        private void Simulation_PositionChanged(object sender, Position position)
        {
            Rectangle pos = AddLocation(false);
            pos.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 255));
            Canvas.SetLeft(pos, position.Y*10);
            Canvas.SetTop(pos, position.X*10);
            Canvas.Children.Add(pos);
            Canvas.UpdateLayout();
            DoEvents();
        }

        private void DrawMap()
        {
            Canvas.Children.Clear();
            for (double slat = 0; slat <= SourceData.Warehouse.GridLength; slat++)
            {
                for (double slon = 0; slon <= SourceData.Warehouse.GridWidth; slon ++)
                {
                    bool isDest = SourceData.Simulation.Destination[1] == slon &&
                                  SourceData.Simulation.Destination[0] == slat;
                    Rectangle location = AddLocation(isDest);

                    location.Tag = slon + "," + slat;
                    double x = ((int) (Canvas.ActualHeight/SourceData.Warehouse.GridWidth) - 1);
                    double y = ((int) (Canvas.ActualWidth/SourceData.Warehouse.GridLength) - 1);

                    Position pos =
                        (from loc in SourceData.Warehouse.Positions where loc.X == slon && loc.Y == slat select loc)
                            .FirstOrDefault();

                    if (pos != null)
                    {
                        switch (pos.MapMode)
                        {
                            case MapMode.ChargeMode:
                                location.Fill = new SolidColorBrush(Color.FromRgb(220, 20, 60));
                                break;
                            case MapMode.ObstructionMode:
                                location.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                                break;
                            case MapMode.PersonMode:
                                location.Fill = new SolidColorBrush(Color.FromRgb(255, 165, 0));
                                break;
                            case MapMode.PickupMode:
                                location.Fill = new SolidColorBrush(Color.FromRgb(138, 43, 226));
                                break;
                            case MapMode.ShipMode:
                                location.Fill = new SolidColorBrush(Color.FromRgb(127, 255, 0));
                                break;
                            case MapMode.StorageMode:
                                location.Fill = new SolidColorBrush(Color.FromRgb(165, 42, 42));
                                break;
                            case MapMode.PathMode:
                                location.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                                break;
                        }
                    }

                    Canvas.SetLeft(location, slon*x);
                    Canvas.SetTop(location, slat*y);
                    Canvas.Children.Add(location);
                }
            }
            Canvas.UpdateLayout();
            DoEvents();
        }

        private Rectangle AddLocation(bool isDestination = false)
        {
            var container = new Rectangle();
            if (!isDestination)
            {
                container.Stroke = new SolidColorBrush(Color.FromRgb(200, 200, 200));
                container.StrokeThickness = 1;
            }
            else
            {
                container.Stroke = new SolidColorBrush(Color.FromRgb(255, 128, 128));
                container.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                container.StrokeThickness = 2;
            }

            container.MouseRightButtonUp += container_MouseRightButtonUp;
            container.Width = Canvas.ActualWidth/SourceData.Warehouse.GridWidth;
            container.Height = Canvas.ActualHeight/SourceData.Warehouse.GridLength;
            return container;
        }

        private void container_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var src = (Rectangle) e.Source;
            if (src.Tag != null) MessageBox.Show(src.Tag.ToString(), "Location Clicked");
        }

        private void RunNetworks(object sender, RoutedEventArgs e)
        {
            var reclocs = (from p in SourceData.Warehouse.Positions where p.MapMode == MapMode.PickupMode select p);
            var shiplocs = (from p in SourceData.Warehouse.Positions where p.MapMode == MapMode.ShipMode select p);
            var storelocs = (from p in SourceData.Warehouse.Positions where p.MapMode == MapMode.StorageMode select p);
            var chrlocs = (from p in SourceData.Warehouse.Positions where p.MapMode == MapMode.ChargeMode select p);
            foreach (var cl in chrlocs)
            {
                SourceLocation = new Position(cl.X, cl.Y);
                foreach (var l in reclocs.ToArray())
                {                   
                    DestLocation = new Position(l.X, l.Y);
                    BuildNetwork(cl.X, cl.Y, l.X, l.Y);
                }
                foreach (var l in shiplocs.ToArray())
                {
                    DestLocation = new Position(l.X, l.Y);
                    BuildNetwork(cl.X, cl.Y, l.X, l.Y);
                }
                foreach (var l in storelocs.ToArray())
                {
                    DestLocation = new Position(l.X, l.Y);
                    BuildNetwork(cl.X, cl.Y, l.X, l.Y);
                }
            }
            foreach (var cl in reclocs)
            {
                SourceLocation = new Position(cl.X, cl.Y);
                foreach (var l in chrlocs.ToArray())
                {
                    DestLocation = new Position(l.X, l.Y);
                    BuildNetwork(cl.X, cl.Y, l.X, l.Y);
                }                
                foreach (var l in storelocs.ToArray())
                {
                    DestLocation = new Position(l.X, l.Y);
                    BuildNetwork(cl.X, cl.Y, l.X, l.Y);
                }
            }
            foreach (var cl in storelocs)
            {
                SourceLocation = new Position(cl.X, cl.Y);
                foreach (var l in chrlocs.ToArray())
                {
                    DestLocation = new Position(l.X, l.Y);
                    BuildNetwork(cl.X, cl.Y, l.X, l.Y);
                }
                foreach (var l in shiplocs.ToArray())
                {
                    DestLocation = new Position(l.X, l.Y);
                    BuildNetwork(cl.X, cl.Y, l.X, l.Y);
                }
            }
            
        }

        private void CommitWarehouse(object sender, RoutedEventArgs e)
        {
            SourceData.Warehouse.GridLength = SourceData.Warehouse.Length/4;
            SourceData.Warehouse.GridWidth = SourceData.Warehouse.Width/4;
            MongoCollection<Rover.Models.Warehouse> col = Mds.DataBase.GetCollection<Rover.Models.Warehouse>("Warehouse");
            SafeModeResult safeModeResult = col.Save(SourceData.Warehouse);

            MessageBox.Show("Success!");
            DrawMap();
        }
    }
}