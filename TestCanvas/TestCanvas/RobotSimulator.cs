using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Encog.Util;
using TestCanvas.Annotations;

namespace TestCanvas
{
    public class RobotSimulator : INotifyPropertyChanged
    {
        private static readonly Random _random = new Random();
        public ConsoleColor Color = GetRandomConsoleColor();
        private double[] _position;
        private double[] _destination;
        private double[] _startPosition;
        private double _startDistance;
        private double _fuel;
        private int _seconds;
        private double _altitude;
        private double _turns;
        private double _rests;
        private double _success;
        private double _lastDistance;
        private double _previousDistance;
        private double _distanceToDestination;
        private double _heading;
        private RobotDirection _currentDirection;
        public event OnPositionChanged PositionChanged;

        protected virtual void OnPositionChanged1(Position args)
        {
            OnPositionChanged handler = PositionChanged;
            if (handler != null) handler(this, args);
        }

        public RobotSimulator(Position source, Position destination)
        {
            Success = -10000;
            Fuel = 9000;
            Seconds = 0;
            Altitude = 100000;
            Rests = 0;
            Turns = 0;
            Position = new double[2] {source.X, source.Y};
            Destination = new double[2] {destination.X, destination.Y};
            StartPosition = new double[2] {source.X, source.Y};
            CurrentDirection = RobotDirection.Forward;
            DistanceToDestination = CalculateDistance();
            LastDistance = DistanceToDestination;
            PreviousDistance = LastDistance;
            StartDistance = DistanceToDestination;
            UpdateHeading();
            
        }

        public double[] Position
        {
            get { return _position; }
            set
            {
                if (Equals(value, _position)) return;
                _position = value;
                OnPropertyChanged();
                OnPropertyChanged("LeftDistance");
                OnPropertyChanged("FrontDistance");
                OnPropertyChanged("RightDistance");
                OnPropertyChanged("BackDistance");
            }
        }

        public double[] Destination
        {
            get { return _destination; }
            set
            {
                if (Equals(value, _destination)) return;
                _destination = value;
                OnPropertyChanged();
                OnPropertyChanged("Score");
                OnPropertyChanged("Traveling");
            }
        }

        public double[] StartPosition
        {
            get { return _startPosition; }
            set
            {
                if (Equals(value, _startPosition)) return;
                _startPosition = value;
                OnPropertyChanged();
                OnPropertyChanged("Score");
                OnPropertyChanged("Traveling");
            }
        }

        public double StartDistance
        {
            get { return _startDistance; }
            set { _startDistance = value; }
        }

        public double Fuel
        {
            get { return _fuel; }
            set
            {
                if (value.Equals(_fuel)) return;
                _fuel = value;
                OnPropertyChanged();
                OnPropertyChanged("Score");
                OnPropertyChanged("ShoudRest");
                OnPropertyChanged("Traveling");
            }
        }

        public int Seconds
        {
            get { return _seconds; }
            set
            {
                if (value == _seconds) return;
                _seconds = value;
                OnPropertyChanged();
                OnPropertyChanged("Score");
                OnPropertyChanged("Traveling");
            }
        }

        public double Altitude
        {
            get { return _altitude; }
            set
            {
                if (value.Equals(_altitude)) return;
                _altitude = value;
                OnPropertyChanged();
            }
        }

        public double Turns
        {
            get { return _turns; }
            set
            {
                if (value.Equals(_turns)) return;
                _turns = value;
                OnPropertyChanged();
            }
        }

        public double Rests
        {
            get { return _rests; }
            set
            {
                if (value.Equals(_rests)) return;
                _rests = value;
                OnPropertyChanged();
                OnPropertyChanged("Score");
            }
        }

        public RobotDirection CurrentDirection
        {
            get { return _currentDirection; }
            set
            {
                if (value == _currentDirection) return;
                _currentDirection = value;
                OnPropertyChanged();
            }
        }

        public int Score
        {
            get
            {
                double startdist = CalculateDistance(StartPosition, Destination);
                double lastdist = CalculateDistance();
                double closeness = startdist - lastdist;
                double traveled = startdist - closeness;

                double perctraverced = (startdist/traveled);
                if (Success > 0)
                    perctraverced = 10;
                return (int) ((Fuel*10) + Success + perctraverced + (Rests*-100) + (Seconds*-1));
            }
        }

        public double ShoudRest
        {
            get { return Fuel/200; }
        }

