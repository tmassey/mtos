using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using aXon.Rover.Annotations;
using aXon.Rover.Enumerations;

namespace aXon.Rover.Models
{
    public class WarehouseRover : INotifyPropertyChanged
    {
        private RoverDirection _direction;
        private double _currentHeading;
        private double _fuelRemaining;
        private RoverMode _currentMode;
        private Position _currentPosition;
        private string _serialNumber;
        private Guid _id;

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

        public string SerialNumber
        {
            get { return _serialNumber; }
            set
            {
                if (value == _serialNumber) return;
                _serialNumber = value;
                OnPropertyChanged();
            }
        }

        public Position CurrentPosition
        {
            get { return _currentPosition; }
            set
            {
                if (Equals(value, _currentPosition)) return;
                _currentPosition = value;
                OnPropertyChanged();
            }
        }

        public RoverMode CurrentMode
        {
            get { return _currentMode; }
            set
            {
                if (value == _currentMode) return;
                _currentMode = value;
                OnPropertyChanged();
            }
        }

        public double FuelRemaining
        {
            get { return _fuelRemaining; }
            set
            {
                if (value.Equals(_fuelRemaining)) return;
                _fuelRemaining = value;
                OnPropertyChanged();
            }
        }

        public double CurrentHeading
        {
            get { return _currentHeading; }
            set
            {
                if (value.Equals(_currentHeading)) return;
                _currentHeading = value;
                OnPropertyChanged();
            }
        }

        public RoverDirection Direction
        {
            get { return _direction; }
            set
            {
                if (value == _direction) return;
                _direction = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}