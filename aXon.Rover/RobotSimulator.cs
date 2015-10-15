// Encog(tm) Core v3.2 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//

using System;
using Encog.Util;
using aXon.Rover.Enumerations;
using aXon.Rover.Models;
using aXon.Rover.Utilities;

namespace aXon.Rover
{
    public class RobotSimulator
    {
        //public const double Gravity = 1.62;
        //public const double Thrust = 3;
        //public const double TerminalVelocity = 100;
        private static Random _random = new Random();
        public double[] Position { get; set; }
        public double[] Destination { get; set; }
        public double[] StartPosition { get; set; }
        public double StartDistance { get; set; }
        public ConsoleColor Color = GetRandomConsoleColor();


        private static ConsoleColor GetRandomConsoleColor()
        {
            var consoleColors = Enum.GetValues(typeof(ConsoleColor));
            var clr = (ConsoleColor)consoleColors.GetValue(_random.Next(consoleColors.Length));
            if (clr == ConsoleColor.Black)
                clr = GetRandomConsoleColor();
            return clr;
        }

        public RobotSimulator(Position source, Position destination)
        {
            Success = -10000;
            Fuel = 200;
            Seconds = 0;
            Altitude = 100000;
            Rests = 0;
            Turns = 0;
            Position = new double[2] { source.Latitude, source.Longitude };
            Destination = new double[2] { destination.Latitude, destination.Longitude };
            StartPosition = new double[2] { source.Latitude, source.Longitude };
            CurrentDirection = RobotDirection.Forward;
            DistanceToDestination = CalculateDistance();
            LastDistance = DistanceToDestination;
            PreviousDistance = LastDistance;
            StartDistance = DistanceToDestination;
            UpdateHeading();
            lock (RobotContol.ConsoleLock)
            {
                Console.ForegroundColor = Color;
                Console.SetCursorPosition((int)Destination[1], (int)Destination[0]);
                Console.Write("X");
            }


        }

        private double CalculateDistance()
        {
            var xdist = Position[0] - Destination[0];
            var ydist = Position[1] - Destination[1];
            if (xdist < 0)
                xdist = xdist * -1;
            if (ydist < 0)
                ydist = ydist * -1;

            return xdist + ydist;


        }
        private double CalculateDistance(double[] source, double[] dest)
        {
            var xdist = source[0] - dest[0];
            var ydist = source[1] - dest[1];
            if (xdist < 0)
                xdist = xdist * -1;
            if (ydist < 0)
                ydist = ydist * -1;

            return xdist + ydist;


        }

        public double Fuel { get; set; }
        public int Seconds { get; set; }
        public double Altitude { get; set; }
        public double Turns { get; set; }
        public double Rests { get; set; }
        public RobotDirection CurrentDirection { get; set; }
        public int Score
        {
            get
            {
                var startdist = CalculateDistance(StartPosition, Destination);
                var lastdist = CalculateDistance();
                var closeness = startdist - lastdist;
                var traveled = startdist - closeness;

                var perctraverced = (startdist / traveled);
                if (Success > 0)
                    perctraverced = 10;
                return (int)((Fuel * 10) + Success + perctraverced + (Rests * -1) + (Seconds * -1));
            }
        }
        public double ShoudRest
        {
            get { return Fuel / 200; }
        }
        public void Turn(RobotDirection newDirection)
        {
            //Thread.Sleep(250);
            CurrentDirection = newDirection;
            Seconds++;
            if (newDirection == RobotDirection.Rest)
            {
                Rests++;
                Fuel += 10;
                if (Fuel >= 200)
                    Fuel = 200;
                return;
            }
            if (Fuel > 0)
            {
                Fuel -= 5;
            }


            if (!CanGoInDirection(newDirection))
            {
                //Console.Write(@"#");
                return;
            }
            //Console.Write(@".");
            MoveToNewPostion(newDirection);

            DistanceToDestination = CalculateDistance();


            if (Altitude < 0)
                Altitude = 0;
            // Console.WriteLine(Telemetry());
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
            }
            if (Position[0] < 0)
                Position[0] = 0;
            if (Position[1] < 0)
                Position[1] = 0;

            //if (Position[0] > 90)
            //    Position[0] = 90;
            //if (Position[1] > 60)
            //    Position[1] = 60;
            UpdateHeading();
        }

        private void UpdateHeading()
        {
            var src = new Position(Position[0], Position[1]);
            var dest = new Position(Destination[0], Destination[1]);
            PositionBearingCalculator calc = new PositionBearingCalculator(new AngleConverter());
            Heading = calc.CalculateBearing(dest, src);
        }

        public void setdestlose()
        {
            //lock (RobotContol.ConsoleLock)
            //{
            //    Console.ForegroundColor = ConsoleColor.Black;
            //    Console.SetCursorPosition((int)Destination[1], (int)Destination[0]);
            //    Console.Write("X");
            //}
        }

        public bool CanGoInDirection(RobotDirection newDirection)
        {
            if (Fuel == 0) return false;
            return true;
        }

        public String Telemetry()
        {
            return string
                .Format("time: {0} s, Fuel: {1} l, dist: {2} ft,  dir: {3}, x: {4}, y: {5}, Score: {6}, Should Rest: {7}",
                Seconds,
                Fuel,
                Format.FormatDouble(DistanceToDestination, 4),
                CurrentDirection.ToString(), Position[0], Position[1], Score, ShoudRest);
        }

        public bool Traveling
        {
            get
            {
                if (DistanceToDestination == 0)
                {
                    lock (RobotContol.ConsoleLock)
                    {
                        Console.ForegroundColor = Color;
                        Console.SetCursorPosition((int)Destination[1], (int)Destination[0]);
                        Console.Write("W");
                    }
                    Success = 1000;
                    return false;
                }
                var startdist = CalculateDistance(StartPosition, Destination);
                var lastdist = CalculateDistance();
                var perdist = 100000 / startdist;
                var closeness = lastdist * perdist;
                //double traveled = closeness;
                //if(closeness<0)
                //    traveled = closeness * -1;
                if (PreviousDistance < DistanceToDestination)
                {
                    Seconds *= 100;
                    Success = closeness * -1;
                    setdestlose();
                    return false;
                }
                else
                {
                    PreviousDistance = LastDistance;
                    LastDistance = DistanceToDestination;
                }
                //if (StartDistance + 2 < DistanceToDestination)
                //    return false;
                if (Seconds >= 2000)
                {
                    Seconds *= 100;


                    Success = closeness * -1;

                    setdestlose();
                    return false;
                }
                if (Fuel == 0 && Seconds >= 2000 && DistanceToDestination > 0)
                {
                    Seconds *= 100;
                    Success = closeness * -1;
                    setdestlose();
                    return false;
                }
                return true;

            }
        }
        public double Success { get; set; }
        public double LastDistance { get; set; }
        public double PreviousDistance { get; set; }
        public double DistanceToDestination { get; set; }

        public double LeftDistance
        {
            get
            {

                var col = Position[1];

                return col;
            }
        }

        public double FrontDistance
        {
            get
            {
                var row = Position[0];
                var col = Position[1];
                return row;
            }
        }

        public double RightDistance
        {
            get
            {
                var row = Position[0];
                var col = Position[1];
                return col;
            }
        }

        public double BackDistance
        {
            get
            {
                var row = Position[0];
                return -row;
            }
        }

        public double Heading { get; set; }
    }
}