        public bool Traveling
        {
            get
            {
                if (DistanceToDestination == 0)
                {
                    //lock (RobotContol.ConsoleLock)
                    //{
                    //    Console.ForegroundColor = Color;
                    //    Console.SetCursorPosition((int) Destination[1], (int) Destination[0]);
                    //    Console.Write("W");
                    //}
                    Success = 1000;
                    return false;
                }
                double startdist = CalculateDistance(StartPosition, Destination);
                double lastdist = CalculateDistance();
                double perdist = 100000/startdist;
                double closeness = lastdist*perdist;
                
                if (PreviousDistance < DistanceToDestination)
                {
                    Seconds *= 100;
                    Success = closeness*-1;                    
                    return false;
                }
                else
                {
                    PreviousDistance = LastDistance;
                    LastDistance = DistanceToDestination;
                }
               
                if (Seconds >= 2000)
                {
                    Seconds *= 100;
                    Success = closeness*-1;                
                    return false;
                }
                if (Fuel == 0 && Seconds >= 2000 && DistanceToDestination > 0)
                {
                    Seconds *= 100;
                    Success = closeness*-1;                    
                    return false;
                }
                return true;
            }
        }

        public double Success
        {
            get { return _success; }
            set
            {
                if (value.Equals(_success)) return;
                _success = value;
                OnPropertyChanged();
                OnPropertyChanged("Score");
            }
        }

        public double LastDistance
        {
            get { return _lastDistance; }
            set
            {
                if (value.Equals(_lastDistance)) return;
                _lastDistance = value;
                OnPropertyChanged();
                OnPropertyChanged("Traveling");
            }
        }

        public double PreviousDistance
        {
            get { return _previousDistance; }
            set
            {
                if (value.Equals(_previousDistance)) return;
                _previousDistance = value;
                OnPropertyChanged();
                OnPropertyChanged("Traveling");
            }
        }

        public double DistanceToDestination
        {
            get { return _distanceToDestination; }
            set
            {
                if (value.Equals(_distanceToDestination)) return;
                _distanceToDestination = value;
                OnPropertyChanged();
                OnPropertyChanged("Traveling");
            }
        }

        public double LeftDistance
        {
            get
            {
                double col = Position[1];

                return col;
            }
        }

        public double FrontDistance
        {
            get
            {
                double row = Position[0];
                double col = Position[1];
                return row;
            }
        }

        public double RightDistance
        {
            get
            {
                double row = Position[0];
                double col = Position[1];
                return col;
            }
        }

        public double BackDistance
        {
            get
            {
                double row = Position[0];
                return -row;
            }
        }

        public double Heading
        {
            get { return _heading; }
            set
            {
                if (value.Equals(_heading)) return;
                _heading = value;
                OnPropertyChanged();
            }
        }

        private static ConsoleColor GetRandomConsoleColor()
        {
            Array consoleColors = Enum.GetValues(typeof (ConsoleColor));
            var clr = (ConsoleColor) consoleColors.GetValue(_random.Next(consoleColors.Length));
            if (clr == ConsoleColor.Black)
                clr = GetRandomConsoleColor();
            return clr;
        }

        private double CalculateDistance()
        {
            double xdist = Position[0] - Destination[0];
            double ydist = Position[1] - Destination[1];
            if (xdist < 0)
                xdist = xdist*-1;
            if (ydist < 0)
                ydist = ydist*-1;

            return xdist + ydist;
        }

        private double CalculateDistance(double[] source, double[] dest)
        {
            double xdist = source[0] - dest[0];
            double ydist = source[1] - dest[1];
            if (xdist < 0)
                xdist = xdist*-1;
            if (ydist < 0)
                ydist = ydist*-1;

            return xdist + ydist;
        }

        public void Turn(RobotDirection newDirection)
        {
            CurrentDirection = newDirection;
            Seconds++;
            //if (newDirection == RobotDirection.Rest)
            //{
            //    Rests++;
            //    Fuel = 200;
            //    return;
            //}
            if (Fuel > 0)
            {
                Fuel -= 1;
            }


            if (!CanGoInDirection(newDirection))
            {                
                return;
            }            
            MoveToNewPostion(newDirection);
            OnPositionChanged1(new Position(Position[0],Position[1]));
            DistanceToDestination = CalculateDistance();

            if (Altitude < 0)
                Altitude = 0;
         
        }

