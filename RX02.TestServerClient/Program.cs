using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RX02.TestServerClient
{
    class Program
    {
        public static string cLock="";
        static void Main(string[] args)
        {
            
            for (int x = 1; x != 50; x++)
            {
                Thread thread = new Thread(new ThreadStart(WorkThreadFunction));
                thread.Start();
                lock (cLock)
                {
                    Console.WriteLine("Starting: " + x.ToString());
                }
            }
            Console.ReadLine();
        }

        public static void WorkThreadFunction()
        {
            try
            {
                var client = new Client();
                client.ConnectToServer();

                client.TransmitTestData();
            }
            catch (Exception ex)
            {
                // log errors
            }
        }
    }

    public class Client
    {
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        NetworkStream serverStream;
        public void ConnectToServer()
        {
            clientSocket.Connect("localhost", 23);

        }

        public void TransmitTestData()
        {
            for (int x = 0; x != 10000; x++)
            {
                
                SendData(x.ToString());
                Thread.Sleep(100);                
            }
            CloseConnection();
        }

        public void SendData(string dataTosend)
        {
            if (string.IsNullOrEmpty(dataTosend))
                return;
            NetworkStream serverStream = clientSocket.GetStream();
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(dataTosend);
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
        }

        public void CloseConnection()
        {
            clientSocket.Close();
        }
        public string ReceiveData()
        {
            StringBuilder message = new StringBuilder();
            NetworkStream serverStream = clientSocket.GetStream();
            serverStream.ReadTimeout = 100;
            //the loop should continue until no dataavailable to read and message string is filled.
            //if data is not available and message is empty then the loop should continue, until
            //data is available and message is filled.
            while (true)
            {
                if (serverStream.DataAvailable)
                {
                    int read = serverStream.ReadByte();
                    if (read > 0)
                        message.Append((char)read);
                    else
                        break;
                }
                else if (message.ToString().Length > 0)
                    break;
            }
            return message.ToString();
        }
    }
}
