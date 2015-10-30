using Encog.ML;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;

namespace aXon.Rover
{
    public class RobotScore : ICalculateScore
    {
        public double CalculateScore(IMLMethod network)
        {
            var pilot = new NeuralRobot((BasicNetwork)network, false, RobotContol.SourceLocation, RobotContol.DestLocation);
            int score = pilot.ScorePilot();
            //RobotContol.Scores.Add(score);                    
            return score;
        }


        public bool ShouldMinimize
        {
            get { return false; }
        }


        /// <inheritdoc />
        public bool RequireSingleThreaded
        {
            get { return false; }
        }
    }
}