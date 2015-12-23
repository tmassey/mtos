using System.Timers;
using System.Windows.Threading;
using FirstFloor.ModernUI.Windows.Controls;
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

namespace aXon.Desktop
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : ModernWindow
    {
        private Timer _t = new Timer(50);
        public Splash()
        {
            InitializeComponent();
            _t.Elapsed += _t_Elapsed;
            _t.Start();
        }

        void _t_Elapsed(object sender, ElapsedEventArgs e)
        {
            _t.Stop();
            _t.Enabled = false;
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                   new Action(() => Load()));
        }

        private MainWindow _main;
        void Load()
        {
            Hide();
            Login l = new Login();
            l.ShowDialog();
            if (l.DialogResult.Value)
            {
                _main = new MainWindow();
                _main.BuidMenu();
                _main.Show();
                _main.Closed += _main_Closed;
            }
            else
            {
                Close();
            }
        }

        void _main_Closed(object sender, EventArgs e)
        {
            Close();
        }
    }
}
