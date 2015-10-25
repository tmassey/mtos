using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Encog.Neural.Networks;
using Encog.Persist;
using MongoDB.Driver;
using aXon.Rover;
using aXon.Rover.Models;

namespace TestCanvas
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            //KeyUp += MainWindow_KeyUp;
            KeyDown += MainWindow_KeyDown;
            Canvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
        }

        public MongoDataService Mds { get; set; }

        //public RobotSimulator Simulation { get; set; }
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
                case Key.NumPad7: //fowardLeft
                    SourceData.Simulation.Turn(RobotDirection.FowardLeft);
                    break;
                case Key.NumPad8: //foward
                    SourceData.Simulation.Turn(RobotDirection.Forward);
                    break;
                case Key.NumPad9: //fowardRight
                    SourceData.Simulation.Turn(RobotDirection.FowardRight);
                    break;
                case Key.NumPad4: //Left
                    SourceData.Simulation.Turn(RobotDirection.Left);
                    break;
                case Key.NumPad5: //Rest
                    SourceData.Simulation.Turn(RobotDirection.Rest);
                    break;
                case Key.NumPad6: //Right
                    SourceData.Simulation.Turn(RobotDirection.Right);
                    break;
                case Key.NumPad1: // backLeft
                    SourceData.Simulation.Turn(RobotDirection.BackLeft);
                    break;
                case Key.NumPad2: //Back
                    SourceData.Simulation.Turn(RobotDirection.Reverse);
                    break;
                case Key.NumPad3: //backRight
                    SourceData.Simulation.Turn(RobotDirection.BackRight);
                    break;
            }
            Canvas.SetLeft(pos, SourceData.Simulation.Position[1]*10);
            Canvas.SetTop(pos, SourceData.Simulation.Position[0]*10);
            Canvas.Children.Add(pos);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Mds = new MongoDataService();
            RC = new RobotContol();
            SourceData = new DataSource();
            SourceData.ModeMap = MapMode.ShipMode;
            SourceData.Simulation = new RobotSimulator(new Position(0, 0), new Position(40, 60));
            SourceData.Warehouse = Mds.GetCollectionQueryModel<Warehouse>().FirstOrDefault();
            if (SourceData.Warehouse == null)
                SourceData.Warehouse = new Warehouse {Id = Guid.NewGuid()};
            else
                DrawMap();
            DataContext = SourceData;
            ModeGrid.DataContext = SourceData;
        }

        private void BuildNetwork(double slat, double slon, double lat, double lon)
        {
            BasicNetwork network = null;
            string fn = @"c:\robotnn\Robot_From" + slat + "_" + slon + "_To_" + lat + "_" + lon + ".net";
            if (!File.Exists(fn))
            {
                Title = (fn + " Does Not Exist!");
                DoEvents();
            }
            else
            {
                lock (RobotContol.NetworkLock)
                {
                    network = (BasicNetwork) EncogDirectoryPersistence.LoadObject(new FileInfo(fn));
                }
                var pilot = new NeuralRobot(network, true, SourceLocation, DestLocation);
                SourceData.Simulation = pilot.sim;
                DataContext = SourceData;
                DrawMap();
                SourceData.Simulation.PositionChanged += Simulation_PositionChanged;
                pilot.ScorePilot();

                //Console.WriteLine(pilot.ScorePilot(SourceLocation, DestLocation));
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

        private void Simulation_PositionChanged(object sender, Position args)
        {
            Rectangle pos = AddLocation(false);
            pos.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 255));
            Canvas.SetLeft(pos, args.Longitude*10);
            Canvas.SetTop(pos, args.Latitude*10);
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
                    int x = ((int) (Canvas.ActualHeight/SourceData.Warehouse.GridWidth) - 1);
                    int y = ((int) (Canvas.ActualWidth/SourceData.Warehouse.GridLength) - 1);
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
            for (double slat = 0; slat <= 90; slat += 20)
            {
                for (double slon = 0; slon <= 90; slon += 20)
                {
                    for (double lat = 0; lat <= 90; lat += 20)
                    {
                        for (double lon = 0; lon <= 90; lon += 20)
                        {
                            SourceLocation = new Position(slat, slon);
                            DestLocation = new Position(lat, lon);
                            BuildNetwork(slat, slon, lat, lon);
                        }
                    }
                }
            }
        }

        private void CommitWarehouse(object sender, RoutedEventArgs e)
        {
            SourceData.Warehouse.GridLength = SourceData.Warehouse.Length/4;
            SourceData.Warehouse.GridWidth = SourceData.Warehouse.Width/4;
            MongoCollection<Warehouse> col = Mds.DataBase.GetCollection<Warehouse>("Warehouse");
            SafeModeResult safeModeResult = col.Save(SourceData.Warehouse);

            MessageBox.Show("Success!");
            DrawMap();
        }
    }
}