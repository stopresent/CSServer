using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
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
            this.playerId = (long)BitConverter.ToUInt64(new ReadOnlySpan<byte>(s.Array, s.Offset + count, s.Count - count));
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

    class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            ////byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server !");

            //Packet packet = new Packet() { size = 4, packetId = 7 };
            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            //byte[] buffer = BitConverter.GetBytes(packet.size);
            //byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
            //Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);

            //Send(sendBuff);

            Thread.Sleep(5000);

            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            int count = 0;
            int size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;
            int id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            switch ((PacketId)id)
            {
                case PacketId.PlayerInfoReq:
                    {
                        PlayerInfoReq p = new PlayerInfoReq();
                        p.Read(buffer);

                        count += 8;
                        Console.WriteLine($"PlayerInfoReq : {p.playerId}");
                    }
                    break;
                case PacketId.PlayerInfoOk:
                    break;
                default:
                    break;
            }

            Console.WriteLine($"Packet id : {id}, Packet size : {size}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }
}
