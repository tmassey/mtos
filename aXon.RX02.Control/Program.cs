using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace aXon.RX02.Control
{
    /// <summary>
    /// This application Will utalize the workers and the neural networks though
    /// a connect established by the Control Server. the control server will execute 
    /// an instance of this application and bind the IO streams to the connection allowing the RX-02 
    /// Robots to maintin a constant stream to the Super Computer though the commands accesed in this application. 
    /// This application will provide the primary logic to authenticate and maintin the robotics control systems.
    /// </summary>
    class Program
    {
        public enum Outboundcommands
        {
            Authentication = 0,

        }

        public enum InboundCommandModes
        {
            Authentication=0,
        }

        static void Main(string[] args)
        {
           Console.Write(Outboundcommands.Authentication);
            var auth1 = Console.Read();
            while (auth1 == 160)
                auth1 = Console.Read();
            Console.Write(auth1);
            while (true)
            {
                Thread.Sleep(10);
            }
        }
    }
}
