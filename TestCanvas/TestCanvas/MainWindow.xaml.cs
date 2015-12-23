using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using aXon.Rover;
using aXon.Rover.Enumerations;
using aXon.Rover.Models;
using aXon.TaskTransport;
using aXon.TaskTransport.Messages;
using aXon.Warehouse.Enumerations;
using aXon.Worker;
using Encog.Neural.Networks;
using Encog.Persist;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RabbitMQ.Client;
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
            var pos = e.GetPosition(Canvas);
            var x = (int) pos.X/((int) (Canvas.ActualHeight/SourceData.Warehouse.GridWidth) - 1);
            var y = (int) pos.Y/((int) (Canvas.ActualWidth/SourceData.Warehouse.GridLength) - 1);
            var p = AddLocation(false);
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
            var l =
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
            var col = Mds.DataBase.GetCollection<Rover.Models.Warehouse>("Warehouse");
            SafeModeResult safeModeResult = col.Save(SourceData.Warehouse);
            Canvas.SetLeft(p, x*((int) (Canvas.ActualHeight/SourceData.Warehouse.GridWidth) - 1));
            Canvas.SetTop(p, y*((int) (Canvas.ActualWidth/SourceData.Warehouse.GridLength) - 1));
            Canvas.Children.Add(p);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
           // MoveToPosition(e);
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            //MoveToPosition(e);
        }

        private void MoveToPosition(KeyEventArgs e)
        {
            var pos = AddLocation(false);
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
            RC = new RobotContol(DateTime.Now, Guid.Empty);
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
            //ModeGrid.DataContext = SourceData;
          
            //maptb.DataContext = this;

            BuidMenu();
        }

        private void BuidMenu()
        {
            var list=ReflectiveEnumerator.GetEnumerableOfType<AxonScreen>();
            Screens = new ObservableCollection<AxonScreen>(list);
            List<MenuItem> mods= new List<MenuItem>();
            TreeViewItem root = new TreeViewItem() { Header = "Modules", IsExpanded = true, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
            foreach (var s in list.ToArray())
            {                
                var mod=mods.FirstOrDefault(n => (string)n.Header == s.ModuleName);
                var query = from TreeViewItem childs in root.Items where ((string)childs.Header) == s.ModuleName select childs;
                var tmod = query.FirstOrDefault();
                if(mod==null)
                {
                    mods.Add(new MenuItem() { Header = s.ModuleName, Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0)) });
                }
                if(tmod==null)
                {
                    var mti = new TreeViewItem()
                                  {
                                      Header = s.ModuleName,
                                      Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                                      IsExpanded = true,
                                      IsEnabled=true
                                      
                                  };
                    foreach (var screen in list.ToArray())
                    {
                        screen.DataService = (aXon.Rover.Interfaces.IDataService)Mds;
                        screen.Shell = this;
                        if (screen.ModuleName != s.ModuleName) continue;                        
                        var ti = new TreeViewItem()
                        {
                            Visibility = Visibility.Visible,
                            Header = screen.ScreenName,
                            Tag = screen.ScreenName,
                            Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                            IsExpanded = true,
                            IsEnabled = true
                        };
                        ti.MouseDoubleClick += ti_MouseDoubleClick;
                        mti.Items.Add(ti);
                    }
                    root.Items.Add(mti);
                }
            }
            TreeMenu.Items.Add(root);
            
            foreach (var screen in list.ToArray())
            {
                screen.DataService = (aXon.Rover.Interfaces.IDataService)Mds;
                screen.Shell = this;
                var mod = mods.FirstOrDefault(n => (string)n.Header == screen.ModuleName);
                
                if (mod == null ) continue;               
                var mi = new MenuItem
                {
                    Header = screen.ScreenName,
                    Tag = screen.ScreenName,
                    Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0))
                };
                mi.Click += mi_Click;
                mod.Items.Add(mi);
            }
            foreach (var m in mods)
                Modules.Items.Add(m);           
        }

        void ti_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var src = (TreeViewItem)e.Source;
            LoadScreen((string)src.Tag);
        }

        void mi_Click(object sender, RoutedEventArgs e)
        {
            var src=(MenuItem) e.Source;
            LoadScreen((string)src.Tag);

        }
        public ObservableCollection<AxonScreen> Screens { get; set; }
        public void LoadScreen(string name)
        {
            var screen = Screens.FirstOrDefault(s => s.ScreenName == name);
            TabItem tab =   new TabItem();
            screen.SourceData = SourceData;
            tab.Header = new TabHeader() { HeaderText = name, MyTab = tab, Tabs = Tabs };
            tab.IsSelected = true;
            tab.Content = screen;
            Tabs.Items.Add(tab);
            DoEvents();
        }
        private void SetWindowTitle()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            Title = "aXon Warehouse Desktop Version: " + version;
        }

        private void BuildNetwork(double slat, double slon, double lat, double lon)
        {
            BasicNetwork network = null;
            var nn =
                Mds.GetCollectionQueryModel<NeuralNetwork>(Query.And(Query.EQ("StartPosition.X", slat),
                    Query.EQ("StartPosition.Y", slon), Query.EQ("EndPosition.X", lat), Query.EQ("EndPosition.Y", lon)))
                    .FirstOrDefault();
            var tsk =
                Mds.GetCollectionQueryModel<RoverTask>(
                    Query.And(Query.EQ("TrainingProperties.StartPosition.StartPosition.X", slat),
                        Query.EQ("TrainingProperties.StartPosition.StartPosition.Y", slon),
                        Query.EQ("TrainingProperties.StartPosition.EndPosition.X", lat),
                        Query.EQ("TrainingProperties.StartPosition.EndPosition.Y", lon))).FirstOrDefault();
            if (tsk != null && nn == null) return;
            if (nn == null && tsk == null)
            {
                var t = new TaskMessage
                {
                    LogReportingLevel = LogLevel.Verbatium,
                    MessageId = Guid.NewGuid(),
                    ScriptType = TaskScriptType.LoadWorker,
                    TaskScript = new RoverWorker().GetType().ToString(),
                    TransmisionDateTime = DateTime.Now,
                    TaskId = Guid.NewGuid()
                };
                var col = Mds.DataBase.GetCollection<RoverTask>("RoverTask");
                var task = new RoverTask
                {
                    Status = (TaskTransport.TaskStatus) TaskStatus.Pending,
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
                var x = 0;
                while (x != 10)
                {
                    Thread.Sleep(10);
                    DoEvents();
                    x++;
                }
                q.Publish(t);
                DoEvents();
            }
            else
            {
                var fn = nn.Id.ToString();
                lock (RobotContol.NetworkLock)
                {
                    var rawbytes = Mds.OpenFile(nn.Id);

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
            var p = AddELocation(false);
            p.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            p.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            var w = Canvas.ActualWidth/SourceData.Warehouse.GridWidth;
            var h = Canvas.ActualHeight/SourceData.Warehouse.GridLength;
            Canvas.SetLeft(p, position.X*((int) (Canvas.ActualHeight/SourceData.Warehouse.GridWidth) - 1));
            Canvas.SetTop(p, position.Y*((int) (Canvas.ActualWidth/SourceData.Warehouse.GridLength) - 1));
            Canvas.Children.Add(p);
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
                    var isDest = SourceData.Simulation.Destination[1] == slon &&
                                 SourceData.Simulation.Destination[0] == slat;
                    var location = AddLocation(isDest);

                    location.Tag = slon + "," + slat;
                    double x = (int) (Canvas.ActualHeight/SourceData.Warehouse.GridWidth) - 1;
                    double y = (int) (Canvas.ActualWidth/SourceData.Warehouse.GridLength) - 1;
                   

                    var pos =
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
                                location.Fill = new SolidColorBrush(Color.FromRgb(128, 128, 128));
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
                    TextBlock t = new TextBlock();
                    t.Text = location.Tag.ToString();
                    Canvas.SetLeft(t, (slon * x)+5);
                    Canvas.SetTop(t, (slat * y)+5);
                    Canvas.Children.Add(t);
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

        private Ellipse AddELocation(bool isDestination = false)
        {
            var container = new Ellipse();
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

        public void RunNetworks()
        {
            var reclocs = (from p in SourceData.Warehouse.Positions where p.MapMode == MapMode.PickupMode select p).ToArray();
            var shiplocs = (from p in SourceData.Warehouse.Positions where p.MapMode == MapMode.ShipMode select p).ToArray();
            var storelocs = (from p in SourceData.Warehouse.Positions where p.MapMode == MapMode.StorageMode select p).ToArray();
            var chrlocs = (from p in SourceData.Warehouse.Positions where p.MapMode == MapMode.ChargeMode select p).ToArray();
            var total = ((chrlocs.Length*reclocs.Length)*2) + ((chrlocs.Length*shiplocs.Length)*2) +
                        ((chrlocs.Length*storelocs.Length)*2) + (reclocs.Length*storelocs.Length) + (storelocs.Length*shiplocs.Length);
            Progress.Maximum = total;
            Progress.Value = 0;
            foreach (var cl in chrlocs)
            {
                SourceLocation = new Position(cl.X, cl.Y);
                foreach (var l in reclocs.ToArray())
                {
                    DestLocation = new Position(l.X, l.Y);
                    BuildNetwork(cl.X, cl.Y, l.X, l.Y);
                    Progress.Value += 1;
                    txtProgress.Text = Progress.Value.ToString() + "/" + total.ToString();
                    DoEvents();
                }
                foreach (var l in shiplocs.ToArray())
                {
                    DestLocation = new Position(l.X, l.Y);
                    BuildNetwork(cl.X, cl.Y, l.X, l.Y);
                    Progress.Value += 1;
                    txtProgress.Text = Progress.Value.ToString() + "/" + total.ToString();
                    DoEvents();
                }
                foreach (var l in storelocs.ToArray())
                {
                    DestLocation = new Position(l.X, l.Y);
                    BuildNetwork(cl.X, cl.Y, l.X, l.Y);
                    Progress.Value += 1;
                    txtProgress.Text = Progress.Value.ToString() + "/" + total.ToString();
                    DoEvents();
                }
            }
            foreach (var cl in reclocs)
            {
                SourceLocation = new Position(cl.X, cl.Y);
                foreach (var l in chrlocs.ToArray())
                {
                    DestLocation = new Position(l.X, l.Y);
                    BuildNetwork(cl.X, cl.Y, l.X, l.Y);
                    Progress.Value += 1;
                    txtProgress.Text = Progress.Value.ToString() + "/" + total.ToString();
                    DoEvents();
                }
                foreach (var l in storelocs.ToArray())
                {
                    DestLocation = new Position(l.X, l.Y);
                    BuildNetwork(cl.X, cl.Y, l.X, l.Y);
                    Progress.Value += 1;
                    txtProgress.Text = Progress.Value.ToString() + "/" + total.ToString();
                    DoEvents();
                }
            }
            foreach (var cl in storelocs)
            {
                SourceLocation = new Position(cl.X, cl.Y);
                foreach (var l in chrlocs.ToArray())
                {
                    DestLocation = new Position(l.X, l.Y);
                    BuildNetwork(cl.X, cl.Y, l.X, l.Y);
                    Progress.Value += 1;
                    txtProgress.Text = Progress.Value.ToString() + "/" + total.ToString();
                    DoEvents();
                }
                foreach (var l in shiplocs.ToArray())
                {
                    DestLocation = new Position(l.X, l.Y);
                    BuildNetwork(cl.X, cl.Y, l.X, l.Y);
                    Progress.Value += 1;
                    txtProgress.Text = Progress.Value.ToString() + "/" + total.ToString();
                    DoEvents();
                }
            }
        }

        private void CommitWarehouse(object sender, RoutedEventArgs e)
        {
            SourceData.Warehouse.GridLength = SourceData.Warehouse.Length/4;
            SourceData.Warehouse.GridWidth = SourceData.Warehouse.Width/4;
            var col = Mds.DataBase.GetCollection<Rover.Models.Warehouse>("Warehouse");
            SafeModeResult safeModeResult = col.Save(SourceData.Warehouse);

            MessageBox.Show("Success!");
            DrawMap();
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            switch ((string)((MenuItem) e.Source).Tag)
            {
                case "Exit":
                    Application.Current.Shutdown(0);
                    break;
                    
            }
        }

        private void Networks_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void RefreshNetworks(object sender, RoutedEventArgs e)
        {
           
            
        }

        public void RunNetwork(Guid id)
        {
            MapTab.IsSelected = true;
            DoEvents();
            BasicNetwork network = null;
            
            var fn = id.ToString();
            var net = Mds.GetCollectionQueryModel<NeuralNetwork>(Query.EQ("_id",id)).FirstOrDefault();
            if (net != null)
            {
                SourceLocation = new Position(net.StartPosition.X,net.StartPosition.Y);
                DestLocation = new Position(net.EndPosition.X,net.EndPosition.Y);

                lock (RobotContol.NetworkLock)
                {
                    var rawbytes = Mds.OpenFile(id);

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
           
            DoEvents();
        }

        private void RefreshRovers(object sender, RoutedEventArgs e)
        {

        }

        private void Rovers_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AddRover(object sender, RoutedEventArgs e)
        {

        }

        private void EditRover(object sender, RoutedEventArgs e)
        {

        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void Maximize(object sender, RoutedEventArgs e)
        {
            this.WindowState=WindowState.Maximized;
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void tabchanges(object sender, SelectionChangedEventArgs e)
        {
            var showtoolbar = false;
            foreach(TabItem t in Tabs.Items)
            {
                try
                {
                    if ((string) t.Header == "Warehouse Map" && t.IsSelected)
                        showtoolbar = true;
                }
                catch
                {
                }
            }
            if (showtoolbar)
            {
                mainToolBar.Visibility=Visibility.Visible;
                mainToolBar.Height = 50;
                toolbarRow.Height = new GridLength(50);
            }
            else
            {
                mainToolBar.Visibility = Visibility.Collapsed;
                mainToolBar.Height = 0;
                toolbarRow.Height = new GridLength(0);
            }

        }

        private void About_Click(object sender, RoutedEventArgs e)
        {

        }
        
    }
}