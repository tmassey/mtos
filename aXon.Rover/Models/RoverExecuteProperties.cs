using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using aXon.Rover.Annotations;

namespace aXon.Rover.Models
{
    public class RoverExecuteProperties : INotifyPropertyChanged
    {
        private Guid _id;
        private Position _destinationPosition;
        private Position _startPosition;
        private WarehouseRover _rover;

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
        public WarehouseRover Rover
        {
            get { return _rover; }
            set
            {
                if (Equals(value, _rover)) return;
                _rover = value;
                OnPropertyChanged();
            }
        }


        public Position DestinationPosition
        {
            get { return _destinationPosition; }
            set
            {
                if (Equals(value, _destinationPosition)) return;
                _destinationPosition = value;
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