        private void MoveToNewPostion(RobotDirection newDirection)
        {
            switch (newDirection)
            {
                case RobotDirection.Forward:
                    Position[0] = Position[0] - 1;
                    break;
                case RobotDirection.Left:
                    Position[1] = Position[1] - 1;

                    break;
                case RobotDirection.Right:
                    Position[1] = Position[1] + 1;
                    break;
                case RobotDirection.Reverse:
                    Position[0] = Position[0] + 1;
                    break;
                //case RobotDirection.FowardLeft:
                //    if (Position[0] - 1 >= 0 && Position[1] - 1 >= 0)
                //    {
                //        Position[0] = Position[0] - 1;
                //        Position[1] = Position[1] - 1;
                //    }
                //    break;
                //case RobotDirection.FowardRight:
                //    if (Position[0] - 1 >= 0 && Position[1] + 1 >= 0)
                //    {
                //        Position[0] = Position[0] - 1;
                //        Position[1] = Position[1] + 1;
                //    }
                //    break;
                //case RobotDirection.BackLeft:
                //    if (Position[0] + 1 >= 0 && Position[1] - 1 >= 0)
                //    {
                //        Position[0] = Position[0] + 1;
                //        Position[1] = Position[1] - 1;
                //    }
                //    break;
                //case RobotDirection.BackRight:
                //    if (Position[0] + 1 >= 0 && Position[1] + 1 >= 0)
                //    {
                //        Position[0] = Position[0] + 1;
                //        Position[1] = Position[1] + 1;
                //    }
                //    break;
            }
            if (Position[0] < 0)
                Position[0] = 0;
            if (Position[1] < 0)
                Position[1] = 0;

            Position = new double[2]{Position[0],Position[1]};
            UpdateHeading();
        }

        private void UpdateHeading()
        {
            var src = new Position(Position[0], Position[1]);
            var dest = new Position(Destination[0], Destination[1]);
            var calc = new PositionBearingCalculator(new AngleConverter());
            Heading = calc.CalculateBearing(src, dest);
        }

       

        public bool CanGoInDirection(RobotDirection newDirection)
        {
            if (Fuel == 0) return false;
            return true;
        }

        public String Telemetry()
        {
            //lock (RobotContol.ConsoleLock)
            //{
            //    Console.SetCursorPosition(90, 3);
            //    Console.Write(@"                                            ");
            //    Console.SetCursorPosition(90, 3);
            //    Console.Write(@"Time: " + Seconds);
            //    Console.SetCursorPosition(90, 4);
            //    Console.Write(@"                                            ");
            //    Console.SetCursorPosition(90, 4);
            //    Console.Write(@"Fuel: " + Fuel);
            //    Console.SetCursorPosition(90, 5);
            //    Console.Write(@"                                            ");
            //    Console.SetCursorPosition(90, 5);
            //    Console.Write(@"Distance to Destination: " + Format.FormatDouble(DistanceToDestination, 4));
            //    Console.SetCursorPosition(90, 6);
            //    Console.Write(@"                                            ");
            //    Console.SetCursorPosition(90, 6);
            //    Console.Write(@"Last Move: " + CurrentDirection.ToString());
            //    Console.SetCursorPosition(90, 7);
            //    Console.Write(@"                                            ");
            //    Console.SetCursorPosition(90, 7);
            //    Console.Write(@"Position (x,y): (" + Position[0] + "," + Position[1] + ")");
            //    Console.SetCursorPosition(90, 8);
            //    Console.Write(@"                                            ");
            //    Console.SetCursorPosition(90, 8);
            //    Console.Write(@"Destination (x,y): (" + Destination[0] + "," + Destination[1] + ")");
            //    Console.SetCursorPosition(90, 9);
            //    Console.Write(@"                                            ");
            //    Console.SetCursorPosition(90, 9);
            //    Console.Write(@"Rest Factor: " + ShoudRest);
            //    Console.SetCursorPosition(90, 10);
            //    Console.Write(@"                                            ");
            //    Console.SetCursorPosition(90, 10);
            //    Console.Write(@"# of Rests this Trip: " + Rests);
            //    Console.SetCursorPosition(90, 11);
            //    Console.Write(@"Heading:                                    ");
            //    Console.SetCursorPosition(90, 11);
            //    Console.Write(@"Heading: " + Heading);
            //}
            return string
                .Format(
                    "time: {0} s, Fuel: {1} l, dist: {2} ft,  dir: {3}, x: {4}, y: {5}, Score: {6}, Should Rest: {7}",
                    Seconds,
                    Fuel,
                    Format.FormatDouble(DistanceToDestination, 4),
                    CurrentDirection.ToString(), Position[0], Position[1], Score, ShoudRest);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public delegate void OnPositionChanged(object sender, Position args);
}