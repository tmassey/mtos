using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using aXon.Rover;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace aXon.RX02.ControlServer
{
    public class ControllerStartup
    {
        public static string CLock = "";
        private static readonly MongoDataService ds = new MongoDataService();
        public static MqttClient Client;
        public static List<RobotManager> Robots { get; set; }

        public static void Main()
        {
            Console.WriteLine("aXon Robotics Server Version 1.0");
            Console.WriteLine("Processing Startup");
            bool connected = false;
            while (connected == false)
            {
                try
                {
                    Console.WriteLine("Connecting to MQTT Server!");
                    Client = new MqttClient(IPAddress.Parse("192.168.1.19"));
                    byte code = Client.Connect("aXon");
                    Client.Subscribe(new[] {"/RXAUTH", "/testRX"},
                                     new[] {MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE});
                    Client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
                    connected = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());                    
                }
            }
            Console.ReadLine();
        }


        private static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (e.Topic == "/RXAUTH")
            {
                string newSerial = Encoding.UTF8.GetString(e.Message);
                var robot = new RobotManager(newSerial);
                if (Robots == null)
                    Robots = new List<RobotManager>();
                Robots.Add(robot);
            }

            Console.WriteLine(e.Topic + " : " + Encoding.UTF8.GetString(e.Message));
        }
    }
}