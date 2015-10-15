//
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
using System.Linq;
using System.Threading;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Util.Arrayutil;
using aXon.Rover.Enumerations;
using aXon.Rover.Models;

namespace aXon.Rover
{
    public class NeuralRobot
    {
        private readonly NormalizedField _fuelStats;
        private readonly BasicNetwork _network;
        private readonly bool _track;
        private readonly NormalizedField _totalDistStats;


        private readonly NormalizedField _posxStats;
        private readonly NormalizedField _destyStats;
        private readonly NormalizedField _destxStats;
        private readonly NormalizedField _posyStats;

        private readonly NormalizedField _dStats;
        private readonly NormalizedField _hStats;
        private readonly NormalizedField _CanGoStats;






        public NeuralRobot(BasicNetwork network, bool track)
        {
            _fuelStats = new NormalizedField(NormalizationAction.Normalize, "fuel", 200, 0, -0.9, 0.9);
            _totalDistStats = new NormalizedField(NormalizationAction.Normalize, "DistanceToDestination", 130, 0, 0.9, -0.9);

            _posxStats = new NormalizedField(NormalizationAction.Normalize, "posx", 80, 0, 0.9, -0.9);
            _destxStats = new NormalizedField(NormalizationAction.Normalize, "Right", 80, 0, 0.9, -0.9);
            _posyStats = new NormalizedField(NormalizationAction.Normalize, "posy", 80, 0, 0.9, -0.9);
            _destyStats = new NormalizedField(NormalizationAction.Normalize, "reverse", 80, 0, 0.9, -0.9);

            _dStats = new NormalizedField(NormalizationAction.Normalize, "direction", 2000, 0, -0.9, 0.9);
            _hStats = new NormalizedField(NormalizationAction.Normalize, "Heading", 359, 0, .9, -.9);

            _CanGoStats = new NormalizedField(NormalizationAction.Normalize, "CanGo", 1, 0, 0.9, -0.9);


            //_velocityStats = new NormalizedField(NormalizationAction.Normalize, "velocity", RobotSimulator.TerminalVelocity, -RobotSimulator.TerminalVelocity,-0.9, 0.9);

            _track = track;
            _network = network;
        }



        public int ScorePilot(Position source, Position destination)
        {
            var sim = new RobotSimulator(source, destination);
            while (sim.Traveling)
            {
                var input = new BasicMLData(9);
                input[0] = _fuelStats.Normalize(sim.Fuel);
                input[1] = _totalDistStats.Normalize(sim.DistanceToDestination);
                input[2] = _CanGoStats.Normalize(sim.ShoudRest);
                input[3] = _posxStats.Normalize(sim.Position[0]);
                input[4] = _posyStats.Normalize(sim.Position[1]);
                input[5] = _destxStats.Normalize(0);
                input[6] = _destyStats.Normalize(0);
                input[7] = _dStats.Normalize(sim.Rests);
                input[8] = _hStats.Normalize(sim.Heading);
                // input[6] = _dStats.Normalize((double)sim.CurrentDirection);

                //input[7] = _CanGoStats.Normalize(Convert.ToDouble(sim.CanGoInDirection(RobotDirection.Forward)));
                //input[8] = _CanGoStats.Normalize(Convert.ToDouble(sim.CanGoInDirection(RobotDirection.Reverse)));
                //input[9] = _CanGoStats.Normalize(Convert.ToDouble(sim.CanGoInDirection(RobotDirection.Left)));
                //input[10] = _CanGoStats.Normalize(Convert.ToDouble(sim.CanGoInDirection(RobotDirection.Right)));


                IMLData output = _network.Compute(input);
                double f = output[0];
                double l = output[1];
                double r = output[2];
                double rev = output[3];
                double rest = output[4];

                var dirs = new System.Collections.Generic.Dictionary<string, double>
                    {
                        { "f",f},
                        {"l",l},
                        {"r",r},
                        {"rev",rev},
                        {"rest",rest}
                        
                    };
                var d = dirs.First(v => v.Value == 1.0);

                int cnt = 0;
                //foreach (var dir in dirs)
                //{
                //    lock (RobotContol.ConsoleLock)
                //    {
                //        Console.SetCursorPosition(0, cnt);
                //        Console.WriteLine(dir.Key + ": " + dir.Value);
                //        cnt++;
                //    }
                //}

                RobotDirection thrust = RobotDirection.Rest;

                switch (d.Key)
                {
                    case "f":
                        thrust = RobotDirection.Forward;
                        break;
                    case "rev":
                        thrust = RobotDirection.Reverse;
                        break;
                    case "l":
                        thrust = RobotDirection.Left;
                        break;
                    case "r":
                        thrust = RobotDirection.Right;
                        break;
                    case "rest":
                        thrust = RobotDirection.Rest;
                        break;
                }



                sim.Turn(thrust);
                //lock (RobotContol.ConsoleLock)
                //{
                //    Console.SetCursorPosition(60, cnt);
                //    Console.WriteLine(thrust.ToString());
                //    cnt++;
                //}
                lock (RobotContol.ConsoleLock)
                {
                    //    Console.SetCursorPosition(0, cnt);
                    //    Console.WriteLine("Score: " + sim.Score);
                    //    cnt++;
                    // Console.Clear();
                    Console.SetCursorPosition((int)sim.Position[1], (int)sim.Position[0]);
                    Console.ForegroundColor = sim.Color;
                    switch (thrust)
                    {
                        case RobotDirection.Forward:
                            Console.Write("#");
                            break;
                        case RobotDirection.Left:
                            Console.Write("L");
                            break;
                        case RobotDirection.Rest:
                            Console.Write("@");
                            break;
                        case RobotDirection.Reverse:
                            Console.Write("*");
                            break;
                        case RobotDirection.Right:
                            Console.Write("R");
                            break;
                    }

                    Console.ForegroundColor = ConsoleColor.White;
                }
                if (_track)
                {
                    //  Console.WriteLine();
                    lock (RobotContol.ConsoleLock)
                    {
                        Console.SetCursorPosition(60, 3);
                        Console.WriteLine(sim.Telemetry());
                        switch (thrust)
                        {
                            case RobotDirection.Rest:
                                Thread.Sleep(10);
                                break;
                            default:
                                Thread.Sleep(5);
                                break;

                        }

                    }
                }

            }
            //Console.Clear();
            return (sim.Score);
        }
    }
}

