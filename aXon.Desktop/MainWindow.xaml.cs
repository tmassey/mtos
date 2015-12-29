using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Threading;
using FirstFloor.ModernUI.Presentation;
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
using aXon.Data;
using RabbitMQ.Client;
using aXon.Rover;


namespace aXon.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            //BuidMenu();
            Loaded += MainWindow_Loaded;
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
        public aXonEntities Mds { get; set; }
        private static IConnection _Connection;
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
        public DataSource SourceData { get; set; }
        public ToastinetWPF.Toastinet Toast { get; set; }
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = "aXon OS (Code Named: Spark) Version: " + Assembly.GetEntryAssembly().GetName().Version.ToString();
            WarehouseName.DisplayName="Company: " + Globals.CurrentUser.Company.Name;
            Toast= new ToastinetWPF.Toastinet();
            Globals.Toast = Toast;
        }

        public ObservableCollection<AxonScreen> Screens { get; set; }
        public void BuidMenu()
        {
            var list = ReflectiveEnumerator.GetEnumerableOfType<AxonScreen>();
            Screens = new ObservableCollection<AxonScreen>(list);
                      LinkGroupCollection col = new LinkGroupCollection();
            foreach (LinkGroup linkGroup in MenuLinkGroups)
            {
                var screens = (from s in list where s.ModuleName == linkGroup.DisplayName select s) ;
                foreach (AxonScreen screen in screens)
                {
                    var mi = new Link
                    {
                        DisplayName = screen.ScreenName,
                        Source = screen.SourcePath
                    };
                    linkGroup.Links.Add(mi);
                }
            }
           //foreach (var s in list.ToArray())
           // {
           //     var mod = MenuLinkGroups.FirstOrDefault(n => (string)n.DisplayName == s.ModuleName);
           //     if (mod == null )
           //     {                    
           //         foreach (var screen in list.ToArray())
           //         {
           //             if (screen.ModuleName != s.ModuleName) continue;
           //             screen.DataService = Mds;
           //             screen.Shell = this;
           //             var mi = new Link
           //             {
           //                 DisplayName = screen.ScreenName,
           //                 Source=screen.SourcePath
           //             };                        
           //             mod.Links.Add(mi);
           //         }                    
           //     }           
           // }
            
            this.UpdateLayout();
            
        }
    }
}
