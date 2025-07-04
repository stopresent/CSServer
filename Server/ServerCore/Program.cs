using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        static Listener _listner = new Listener();

        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                Session session = new Session();
                session.Start(clientSocket);

                // 받는다
                
                // 보낸다
                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server !");
                session.Send(sendBuff);

                Thread.Sleep(1000);

                session.Disconnect();
                session.Disconnect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {
            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listner.Init(endPoint, OnAcceptHandler);
            Console.WriteLine("Listening...");

            while (true)
            {
                ;
            }

        }
    }
}