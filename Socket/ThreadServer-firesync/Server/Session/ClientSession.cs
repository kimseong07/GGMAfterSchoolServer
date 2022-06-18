using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Numerics;

namespace Server
{
    class ClientSession : PacketSession
    {
        public int SessionId { get; set; }
        public GameRoom Room { get; set; }

        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }

        public Vector3 color { get; set; }
        Random _rand = new Random();        

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");
            PosX = _rand.Next(-5, 5);
            PosZ = _rand.Next(-5, 5);
            PosY = 0.5f;
            color = new Vector3((float)_rand.NextDouble(), (float)_rand.NextDouble(), (float)_rand.NextDouble());
            Console.WriteLine($"Color is {color.X}, {color.Y}, {color.Z}");
            Program.Room.Push(() => Program.Room.Enter(this));
            //Program.Room.Enter(this);
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);

            if(Room != null)
            {
                GameRoom r = Room;
                Room.Push(() => r.Leave(this));
                Room = null;
            }

            Console.WriteLine($"OnDisConnected : {endPoint}");
        }


        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transfered Byte : {numOfBytes}");
        }
    }
}
