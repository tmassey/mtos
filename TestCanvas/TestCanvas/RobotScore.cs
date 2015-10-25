
using System;
using Encog.ML;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;

namespace TestCanvas
{
    public class RobotScore : ICalculateScore
    {
        public double CalculateScore(IMLMethod network)
        {
            var pilot = new NeuralRobot((BasicNetwork)network, false, RobotContol.SourceLocation, RobotContol.DestLocation);
            int score = pilot.ScorePilot();
            RobotContol.Scores.Add(score);
            double best = RobotContol.GetitterationBest();
            double average = RobotContol.GetitterationAverage();
            //lock (RobotContol.ConsoleLock)
            //{
            //    // Console.Clear();
            //    Console.SetCursorPosition(90, 0);
            //    Console.WriteLine(@"                                                     ");
            //    Console.SetCursorPosition(90, 1);
            //    Console.WriteLine(@"                                                     ");
            //    Console.SetCursorPosition(90, 0);
            //    Console.WriteLine(@"Itteration Best: " + best);
            //    Console.SetCursorPosition(90, 1);
            //    Console.WriteLine(@"Itteration avg: " + average);
            //}
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