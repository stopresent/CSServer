using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class GameRoom : IJobQueue
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        JobQueue _jobQueue = new JobQueue();

        public void Enter(ClientSession session)
        {
            _sessions.Add(session);
            session.Room = this;
            Console.WriteLine($"Enter : {session.SessionId}");
        }

        public void Leave(ClientSession session)
        {
            _sessions.Remove(session);
            Console.WriteLine($"Leave : {session.SessionId}");
        }

        public void Broadcast(ClientSession session, string chat)
        {
            S_Chat chatPacket = new S_Chat();
            chatPacket.playerId = session.SessionId;
            chatPacket.chat = $"{chat} I am {session.SessionId}";
            ArraySegment<byte> segment = chatPacket.Write();

            foreach (ClientSession s in _sessions)
            {
                s.Send(segment);
            }
        }

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }
    }
}
