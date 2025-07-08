using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{
    public abstract class Packet
    {
        public ushort size;
        public ushort packetId;

        public abstract void Read(ArraySegment<byte> buffer);
        public abstract ArraySegment<byte> Write();
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;

        public PlayerInfoReq()
        {
            packetId = (ushort)PacketId.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> s)
        {
            ushort count = 0;
            //int size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;
            //int id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;
            this.playerId = (long)BitConverter.ToUInt64(new ReadOnlySpan<byte>(s.Array, s.Offset + count, s.Count - count)); // 패킷이 거짓으로 올때 처리용
            count += 8;
        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> s = SendBufferHelper.Open(4096); // s == openSegment

            bool success = true;
            ushort count = 0;
            //success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), packet.size);
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.packetId);
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.playerId);
            count += 8;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);

            if (success == false)
                return null;

            return SendBufferHelper.Close(count);

        }
    }

    public enum PacketId
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,
    }

    class ServerSession : Session
    {
        //static unsafe void ToByte(byte[] buffer, int offset, int value) // C++처럼 포인터 사용
        //{
        //    fixed (byte* p = &buffer[offset])
        //    {
        //        *(int*)p = value;
        //    }
        //}

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1001 };

            //for (int i = 0; i < 5; i++)
            {
                ArraySegment<byte> s = packet.Write();
                if (s != null)
                    Send(s);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }
}
