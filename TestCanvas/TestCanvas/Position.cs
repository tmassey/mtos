using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using aXon.Rover.Annotations;

namespace TestCanvas
{
    /// <summary>
    ///     This class represents a geographical position. It
    ///     has a latitude and longitude, but no meta data.
    /// </summary>
    /// <remarks>
    ///     Author:     Daniel Saidi [daniel.saidi@gmail.com]
    ///     Link:       http://danielsaidi.github.com/nextra
    /// </remarks>
    public class Position : IPosition,INotifyPropertyChanged
    {
        private double _latitude;
        private double _longitude;
        private Guid _id;
        

        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }


        public double X
        {
            get { return _latitude; }
             set
            {
                if (value.Equals(_latitude)) return;
                _latitude = value;
                OnPropertyChanged();
            }
        }

        public double Y
        {
            get { return _longitude; }
             set
            {
                if (value.Equals(_longitude)) return;
                _longitude = value;
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