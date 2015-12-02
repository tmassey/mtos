using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace aXon.RX02.ControlServer
{
    public class ControllerStartup
    {
        private static TcpListener _listener;
        public static string cLock="";
        public static void Main()
        {
            StartServer();
            while (true)
            {
            }
        }

        public static void StartServer()
        {
            _listener = new TcpListener(IPAddress.Any,23);
            _listener.Start();
            WaitForClientConnect();
        }
        private static void WaitForClientConnect()
        {
            object obj = new object();
            _listener.BeginAcceptTcpClient(new System.AsyncCallback(OnClientConnect), obj);
        }
        private static void OnClientConnect(IAsyncResult asyn)
        {
            try
            {
                TcpClient clientSocket = default(TcpClient);
                clientSocket = _listener.EndAcceptTcpClient(asyn);
                HandleClientRequest clientReq = new HandleClientRequest(clientSocket);
                clientReq.StartClient();
            }
            catch (Exception se)
            {
                throw;
            }

            WaitForClientConnect();
        }
    }
}
