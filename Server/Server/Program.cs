using ServerCore;
using System;
using System.Net;
using System.Text;

namespace Server
{
    class Program
    {
        static Listener _listner = new Listener();
        public static GameRoom Room = new GameRoom();
        static void Main(string[] args)
        {
            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listner.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            
            while (true)
            {
                ;
            }

        }
    }

}