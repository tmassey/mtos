using Encog.ML;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;

namespace aXon.Rover
{
    public class RobotScore : ICalculateScore
    {
        public double CalculateScore(IMLMethod network)
        {
            NeuralRobot pilot = new NeuralRobot((BasicNetwork)network, false);

            var score = pilot.ScorePilot(RobotContol.SourceLocation, RobotContol.DestLocation);
            RobotContol.Scores.Add(score);
            var average = RobotContol.GetitterationAverage();

            //lock (RobotContol.ConsoleLock)
            //{
            //    // Console.Clear();
            //    Console.SetCursorPosition(60, 0);
            //    Console.ForegroundColor = ConsoleColor.White;
            //    Console.WriteLine(@" Itteration Best: " + average);
            //}
            return score;
        }


        public bool ShouldMinimize
        {
            get { return false; }
        }


        /// <inheritdoc/>
        public bool RequireSingleThreaded
        {
            get { return false; }
        }
    }
}
