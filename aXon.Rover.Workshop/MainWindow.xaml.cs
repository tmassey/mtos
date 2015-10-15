using System;
using System.Collections.Generic;
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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Map;

namespace aXon.Rover.Workshop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ThemeManager.SetTheme(this, new Theme("MetropolisDark"));
            InitializeComponent();
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine(Map.CenterPoint.ToString());
            GeoPoint geoPt = Map.Layers[0].ScreenToGeoPoint(e.GetPosition(Map));
            //LastNavSelection = geoPt;
            Map.CenterPoint = geoPt;
            Map.DataContext = this;            
        }

        private void Geolayer_OnChanged(object sender, EventArgs e)
        {

        }
    }
}
