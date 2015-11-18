using System.ComponentModel;
using System.Runtime.CompilerServices;
using aXon.Rover;
using aXon.Rover.Annotations;
using aXon.Rover.Enumerations;

namespace aXon.Warehouse.Desktop
{
    public class DataSource : INotifyPropertyChanged
    {
        private MapMode _mapMode;
        private RobotSimulator _simulation;
        private Rover.Models.Warehouse _warehouse;

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

        public Rover.Models.Warehouse Warehouse
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

        public event PropertyChangedEventHandler PropertyChanged;


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}