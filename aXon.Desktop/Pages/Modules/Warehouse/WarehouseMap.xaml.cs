using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using aXon.Rover;
using aXon.Rover.Annotations;
using MongoDB.Driver;
using aXon.Rover.Enumerations;
using aXon.Rover.Interfaces;
using aXon.Rover.Models;
using Encog.Neural.Networks;
using Encog.Persist;
using MongoDB.Driver.Builders;
using Path = System.Windows.Shapes.Path;

namespace aXon.Desktop.Pages.Modules.Warehouse
{
    /// <summary>
    /// Interaction logic for WarehouseMap.xaml
    /// </summary>
    public partial class WarehouseMap : UserControl,INotifyPropertyChanged
    {
        private DataSource _sourceData;
        private IDataService _dataService;

        public WarehouseMap()
        {
            InitializeComponent();
           
            Loaded += WarehouseMap_Loaded;
            SizeChanged += WarehouseMap_SizeChanged;
        }

        private void WarehouseMap_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawMap();
        }

        void WarehouseMap_Loaded(object sender, RoutedEventArgs e)
        {
            Canvas.DataContext = this;
            Viewbox.DataContext = this;
            RobotContol.NetworkLock = "lock";
            RobotContol.ConsoleLock = "lock";
            SourceData = new DataSource();
            DataService = new MongoDataService();
            SourceData.Warehouse = DataService.GetCollectionQueryModel<Rover.Models.Warehouse>().FirstOrDefault();
            RobotContol.Warehouse = SourceData.Warehouse;        
            
        }

        public IDataService DataService
        {
            get { return _dataService; }
            set
            {
                if (Equals(value, _dataService)) return;
                if (Equals(value, _dataService)) return;
                _dataService = value;
                OnPropertyChanged();
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
            ((DispatcherFrame)f).Continue = false;

            return null;
        }
        public DataSource SourceData
        {
            get { return _sourceData; }
            set
            {
                if (Equals(value, _sourceData)) return;
                _sourceData = value;
                OnPropertyChanged();                
            }
        }

        public void DrawMap()
        {
            if (SourceData != null && SourceData.Warehouse != null)
            {
                Canvas.Children.Clear();
                for (double slat = 0; slat <= SourceData.Warehouse.GridLength; slat++)
                {
                    for (double slon = 0; slon <= SourceData.Warehouse.GridWidth; slon++)
                    {
                        //var isDest = SourceData.Simulation.Destination[1] == slon &&
                        //             SourceData.Simulation.Destination[0] == slat;
                        var location = AddLocation(false);

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
                                    location.Fill = new SolidColorBrush(Color.FromRgb(100, 100, 100));
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
                        TextBlock t = new TextBlock {Text = location.Tag.ToString()};
                        Canvas.SetLeft(t, (slon*x) + 5);
                        Canvas.SetTop(t, (slat*y) + 5);
                        Canvas.Children.Add(t);
                    }
                }
                Canvas.UpdateLayout();
                DoEvents();
            }
        }
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(Canvas);
            var x = (int)pos.X / ((int)(Canvas.ActualHeight / SourceData.Warehouse.GridWidth) - 1);
            var y = (int)pos.Y / ((int)(Canvas.ActualWidth / SourceData.Warehouse.GridLength) - 1);
            var p = AddLocation(false);
            switch (SourceData.ModeMap)
            {
                case MapMode.ChargeMode:
                    p.Fill = new SolidColorBrush(Color.FromRgb(200, 0, 0));
                    break;
                case MapMode.ObstructionMode:
                    p.Fill = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                    break;
                case MapMode.PersonMode:
                    p.Fill = new SolidColorBrush(Color.FromRgb(128, 100, 50));
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
                    p.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 200));
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
                l = new Position(x, y) { MapMode = SourceData.ModeMap };
                SourceData.Warehouse.Positions.Add(l);
            }
            var col = DataService.DataBase.GetCollection<Rover.Models.Warehouse>("Warehouse");
            SafeModeResult safeModeResult = col.Save(SourceData.Warehouse);
            Canvas.SetLeft(p, x * ((int)(Canvas.ActualHeight / SourceData.Warehouse.GridWidth) - 1));
            Canvas.SetTop(p, y * ((int)(Canvas.ActualWidth / SourceData.Warehouse.GridLength) - 1));
            Canvas.Children.Add(p);
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
            container.Width = Canvas.ActualWidth / SourceData.Warehouse.GridWidth;
            container.Height = Canvas.ActualHeight / SourceData.Warehouse.GridLength;
            return container;
        }
        private void container_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var src = (Rectangle)e.Source;
            if (src.Tag != null) MessageBox.Show(src.Tag.ToString(), "Location Clicked");
        }
        private Path AddELocation(bool isDestination = false)
        {
            var container = new Path();
      
            container.Data = Geometry.Parse(Application.Current.Resources["RobotString"].ToString());            
            if (!isDestination)
            {
                container.Stroke = new SolidColorBrush(Color.FromRgb(200, 200, 200));
                container.StrokeThickness = 2;
            }
            else
            {
                container.Stroke = new SolidColorBrush(Color.FromRgb(255, 128, 128));
                container.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                container.StrokeThickness = 2;
            }

            container.MouseRightButtonUp += container_MouseRightButtonUp;
            container.Width = (Canvas.ActualWidth / SourceData.Warehouse.GridWidth);
            container.Height = (Canvas.ActualHeight / SourceData.Warehouse.GridLength);
            return container;
        }
        public  Position SourceLocation { get; set; }
        public  Position DestLocation { get; set; }
        private void Simulation_PositionChanged(object sender, Position position)
        {
            var p = AddELocation(false);
            p.Fill = new SolidColorBrush(Color.FromRgb(200, 200, 200));
            p.Stroke =new SolidColorBrush(Color.FromRgb(200, 200, 200));
            var w = Canvas.ActualWidth / SourceData.Warehouse.GridWidth;
            var h = Canvas.ActualHeight / SourceData.Warehouse.GridLength;
            Canvas.SetLeft(p, position.X * ((int)(Canvas.ActualHeight / SourceData.Warehouse.GridWidth) - 1));
            Canvas.SetTop(p, position.Y * ((int)(Canvas.ActualWidth / SourceData.Warehouse.GridLength) - 1));
            Canvas.Children.Add(p);
            Canvas.UpdateLayout();
            DoEvents();
        }
        public void RunNetwork(Guid id)
        {
           
            BasicNetwork network = null;

            var fn = id.ToString();
            var net = DataService.GetCollectionQueryModel<NeuralNetwork>(Query.EQ("_id", id)).FirstOrDefault();
            if (net != null)
            {
                SourceLocation = new Position(net.StartPosition.X, net.StartPosition.Y);
                DestLocation = new Position(net.EndPosition.X, net.EndPosition.Y);

                lock (RobotContol.NetworkLock)
                {
                    var rawbytes = DataService.OpenFile(id);

                    File.WriteAllBytes(fn, rawbytes);
                    network = (BasicNetwork)EncogDirectoryPersistence.LoadObject(new FileInfo(fn));
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
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
