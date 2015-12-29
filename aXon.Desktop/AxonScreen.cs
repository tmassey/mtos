using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Threading;
using aXon.Data;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Navigation;
using aXon.Rover.Annotations;
using aXon.Rover.Interfaces;

namespace aXon.Desktop
{
    public abstract class AxonScreen : UserControl, IAxonScreen, IContent
    {
        private Guid _id;
        private MainWindow _shell;
        private aXonEntities _dataService;
        private string _screenName;
        private string _moduleName;
        private Uri _sourcePath;


        public Guid Id
        {
            get { return _id; }
            set
            {
                if (value.Equals(_id)) return;
                _id = value;
                OnPropertyChanged("Id");
                
            }
        }

        public Uri SourcePath
        {
            get { return _sourcePath; }
            set
            {
                if (Equals(value, _sourcePath)) return;
                _sourcePath = value;
                OnPropertyChanged("SourcePath");
            }
        }

        public string ScreenName
        {
            get { return _screenName; }
            set
            {
                if (value == _screenName) return;
                _screenName = value;
                OnPropertyChanged("ScreenName");                
            }
        }

        public string ModuleName
        {
            get { return _moduleName; }
            set
            {
                if (value == _moduleName) return;
                if (value == _moduleName) return;
                _moduleName = value;
                OnPropertyChanged("ModuleName");
            }
        }

        public aXonEntities DataService
        {
            get { return _dataService; }
            set
            {
                if (Equals(value, _dataService)) return;
                if (Equals(value, _dataService)) return;
                _dataService = value;
                OnPropertyChanged("DataService");
            }
        }

        public MainWindow Shell
        {
            get { return _shell; }
            set
            {
                if (Equals(value, _shell)) return;
                _shell = value;
                OnPropertyChanged("Shell");
                
            }
        }

        public virtual DataSource SourceData { get; set; }

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


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnFragmentNavigation(FragmentNavigationEventArgs e)
        {
           // throw new NotImplementedException();
        }

        public void OnNavigatedFrom(NavigationEventArgs e)
        {
           // throw new NotImplementedException();
        }

        public void OnNavigatedTo(NavigationEventArgs e)
        {
          //  throw new NotImplementedException();
        }

        public void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }


}