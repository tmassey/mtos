using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Automatonymous;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Persist;
using Encog.Util.Arrayutil;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RabbitMQ.Client;
using aXon.Rover;
using aXon.Rover.Enumerations;
using aXon.Rover.Models;
using aXon.TaskTransport;
using aXon.TaskTransport.Messages;
using aXon.Warehouse.Modules.Robotics.Robot.Models;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace aXon.RX02.ControlServer
{
    public class RobotManager
    {
        private readonly Robot _robot;
        private RobotState _state;
        private RobotStateMachine _machine;
        private readonly MongoDataService ds = new MongoDataService();
        public MqttClient Client;
        private static IConnection _Connection;
        private MessageQueue<RobotJobMessage> _messageQueue;
        private NeuralRobot _neural;
        private BasicNetwork _NeuralNet;
        public RobotManager(string serial)
        {
            Client  = new MqttClient(IPAddress.Parse("192.168.1.19"));
            byte code = Client.Connect("aXon" + serial);
            var ques = BuildQueues(serial);
            byte[] qos = new byte[ques.Length];
            for (int x = 0; x != ques.Length;x++ )
             {
                    qos[x] = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE;
                }
            Client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            Client.Subscribe(ques, qos);
            
            _robot = ds.GetCollectionQueryModel<Robot>(Query.EQ("SerialNumber", serial)).FirstOrDefault();
            if (_robot == null)
            {
                _robot = new Robot
                             {
                                 Id = Guid.NewGuid(),
                                 SerialNumber = serial,
                                 IsActiveRecord = true,
                                 LastEditDateTime = DateTime.Now,
                                 CreateDateTime = DateTime.Now,
                                 CurrentMode = RoverMode.WaitingForJob,
                                 CurrentLocation = new Position(0, 0)
                             };
                MongoCollection<Robot> col = ds.DataBase.GetCollection<Robot>("Robot");
                col.Save(_robot);             
            }
            _state = new RobotState() { Name = _robot.SerialNumber };
            InitConnection();
            _messageQueue = new MessageQueue<RobotJobMessage>(true,_Connection){GetNext=false};
            _messageQueue.OnReceivedMessage += _messageQueue_OnReceivedMessage;
            _machine = new RobotStateMachine();
            _machine.RaiseEvent(_state, _machine.AuthenticateEvent);
            Client.Publish(@"/" + serial + @"/AUTH", new byte[] {1});
            _messageQueue.GetNext = true;

        }

        private static string[] BuildQueues(string serial)
        {
            var queues = new[]
                             {
                                 "/serial/ACK",
                                 "/serial/AUTH",
                                 "/serial/BD",
                                 "/serial/FD",
                                 "/serial/LD",
                                 "/serial/RD",
                                 "/serial/ARV",
                                 "/serial/LW",
                                 "/serial/STOP",
                                 "/serial/RCLX",
                                 "/serial/RClY",
                                 "/serial/VR",
                                 "/serial/MVW"
                             };
            var newQueues = new List<string>();
            foreach (string q in queues)
            {
                newQueues.Add(q.Replace("serial", serial));
            }
            string[] ques = newQueues.ToArray();
            return ques;
        }

        void _messageQueue_OnReceivedMessage(object sender, RobotJobMessage args)
        {
            Console.WriteLine("Job Arrive: " + args.RobotSerial + " NetId: " + args.NetworkId.ToString());
            _machine.RaiseEvent(_state, _machine.JobArrivesEvent);            
            var network = ds.GetCollectionQueryModel<NeuralNetwork>(Query.EQ("_id",args.NetworkId)).FirstOrDefault();
            var fn = network.Id.ToString();
            _NeuralNet = GetNeuralNetwork(network, fn);
            ProcessMovement();
        }

        private BasicNetwork GetNeuralNetwork(NeuralNetwork network, string fn)
        {
            BasicNetwork nn;
            if(RobotContol.NetworkLock==null)
                RobotContol.NetworkLock="t";
            lock (RobotContol.NetworkLock)
            {
                var rawbytes = ds.OpenFile(network.Id);

                File.WriteAllBytes(fn, rawbytes);
                nn = (BasicNetwork) EncogDirectoryPersistence.LoadObject(new FileInfo(fn));
                File.Delete(fn);
            }
            _neural = new NeuralRobot(new BasicNetwork(), true, network.StartPosition, network.EndPosition);
            RobotContol.Warehouse = ds.GetCollectionQueryModel<aXon.Rover.Models.Warehouse>().FirstOrDefault();
            return nn;
        }

        private CommandDirection ProcessNeuralRecomendation(BasicNetwork nn)
        {
            var input = new BasicMLData(2);
            NormalizedField _hStats = new NormalizedField(NormalizationAction.Normalize, "Heading", 359, 0, .9, -.9);
            input[0] = _neural.sim.DistanceToDestination;
            input[1] = _hStats.Normalize(_neural.sim.Heading);


            IMLData output = nn.Compute(input);
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
            return thrust;
        }

        private static void InitConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = "192.169.164.138",
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
            _Connection = factory.CreateConnection();
        }
        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Console.WriteLine("Topic Arrived: " + e.Topic);
            if (!e.Topic.Contains(_robot.SerialNumber)) return;
            switch (e.Topic.Split('/')[2])
            {
                case "ACK":
                    break;
                case "AUTH":
                    _machine.RaiseEvent(_state, _machine.ReadyForWorkEvent);
                    //TODO: Need to add Code to Subscribe to RobotJobs on RabbitMq
                    _messageQueue.GetNext = true;
                    break;
                case "FD":
                    break;
                case "LD":
                    break;
                case "RD":
                    break;
                case "ARV":
                    ProcessMovement();
                    break;
                case "LW":
                    break;
                case "STOP":
                    _machine.RaiseEvent(_state, _machine.ObstructionEvent);
                    break;
                case "RCLX":
                    break;
                case "RClY":
                    break;
                case "VR":
                    break;
            }
        }

        private void ProcessMovement()
        {
            Console.WriteLine("Process movement Called");
            if (!_neural.sim.Traveling)
            {
            }
            else
            {
                var thrust = ProcessNeuralRecomendation(_NeuralNet);
                _neural.sim.Turn(thrust);
                switch (thrust)
                {
                    case CommandDirection.TurnRight: //East
                        Console.WriteLine("move East");
                        Client.Publish(@"/" + _robot.SerialNumber , new byte[] {4});
                        break;
                    case CommandDirection.MoveInReverse: //south
                        Console.WriteLine("move South");
                        Client.Publish(@"/" + _robot.SerialNumber , new byte[] {3});
                        break;
                    case CommandDirection.MoveForward: //north
                        Console.WriteLine("move North");
                        Client.Publish(@"/" + _robot.SerialNumber , new byte[] {2});
                        break;
                    case CommandDirection.TurnLeft: //west
                        Console.WriteLine("move West");
                        Client.Publish(@"/" + _robot.SerialNumber , new byte[] {5});
                        break;
                }
            }
        }
    }
}