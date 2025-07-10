using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class GameRoom
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        object _lock = new object();

        public void Enter(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Add(session);
                session.Room = this;
                Console.WriteLine($"Enter : {session.SessionId}");
            }
        }

        public void Leave(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session);
                Console.WriteLine($"Leave : {session.SessionId}");
            }
        }

        public void Broadcast(ClientSession session, string chat)
        {
            S_Chat chatPacket = new S_Chat();
            chatPacket.playerId = session.SessionId;
            chatPacket.chat = $"{chat} I am {session.SessionId}";
            ArraySegment<byte> segment = chatPacket.Write();

            lock (_lock)
            {
                foreach (ClientSession s in _sessions)
                {
                    s.Send(segment);
                }
            }
        }

    }
}
