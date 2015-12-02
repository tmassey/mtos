using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using aXon.Rover;
using aXon.Rover.Enumerations;
using aXon.Rover.Models;
using aXon.Warehouse.Modules.Robotics.Robot.Models;
using Encog.Neural.Networks;
using Encog.Persist;
using MongoDB.Driver.Builders;

namespace aXon.RX02.ControlServer
{
    public class HandleClientRequest
    {
        private readonly TcpClient _clientSocket;
        private NetworkStream _networkStream;
        public MongoDataService Mds { get; set; }
        public  Position SourceLocation { get; set; }
        public  Position DestLocation { get; set; }
        public Robot Rover { get; set; }
        public HandleClientRequest(TcpClient clientConnected)
        {
            _clientSocket = clientConnected;
            Mds= new MongoDataService();
        }

        public void StartClient()
        {
            _networkStream = _clientSocket.GetStream();

            WaitForRequest();
        }

        public void WaitForRequest()
        {
            var buffer = new byte[1];
            SendByte(0x02);
            _networkStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
        }

        public void SendByte(byte val)
        {
            var sendBytes = new byte[1] {val};
            _networkStream.Write(sendBytes, 0, sendBytes.Length);
            //_networkStream.Flush();
        }

        public void SendByte(byte[] val)
        {
            var sendBytes = val;
            _networkStream.Write(sendBytes, 0, sendBytes.Length);
            //_networkStream.Flush();
        }
        public void RunNetwork(Guid id)
        {
           
            BasicNetwork network = null;

            var fn = id.ToString();
            var net = Mds.GetCollectionQueryModel<NeuralNetwork>(Query.EQ("_id", id)).FirstOrDefault();
            if (net != null)
            {
                SourceLocation = new Position(net.StartPosition.X, net.StartPosition.Y);
                DestLocation = new Position(net.EndPosition.X, net.EndPosition.Y);

                lock (RobotContol.NetworkLock)
                {
                    var rawbytes = Mds.OpenFile(id);

                    File.WriteAllBytes(fn, rawbytes);
                    network = (BasicNetwork)EncogDirectoryPersistence.LoadObject(new FileInfo(fn));
                }
                var pilot = new NeuralRobot(network, true, SourceLocation, DestLocation);
                //SourceData.Simulation = pilot.sim;                           
                pilot.ScorePilot();
                File.Delete(fn);

            }            
        }
        private void ReadCallback(IAsyncResult result)
        {
            var networkStream = _clientSocket.GetStream();
            try
            {
                var read = networkStream.EndRead(result);
                if (read == 0)
                {
                    _networkStream.Close();
                    _clientSocket.Close();
                    return;
                }

                var buffer = result.AsyncState as byte[];
                var data = Encoding.Default.GetString(buffer, 0, read);
                //lock (ControllerStartup.cLock)
                //{
                //    Console.WriteLine(data);
                //}

               
                switch (buffer[0])
                {
                    case 0://Undefined
                        break;
                    case 1://ACK
                        break;
                    case 3://Auth Response known serial
                        break;
                    case 4: //Auth Response Request Serial Number
                        Rover = new Robot
                        {
                            Id = Guid.NewGuid(),
                            SerialNumber = Mds.GetCollectionQueryModel<Robot>().Count + 1,
                            CurrentLocation = Mds.GetCollectionQueryModel<Rover.Models.Warehouse>()
                                .FirstOrDefault()
                                .Positions.FirstOrDefault(p => p.MapMode == MapMode.ChargeMode),
                            CurrentMode = RoverMode.WaitingForJob,
                            CreateDateTime = DateTime.Now,
                            LastEditDateTime = DateTime.Now,
                            CreatedBy = new Guid()
                        };
                        Rover.ModifiedBy = Rover.CreatedBy;
                        Mds.DataBase.GetCollection(typeof (Robot), "Robot").Save(Rover);
                        SendByte(5);
                        SendByte(Convert.ToByte(Rover.SerialNumber));
                        SendByte(Convert.ToByte(Rover.CurrentLocation.X));
                        SendByte(Convert.ToByte(Rover.CurrentLocation.Y));
                        break;
                    case 5: //
                    case 6:
                        BackDistanceReceive();
                        break;
                    case 7:
                        FrontDistanceReceive();
                        break;
                    case 8:
                        LeftDistanceReceive();
                        break;
                    case 9:
                        RightDistanceReceive();
                        break;
                    case 14: //Arrived at Location What Next
                        break;
                    case 20://Lift Weight Retured
                        break;
                    case 21: //Emergency Stop by RX-0X
                        break;
                }              
            }
            catch (Exception ex)
            {
                //throw;
            }

            WaitForRequest();
        }

        private void RightDistanceReceive()
        {
            var fDist = _networkStream.ReadByte();
            lock (ControllerStartup.cLock)
            {
                Console.WriteLine("Right Distance: " + fDist);
            }
            Ack();
        }

        private void Ack()
        {
            SendByte(0x01);
        }

        private void LeftDistanceReceive()
        {
            var fDist = _networkStream.ReadByte();
            lock (ControllerStartup.cLock)
            {
                Console.WriteLine("Left Distance: " + fDist);
            }
            Ack();
        }

        private void FrontDistanceReceive()
        {
            var fDist = _networkStream.ReadByte();
            lock (ControllerStartup.cLock)
            {
                Console.WriteLine("Front Distance: " + fDist);
            }
            Ack();
            //if (fDist >= 200)
            //    SendByte(10);
        }

        private void BackDistanceReceive()
        {
            var backDist = _networkStream.ReadByte();
            lock (ControllerStartup.cLock)
            {
                Console.WriteLine("Back Distance: " + backDist);
            }
            Ack();
            //if (backDist >= 200)
            //    SendByte(11);
        }
    }
}