using System.ComponentModel;
using System.Runtime.CompilerServices;
using aXon.Rover.Annotations;
using aXon.Rover.Enumerations;
using aXon.Rover.Models;

namespace TestCanvas
{
    public class DataSource: INotifyPropertyChanged
    {
        private RobotSimulator _simulation;
        private Warehouse _warehouse;
       
        private MapMode _mapMode;
     
        public event PropertyChangedEventHandler PropertyChanged;
        public RobotSimulator Simulation
        {
            get { return _simulation; }
            set
            {
                if (Equals(value, _simulation)) return;
                _simulation = value;
                OnPropertyChanged();
            }
        }

        public Warehouse Warehouse
        {
            get { return _warehouse; }
            set
            {
                if (Equals(value, _warehouse)) return;
                _warehouse = value;
                OnPropertyChanged();
            }
        }

        
        public MapMode ModeMap
        {
            get { return _mapMode; }
            set
            {
                if (value == _mapMode) return;
                _mapMode = value;
                OnPropertyChanged();
            }
        }

       

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}