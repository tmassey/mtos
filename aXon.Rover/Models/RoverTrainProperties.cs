using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using aXon.Rover.Annotations;

namespace aXon.Rover.Models
{
    public class RoverTrainProperties : INotifyPropertyChanged
    {
        private Guid _id;
        private Position _destinationPosition;
        private Position _startPosition;

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

        public Position StartPosition
        {
            get { return _startPosition; }
            set
            {
                if (Equals(value, _startPosition)) return;
                _startPosition = value;
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