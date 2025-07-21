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

        static void FlushRoom()
        {
            Room.Push(() => Room.Flush());
            JobTimer.Instance.Push(FlushRoom, 250); // 250ms 후에 다시 실행
        }

        static void Main(string[] args)
        {
            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listner.Init(endPoint, () => { return SessionManager.Instance.Generate(); });

            JobTimer.Instance.Push(FlushRoom); // 초기 작업 예약

            while (true)
            {
                JobTimer.Instance.Flush(); // JobTimer를 통해 예약된 작업을 실행
            }

        }
    }

}