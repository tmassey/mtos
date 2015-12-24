using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MongoDB.Driver;
using aXon.Rover.Enumerations;
using aXon.Rover.Models;

namespace aXon.Desktop.Pages.Modules.Warehouse
{
    /// <summary>
    /// Interaction logic for WarehouseMap.xaml
    /// </summary>
    public partial class WarehouseMap : AxonScreen
    {
        private DataSource _sourceData;

        public WarehouseMap()
        {
            InitializeComponent();
            ScreenName = "Warehouse Map";
            ModuleName = "Inventory";
            SourcePath = new Uri("/Pages/Modules/Warehouse/WarehouseMap.xaml", UriKind.Relative);
            Loaded += WarehouseMap_Loaded;
        }

        void WarehouseMap_Loaded(object sender, RoutedEventArgs e)
        {
            DrawMap();
        }

        public DataSource SourceData
        {
            get { return _sourceData; }
            set
            {
                if (Equals(value, _sourceData)) return;
                _sourceData = value;
                OnPropertyChanged("SourceData");                
            }
        }

        private void DrawMap()
        {
            Canvas.Children.Clear();
            for (double slat = 0; slat <= SourceData.Warehouse.GridLength; slat++)
            {
                for (double slon = 0; slon <= SourceData.Warehouse.GridWidth; slon++)
                {
                    var isDest = SourceData.Simulation.Destination[1] == slon &&
                                 SourceData.Simulation.Destination[0] == slat;
                    var location = AddLocation(isDest);

                    location.Tag = slon + "," + slat;
                    double x = (int)(Canvas.ActualHeight / SourceData.Warehouse.GridWidth) - 1;
                    double y = (int)(Canvas.ActualWidth / SourceData.Warehouse.GridLength) - 1;

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

                    Canvas.SetLeft(location, slon * x);
                    Canvas.SetTop(location, slat * y);
                    Canvas.Children.Add(location);
                    TextBlock t = new TextBlock {Text = location.Tag.ToString()};
                    Canvas.SetLeft(t, (slon * x) + 5);
                    Canvas.SetTop(t, (slat * y) + 5);
                    Canvas.Children.Add(t);
                }
            }
            Canvas.UpdateLayout();
            DoEvents();
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
            container.Width = Canvas.ActualWidth / SourceData.Warehouse.GridWidth;
            container.Height = Canvas.ActualHeight / SourceData.Warehouse.GridLength;
            return container;
        }
    }
}
