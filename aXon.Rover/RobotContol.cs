using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Encog;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Genetic;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Pattern;
using Encog.Persist;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RabbitMQ.Client;
using aXon.Rover.Models;
using aXon.TaskTransport;
using aXon.TaskTransport.Messages;

namespace aXon.Rover
{
    public class RobotContol
    {
        public static Object ConsoleLock;
        public static Object NetworkLock;
        private static MessageQueue<TaskProgressMessage> _ProgressQueue;
        private static IConnection _Connection;
        private readonly DateTime _starttime;
        private readonly Guid _taskid;

        public RobotContol(DateTime start, Guid taskId)
        {
            _starttime = start;
            _taskid = taskId;
            NetworkLock = "Lock";
            ConsoleLock = "Lock";
            InitConnection();
            Mds = new MongoDataService();
            _ProgressQueue = new MessageQueue<TaskProgressMessage>(false, _Connection);
            Warehouse = Mds.GetCollectionQueryModel<Warehouse>().FirstOrDefault();
        }

        public MongoDataService Mds { get; set; }
        public static Warehouse Warehouse { get; set; }
        public static Position SourceLocation { get; set; }
        public static Position DestLocation { get; set; }
        public static List<double> Scores { get; set; }

        private static void InitConnection()
        {
            var factory = new ConnectionFactory {HostName = "192.169.164.138"};
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
            _Connection = factory.CreateConnection();
        }


        public void BuildNetwork(double slat, double slon, double lat, double lon)
        {
            string hostname = Dns.GetHostName();
            SourceLocation = new Position(slat, slon);
            DestLocation = new Position(lat, lon);
            BasicNetwork network = CreateNetwork();
            string fn = @"Robot_From" + slat + "_" + slon + "_To_" + lat + "_" + lon + ".net";
            int chromosomes = 128;
            IMLTrain train;
            train = new MLMethodGeneticAlgorithm(() =>
                {
                    BasicNetwork result = CreateNetwork();
                    ((IMLResettable) result).Reset();
                    return result;
                }, new RobotScore(), chromosomes);
            int epoch = 1;
            var scoresAverage = new List<double>();
            double netavg = 0;

            while (epoch <= chromosomes || train.Error <= 0)
            {
                GC.Collect();
                Scores = new List<double>();
                train.Iteration();
                double average = GetitterationAverage();
                scoresAverage.Add(average);
                double avg = GetTrainAverage(scoresAverage);
                //_ProgressQueue.Publish(new TaskProgressMessage
                //    {
                //        CurrentTime = DateTime.Now,
                //        PercentComplete = epoch,
                //        StartTime = _starttime,
                //        Status = TaskStatus.InProcess,
                //        TaskId = _taskid,
                //        MessageId = Guid.NewGuid(),
                //        TransmisionDateTime = DateTime.Now,
                //        Details =
                //            hostname + @" Epoch #" + epoch + @" Score:" + train.Error + @" Chromosomes: " + chromosomes
                //    }
                //    );

                lock (NetworkLock)
                {
                    if (train.Error > 0)
                    {
                        NeuralNetwork net =
                            Mds.GetCollectionQueryModel<NeuralNetwork>(Query.And(Query.EQ("StartPosition.X", slat),
                                                                                 Query.EQ("StartPosition.Y", slon),
                                                                                 Query.EQ("EndPosition.X", lat),
                                                                                 Query.EQ("EndPosition.Y", lon)))
                               .FirstOrDefault();
                        if (net == null)
                            net = new NeuralNetwork
                                {
                                    EndPosition = new Position(lat, lon),
                                    Id = Guid.NewGuid(),
                                    StartPosition = new Position(slat, slon)
                                };

                        FileStream fs = File.Create(fn);
                        EncogDirectoryPersistence.SaveObject(fs, train.Method);
                        fs.Close();
                        MongoCollection<NeuralNetwork> col = Mds.DataBase.GetCollection<NeuralNetwork>("NeuralNetwork");
                        col.Save(net);
                        Mds.SaveFile(fn, net.Id);
                        File.Delete(fn);
                    }
                }
                epoch++;

                if (epoch > (chromosomes/2) && train.Error <= 0)
                {
                    chromosomes *= 2;

                    train = new MLMethodGeneticAlgorithm(() =>
                        {
                            BasicNetwork result = CreateNetwork();
                            ((IMLResettable) result).Reset();
                            return result;
                        }, new RobotScore(), chromosomes);

                    epoch = 1;
                }

                netavg = avg;
                if (chromosomes == 4096)
                    break;
            }
            _ProgressQueue.Publish(new TaskProgressMessage
                {
                    CurrentTime = DateTime.Now,
                    PercentComplete = 100,
                    StartTime = _starttime,
                    Status = TaskStatus.Complete,
                    TaskId = _taskid,
                    MessageId = Guid.NewGuid(),
                    TransmisionDateTime = DateTime.Now,
                    Details = hostname
                }
                );
            EncogFramework.Instance.Shutdown();
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


        public static BasicNetwork CreateNetwork()
        {
            var pattern = new FeedForwardPattern {InputNeurons = 2};
            pattern.AddHiddenLayer(75);
            pattern.OutputNeurons = 4;
            pattern.ActivationFunction = new ActivationCompetitive();
            var network = (BasicNetwork) pattern.Generate();
            network.Reset();
            return network;
        }
    }
}