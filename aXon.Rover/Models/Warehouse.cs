using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using aXon.Rover.Annotations;

namespace aXon.Rover.Models
{
    public class Warehouse: INotifyPropertyChanged
    {
        private Guid _id;
        private string _name;
        private int _gridLength;
        private int _gridWidth;
        private int _width;
        private int _length;
        public event PropertyChangedEventHandler PropertyChanged;
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

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public int Length
        {
            get { return _length; }
            set
            {
                if (value == _length) return;
                _length = value;
                OnPropertyChanged();
            }
        }

        public int Width
        {
            get { return _width; }
            set
            {
                if (value == _width) return;
                _width = value;
                OnPropertyChanged();
            }
        }

        public int GridWidth
        {
            get { return _gridWidth; }
            set
            {
                if (value == _gridWidth) return;
                _gridWidth = value;
                OnPropertyChanged();
            }
        }

        public int GridLength
        {
            get { return _gridLength; }
            set
            {
                if (value == _gridLength) return;
                _gridLength = value;
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