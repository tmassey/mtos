using System;
using System.Collections.Generic;
using System.IO;
using Encog;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Genetic;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Pattern;
using Encog.Persist;
using aXon.Rover.Models;

namespace aXon.Rover
{

    public class RobotContol 
    {
        public static Object ConsoleLock;
        public static Object NetworkLock;
        public static Position SourceLocation { get; set; }
        public static Position DestLocation { get; set; }
        public static List<double> Scores { get; set; }
        public RobotContol()
        {
            NetworkLock = "Lock";
            ConsoleLock = "Lock";
        }
        #region IExample Members

        public void Execute()
        {           
            for (double slat = 0; slat <= 60; slat += 20)
            {
                for (double slon = 0; slon <= 90; slon += 20)
                {
                    for (double lat = 0; lat <= 50; lat += 20)
                    {
                        for (double lon = 0; lon <= 50; lon += 20)
                        {
                            SourceLocation = new Position(slat, slon);
                            DestLocation = new Position(lat, lon);
                            BuildNetwork(slat, slon, lat, lon);
                            //lock (ConsoleLock)
                            //{
                            //    Console.Clear();
                            //}
                            SourceLocation = new Position(lat, lon);
                            DestLocation = new Position(slat, slon);
                            BuildNetwork(lat, lon, slat, slon);
                            //lock (ConsoleLock)
                            //{
                            //    Console.Clear();
                            //}
                        }
                    }
                }
            }
            for (double slat = 0; slat <= 60; slat += 1)
            {
                for (double slon = 0; slon <= 90; slon += 1)
                {
                    for (double lat = 0; lat <= 50; lat += 1)
                    {
                        for (double lon = 0; lon <= 50; lon += 1)
                        {
                            SourceLocation = new Position(slat, slon);
                            DestLocation = new Position(lat, lon);
                            BuildNetwork(slat, slon, lat, lon);
                            
                            SourceLocation = new Position(lat, lon);
                            DestLocation = new Position(slat, slon);
                            BuildNetwork(lat, lon, slat, slon);
                            
                        }
                    }
                }
            }
            for (double slat = 0; slat <= 60; slat += 5)
            {
                for (double slon = 0; slon <= 90; slon += 5)
                {
                    SourceLocation = new Position(slat, slon);
                    for (double lat = 0; lat <= 60; lat += 10)
                    {
                        for (double lon = 0; lon <= 90; lon += 10)
                        {
                            DestLocation = new Position(lat, lon);
                            string fn = @"c:\robotnn\Robot_From" + slat + "_" + slon + "_To_" + lat + "_" + lon + ".net";
                            BasicNetwork network = null;
                            //Console.WriteLine(@"\nHow the winning network got to its destination:");
                            lock (NetworkLock)
                            {
                                //string fs = File.ReadAllText(fn);
                                //network = JsonConvert.DeserializeObject<BasicNetwork>(fs);
                                //network.Structure.FinalizeStructure();
                                network = (BasicNetwork) EncogDirectoryPersistence.LoadObject(new FileInfo(fn));
                            }
                           // var pilot = new NeuralRobot(network, true);
                            //Console.WriteLine(pilot.ScorePilot(SourceLocation, DestLocation));
                        }
                    }
                }
            }
            EncogFramework.Instance.Shutdown();
        }

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
                        ((IMLResettable) result).Reset();
                        return result;
                    }, new RobotScore(), 250);
                int epoch = 1;
                var scoresAverage = new List<double>();
                double netavg = 0;
                //lock (ConsoleLock)
                //{
                //    Console.Clear();
                //}
                int chromosomes = 128;
                while (epoch <= 100 || train.Error <= 0)
                {
                    Scores = new List<double>();
                    train.Iteration();
                    double average = GetitterationAverage();
                    scoresAverage.Add(average);
                    double avg = GetTrainAverage(scoresAverage);

                    //lock (ConsoleLock)
                    //{
                    //    // Console.Clear();
                    //    //Console.SetCursorPosition(60, 2);
                    //    //Console.WriteLine(@"Epoch #" + epoch + @" Score:" + train.Error +
                    //    //                  @" Itteration Average: " +
                    //    //                  average + " Overall Avg:" + netavg);

                    //    Console.SetCursorPosition(90, 13);
                    //    Console.Write(@"                                            ");
                    //    Console.SetCursorPosition(90, 14);
                    //    Console.Write(@"                                            ");
                    //    Console.SetCursorPosition(90, 15);
                    //    Console.Write(@"                                            ");
                    //    Console.SetCursorPosition(90, 16);
                    //    Console.Write(@"                                            ");
                    //    Console.SetCursorPosition(90, 13);
                    //    Console.Write(@"Epoch #" + epoch);
                    //    Console.SetCursorPosition(90, 14);
                    //    Console.Write(@"Score:" + train.Error);
                    //    Console.SetCursorPosition(90, 15);
                    //    Console.Write(@"Itteration Average: " + average);
                    //    Console.SetCursorPosition(90, 16);
                    //    Console.Write(@"Overall Avg:" + netavg);
                    //}
                    lock (NetworkLock)
                    {
                        if (train.Error > 0)
                        {
                            FileStream fs = File.Create(fn);
                            EncogDirectoryPersistence.SaveObject(fs, train.Method);
                            fs.Close();
                        }
                    }
                    epoch++;

                    if (epoch > 5 && train.Error <= 0)
                    {
                        //lock (ConsoleLock)
                        //{
                        //    Console.Clear();
                        //}
                        train = new MLMethodGeneticAlgorithm(() =>
                            {
                                BasicNetwork result = CreateNetwork();
                                ((IMLResettable) result).Reset();
                                return result;
                            }, new RobotScore(), chromosomes*2);
                        chromosomes *= 2;
                        //lock (ConsoleLock)
                        //{
                        //    Console.SetCursorPosition(90, 2);
                        //    Console.WriteLine(@" Chromosomes: " + chromosomes);
                        //}
                        epoch = 1;
                    }

                    netavg = avg;
                }
                //lock (ConsoleLock)
                //{
                //    Console.Clear();
                //}
            }
            else
            {
                lock (NetworkLock)
                {                    
                    network = (BasicNetwork) EncogDirectoryPersistence.LoadObject(new FileInfo(fn));
                }
                //var pilot = new NeuralRobot(network, true);
                //Console.WriteLine(pilot.ScorePilot(SourceLocation, DestLocation));
            }
        }

        private static double GetTrainAverage(List<double> scoresAverage)
        {
            double networkaverage = 0;
            foreach (double d in scoresAverage)
            {
                networkaverage += d;
            }
            double avg = networkaverage/scoresAverage.Count;
            return avg;
        }

        public static double GetitterationBest(List<double> scores = null)
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

            double average = total/Scores.Count;
            return average;
        }

        #endregion

        public static BasicNetwork CreateNetwork()
        {
            var pattern = new FeedForwardPattern {InputNeurons = 2};
            pattern.AddHiddenLayer(75);
            pattern.OutputNeurons = 9;
            pattern.ActivationFunction = new ActivationCompetitive();
            var network = (BasicNetwork) pattern.Generate();
            network.Reset();
            return network;
        }
    }
}