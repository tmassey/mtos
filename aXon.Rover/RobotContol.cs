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
using System.Collections.Generic;
using System.IO;
using Encog;
using Encog.Engine.Network.Activation;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Pattern;
using Encog.ML.Genetic;
using Encog.ML;
using Encog.Persist;
using aXon.Rover.Models;

namespace aXon.Rover
{
	/// <summary>
	/// A lunar lander game where the neural network learns to land a space craft.  
	/// The neural network learns the proper amount of thrust to land softly 
	/// and conserve fuel.
	/// 
	/// This example is unique because it uses supervised training, yet does not 
	/// have expected values.  For this it can use genetic algorithms or 
	/// simulated annealing.
	/// </summary>
	public class RobotContol //: IExample
	{
        //public static ExampleInfo Info {
        //    get {
        //        var info = new ExampleInfo (
        //                       typeof(RobotContol),
        //                       "robot",
        //                       "Train a neural network to Control a robot and get it to its destination using the least amount of battery and the shortest route.",
        //                       "use a map to find the best path");
        //        return info;
        //    }
        //}

        private static readonly Random _random = new Random();
        public static Object ConsoleLock;
        public static Object NetworkLock;
        public static Position SourceLocation { get; set; }
        public static Position DestLocation { get; set; }
        public static List<double> Scores { get; set; }
		#region IExample Members

		/// <summary>
		/// Program entry point.
		/// </summary>
		/// <param name="app">Holds arguments and other info.</param>
		public void Execute ()
		{

		    BasicNetwork network = (BasicNetwork)EncogDirectoryPersistence.LoadObject(new System.IO.FileInfo(@"c:\robotnn\robot799.net"));
		    ConsoleLock = "Lock";
			IMLTrain train;

            
                train = new MLMethodGeneticAlgorithm(() =>
                {
                    BasicNetwork result = CreateNetwork();
                    ((IMLResettable)result).Reset();
                    return result;
                }, new RobotScore(), 5000);
            

			int epoch = 1;

			for (int i = 0; i < 50000; i++) {
                train.Iteration ();
			    //var Robot = new NeuralRobot(network,true);
			    //var score=Robot.ScorePilot();
			    Console.Clear();
                Console.SetCursorPosition(60, 0);
				Console.WriteLine (@"Epoch #" + epoch + @" Score:" + train.Error);
                EncogDirectoryPersistence.SaveObject(File.Create(@"c:\robotnn\robot" + epoch + ".net"),  train.Method);
				epoch++;
			}

			Console.WriteLine (@"\nHow the winning network got to its destination:");
            network = (BasicNetwork)train.Method;
            var pilot = new NeuralRobot(network, true);
            //Console.WriteLine(pilot.ScorePilot());
            EncogDirectoryPersistence.SaveObject(File.Create(@"c:\robot.net"), network);
			EncogFramework.Instance.Shutdown ();
		}

		#endregion
        private static void BuildNetwork(double slat, double slon, double lat, double lon)
        {
            BasicNetwork network = null;
            string fn = @"c:\robotnn\Robot_From" + slat + "_" + slon + "_To_" + lat + "_" + lon + ".net";
            if (!File.Exists(fn))
            {
                network = CreateNetwork();
                IMLTrain train;
                train = new MLMethodGeneticAlgorithm(() =>
                {
                    BasicNetwork result = CreateNetwork();
                    ((IMLResettable)result).Reset();
                    return result;
                }, new RobotScore(), 250);
                int epoch = 1;
                var scoresAverage = new List<double>();
                double netavg = 0;
                lock (ConsoleLock)
                {
                    Console.Clear();
                }
                int chromosomes = 128;
                while (epoch <= 25 || train.Error <= 0)
                {
                    Scores = new List<double>();
                    train.Iteration();
                    double average = GetitterationAverage();
                    scoresAverage.Add(average);
                    double avg = GetTrainAverage(scoresAverage);

                    lock (ConsoleLock)
                    {
                        // Console.Clear();
                        Console.SetCursorPosition(60, 2);
                        Console.WriteLine(@"Epoch #" + epoch + @" Score:" + train.Error +
                                          @" Itteration Average: " +
                                          average + " Overall Avg:" + netavg);
                    }
                    lock (NetworkLock)
                    {
                        if (train.Error > 0)
                        {
                            var fs = File.Create(fn);
                            EncogDirectoryPersistence.SaveObject(fs, train.Method);
                            fs.Close();
                        }
                    }
                    epoch++;

                    if (epoch > 5 && train.Error <= 0)
                    {
                        lock (ConsoleLock)
                        {
                            Console.Clear();
                        }
                        train = new MLMethodGeneticAlgorithm(() =>
                        {
                            BasicNetwork result = CreateNetwork();
                            ((IMLResettable)result).Reset();
                            return result;
                        }, new RobotScore(), chromosomes * 2);
                        chromosomes *= 2;
                        lock (RobotContol.ConsoleLock)
                        {
                            // Console.Clear();
                            Console.SetCursorPosition(130, 0);
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine(@" Chromosomes: " + chromosomes);
                        }
                        epoch = 1;
                    }

                    netavg = avg;
                }
                lock (ConsoleLock)
                {
                    Console.Clear();
                }
            }
            else
            {
                lock (NetworkLock)
                {
                    //string fs = File.ReadAllText(fn);
                    //network = JsonConvert.DeserializeObject<BasicNetwork>(fs);
                    //network.Structure.FinalizeStructure();
                    network = (BasicNetwork)EncogDirectoryPersistence.LoadObject(new FileInfo(fn));
                }
                var pilot = new NeuralRobot(network, true);
                Console.WriteLine(pilot.ScorePilot(SourceLocation, DestLocation));
            }
        }

        private static double GetTrainAverage(List<double> scoresAverage)
        {
            double networkaverage = 0;
            foreach (double d in scoresAverage)
            {
                networkaverage += d;
            }
            double avg = networkaverage / scoresAverage.Count;
            return avg;
        }

        public static double GetitterationAverage(List<double> scores = null)
        {
            if (scores == null)
                scores = Scores;
            double total = -999999999999;
            foreach (double d in scores)
            {
                if (d > total)
                    total = d;
            }
            return total;
            //double average = total/Scores.Count;
            //return average;
        }
		public static BasicNetwork CreateNetwork ()
		{
			var pattern = new FeedForwardPattern() { InputNeurons = 9 };
			pattern.AddHiddenLayer (21);
			pattern.OutputNeurons = 5;
            pattern.ActivationFunction = new ActivationCompetitive();
			var network = (BasicNetwork)pattern.Generate ();
			network.Reset ();
			return network;
		}
	}
}
