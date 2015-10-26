//
// Encog(tm) Core v3.2 - .Net Version
// http://www.heatonresearch.com/encog/
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Util.Arrayutil;
using TestCanvas.Annotations;

namespace TestCanvas
{
    public class NeuralRobot : INotifyPropertyChanged
    {
        private readonly NormalizedField _CanGoStats;
        private readonly NormalizedField _hStats;
        private readonly BasicNetwork _network;
        
        private readonly bool _track;
        private RobotSimulator _sim;

        public NeuralRobot(BasicNetwork network, bool track, Position source, Position destination)
        {
            _hStats = new NormalizedField(NormalizationAction.Normalize, "Heading", 359, 0, .9, -.9);
            _CanGoStats = new NormalizedField(NormalizationAction.Normalize, "CanGo", 1, 0, 0.9, -0.9);
            _track = track;
            _network = network;
            sim = new RobotSimulator(source, destination);
        }

        public RobotSimulator sim
        {
            get { return _sim; }
            set
            {
                if (Equals(value, _sim)) return;
                _sim = value;
                OnPropertyChanged();
            }
        }

        public int ScorePilot()
        {
           
            while (sim.Traveling)
            {
                var input = new BasicMLData(2);
                
                input[0] = _CanGoStats.Normalize(sim.ShoudRest);
                input[1] = _hStats.Normalize(sim.Heading);
              


                IMLData output = _network.Compute(input);
                double f = output[0];
                double l = output[1];
                double r = output[2];
                double rev = output[3];
                double rest = output[4];
                double fl = output[5];
                double fr = output[6];
                double bl = output[7];
                double br = output[8];

                var dirs = new Dictionary<RobotDirection, double>
                    {
                        {RobotDirection.Forward, f},
                        {RobotDirection.Left, l},
                        {RobotDirection.Right, r},
                        {RobotDirection.Reverse, rev}
                    
                    };
                KeyValuePair<RobotDirection, double> d = dirs.First(v => v.Value == 1.0);

                
                RobotDirection thrust = d.Key;
                sim.Turn(thrust);
                lock (RobotContol.ConsoleLock)
                {
                    //Console.SetCursorPosition((int) sim.Position[1], (int) sim.Position[0]);
                    //Console.ForegroundColor = sim.Color;
                    //switch (thrust)
                    //{
                    //    case RobotDirection.Forward:
                    //        Console.Write("^");
                    //        break;
                    //    case RobotDirection.Left:
                    //        Console.Write("<");
                    //        break;
                    //    case RobotDirection.Rest:
                    //        Console.Write("#");
                    //        break;
                    //    case RobotDirection.Reverse:
                    //        Console.Write("!");
                    //        break;
                    //    case RobotDirection.Right:
                    //        Console.Write(">");
                    //        break;
                    //    case RobotDirection.FowardLeft:
                    //        Console.Write("7");
                    //        break;
                    //    case RobotDirection.FowardRight:
                    //        Console.Write("9");
                    //        break;
                    //    case RobotDirection.BackLeft:
                    //        Console.Write("1");
                    //        break;
                    //    case RobotDirection.BackRight:
                    //        Console.Write("3");
                    //        break;
                    //}
                }
                //if (_track)
                //{
                //  Console.WriteLine();
                lock (RobotContol.ConsoleLock)
                {
                    sim.Telemetry();
                    if (_track)
                    {
                        switch (thrust)
                        {
                            //case RobotDirection.Rest:
                            //    Thread.Sleep(100);
                            //    break;
                            default:
                                Thread.Sleep(50);
                                break;
                        }
                    }
                }
            }
            return (sim.Score);
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