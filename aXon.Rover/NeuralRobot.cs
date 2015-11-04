using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Util.Arrayutil;
using aXon.Rover.Annotations;
using aXon.Rover.Enumerations;
using aXon.Rover.Models;

namespace aXon.Rover
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
                
                input[0] = sim.DistanceToDestination;
                input[1] = _hStats.Normalize(sim.Heading);
              


                IMLData output = _network.Compute(input);
                double f = output[0];
                double l = output[1];
                double r = output[2];
                double rev = output[3];              

                var dirs = new Dictionary<CommandDirection, double>
                    {
                        {CommandDirection.MoveForward, f},
                        {CommandDirection.TurnLeft, l},
                        {CommandDirection.TurnRight, r},
                        {CommandDirection.MoveInReverse, rev}
                    
                    };
                KeyValuePair<CommandDirection, double> d = dirs.First(v => v.Value == 1.0);


                CommandDirection thrust = d.Key;
                sim.Turn(thrust);
               
                lock (RobotContol.ConsoleLock)
                {
                    
                    if (_track)
                    {
                        sim.Telemetry();
                        switch (thrust)
                        {                           
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