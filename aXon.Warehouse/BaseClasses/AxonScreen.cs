using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Threading;
using aXon.Rover.Interfaces;
using aXon.Warehouse.Attributes;
using aXon.Warehouse.Enumerations;

namespace aXon.Warehouse.BaseClasses
{
    public abstract class AxonScreen : UserControl,  IAxonScreen,INotifyPropertyChanged
    {
        private Guid _id;
        private IMainWindow _shell;
        private IDataService _dataService;
        private string _screenName;
        private string _moduleName;


        public Guid Id
        {
            get { return _id; }
            set
            {
                if (value.Equals(_id)) return;
                _id = value;
                OnPropertyChanged();
            }
        }

        public string ScreenName
        {
            get { return _screenName; }
            set
            {
                if (value == _screenName) return;
                _screenName = value;
                OnPropertyChanged();
            }
        }

        public string ModuleName
        {
            get { return _moduleName; }
            set
            {
                if (value == _moduleName) return;
                _moduleName = value;
                OnPropertyChanged();
            }
        }

        public IDataService DataService
        {
            get { return _dataService; }
            set
            {
                if (Equals(value, _dataService)) return;
                _dataService = value;
                OnPropertyChanged();
            }
        }

        public IMainWindow Shell
        {
            get { return _shell; }
            set
            {
                if (Equals(value, _shell)) return;
                _shell = value;
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
        public event PropertyChangedEventHandler PropertyChanged;

        [Annotations.NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

       
    }


}