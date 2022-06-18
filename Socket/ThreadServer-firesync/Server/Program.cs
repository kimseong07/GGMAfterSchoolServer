using System;
using System.Net;
using ServerCore;

namespace Server
{

    
    class Program
    {
        static Listener _listener = new Listener(); //리스너 만들어주고
        public static GameRoom Room = new GameRoom();

        static void FlsuhRoom()
        {
            Room.Push(() => Room.Flush());
            JobTimer.Instance.Push(FlsuhRoom, 250);
        }

        static void Main(string[] args)
        {
            //PacketManager.Instance.Register();

            //입장을 담당할 리스너를 만들어야 한다

            //첫번째 인자는 네트워크 주소 
            string host = Dns.GetHostName(); //이 호스트의 이름 (아마 로컬호스트)
            Console.WriteLine(host);
            IPHostEntry ipHost = Dns.GetHostEntry(host); //해당 호스트의 ip리스트들을 반납한다.
            //ipHost.AddressList 이게 그 배열리스트 아마 여기선 localhost라서 127.0.0.1을 낼듯
            IPAddress ipAddr = ipHost.AddressList[0];
            Console.WriteLine(ipAddr.ToString());

            IPEndPoint endPoint = new IPEndPoint(ipAddr, 54000); //54000 포트로 해당 아이피로 서버

            //TCP와 Stream은 셋트여

            _listener.Init(endPoint, () => SessionManager.Instance.Generate() );
            Console.WriteLine("Listening...");


            FlsuhRoom();

            while (true)
            {
                JobTimer.Instance.Flush(); //타이머가 할일 이 있다면 할거고 아니면 안할거고
            }
        }        
    }
}
