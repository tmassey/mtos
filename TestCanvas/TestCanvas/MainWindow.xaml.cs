using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Encog.ML;
using Encog.ML.Genetic;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Persist;

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

        void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var pos=e.GetPosition(Canvas);
            int x = (int)pos.X/10;
            int y = (int)pos.Y/10;
            var p = AddLocation(false);
            p.Fill = new SolidColorBrush(Color.FromRgb(0, 245, 255));
            Canvas.SetLeft(p, x * 10);
            Canvas.SetTop(p, y * 10);
            Canvas.Children.Add(p);
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            MoveToPosition(e);
        }
        public RobotSimulator Simulation { get; set; }
        public RobotContol RC { get; set; }
        public static Position SourceLocation { get; set; }
        public static Position DestLocation { get; set; }
        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            MoveToPosition(e);
        }

        private void MoveToPosition(KeyEventArgs e)
        {
            var pos = AddLocation(false);
            pos.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 255));
            switch (e.Key)
            {
                case Key.NumPad7: //fowardLeft
                    Simulation.Turn(RobotDirection.FowardLeft);
                    break;
                case Key.NumPad8: //foward
                    Simulation.Turn(RobotDirection.Forward);
                    break;
                case Key.NumPad9: //fowardRight
                    Simulation.Turn(RobotDirection.FowardRight);
                    break;
                case Key.NumPad4: //Left
                    Simulation.Turn(RobotDirection.Left);
                    break;
                case Key.NumPad5: //Rest
                    Simulation.Turn(RobotDirection.Rest);
                    break;
                case Key.NumPad6: //Right
                    Simulation.Turn(RobotDirection.Right);
                    break;
                case Key.NumPad1: // backLeft
                    Simulation.Turn(RobotDirection.BackLeft);
                    break;
                case Key.NumPad2: //Back
                    Simulation.Turn(RobotDirection.Reverse);
                    break;
                case Key.NumPad3: //backRight
                    Simulation.Turn(RobotDirection.BackRight);
                    break;
            }
            Canvas.SetLeft(pos, Simulation.Position[1]*10);
            Canvas.SetTop(pos, Simulation.Position[0]*10);
            Canvas.Children.Add(pos);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RC = new RobotContol();
            Simulation= new RobotSimulator(new Position(0,0),new Position(40,60));
            DrawMap();
            DataContext = Simulation;
        }
        private void BuildNetwork(double slat, double slon, double lat, double lon)
        {
            BasicNetwork network = null;
            string fn = @"c:\robotnn\Robot_From" + slat + "_" + slon + "_To_" + lat + "_" + lon + ".net";
            if (!File.Exists(fn))
            {
               Title=(fn + " Does Not Exist!");
                DoEvents();
            }
            else
            {
                lock (RobotContol.NetworkLock)
                {
                    network = (BasicNetwork)EncogDirectoryPersistence.LoadObject(new FileInfo(fn));
                }
                var pilot = new NeuralRobot(network, true,SourceLocation, DestLocation);
                Simulation = pilot.sim;
                DataContext =  Simulation;
                DrawMap();
                Simulation.PositionChanged += Simulation_PositionChanged;
                pilot.ScorePilot();

                //Console.WriteLine(pilot.ScorePilot(SourceLocation, DestLocation));
            }
        }
        public void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        public object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;

            return null;
        }
        void Simulation_PositionChanged(object sender, Position args)
        {
            var pos = AddLocation(false);
            pos.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 255));
            Canvas.SetLeft(pos, args.Longitude * 10);
            Canvas.SetTop(pos, args.Latitude * 10);
            Canvas.Children.Add(pos);
            Canvas.UpdateLayout();
            DoEvents();
        }

        protected static List<double> Scores { get; set; }

        private void DrawMap()
        {
            Canvas.Children.Clear();
            for (double slat = 0; slat <= 90; slat++)
            {
                for (double slon = 0; slon <= 90; slon ++)
                {
                    bool isDest = Simulation.Destination[1] == slon && Simulation.Destination[0] == slat;
                    Rectangle location = AddLocation(isDest);
                    location.Tag = slon + "," + slat;
                    Canvas.SetLeft(location, slon*10);
                    Canvas.SetTop(location, slat*10);
                    Canvas.Children.Add(location);
                }
            }
            Canvas.UpdateLayout();
            DoEvents();
        }

        private Rectangle AddLocation(bool isDestination=false)
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
            container.Width =10;
            container.Height = 10;
            return container;
        }

        void container_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var src = (Rectangle)e.Source;
            if (src.Tag != null) MessageBox.Show(src.Tag.ToString(),"Location Clicked");
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
    }
}