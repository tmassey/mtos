using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using aXon.Rover.Annotations;

namespace aXon.Warehouse.Desktop
{
    /// <summary>
    /// Interaction logic for TabHeader.xaml
    /// </summary>
    public partial class TabHeader : UserControl, INotifyPropertyChanged
    {
        private string _headerText;

        public TabHeader()
        {
            InitializeComponent();
            Loaded += TabHeader_Loaded;
        }

        void TabHeader_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
        }
        public TabItem MyTab { get; set; }
        public TabControl Tabs { get; set; }
        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                if (value == _headerText) return;
                _headerText = value;
                OnPropertyChanged();
            }
        }

        private void CloseTab(object sender, RoutedEventArgs e)
        {
           Tabs.Items.Remove(MyTab);            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
