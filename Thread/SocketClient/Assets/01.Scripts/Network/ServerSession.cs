using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

namespace DummyClient
{

    class ServerSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisConnected : {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer, (s, p) => PacketQueue.Instance.Push(p) );
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transfered Byte : {numOfBytes}");
            //세션이 많아지면서 여기가 빈번하게 일어날테니 잠시 여기는 주석처리
        }
    }
}

