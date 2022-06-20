using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DummyClient
{

    class Program
    {
        static void Main(string[] args)
        {
            //PacketManager.Instance.Register();
            //접속을 위한 엔드포인트 설정
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 54000);

            Connector connector = new Connector();

            connector.Connect(endPoint, () => SessionManager.Instance.Generate() , 5);

            while(true)
            {
                try
                {
                    SessionManager.Instance.SendForEach("Hello Server, This is Client");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                Thread.Sleep(250);  //일반적으로 MMO에서 이동패킷은 1초에 4번정도 쏜다.

            }
            
        }
    }
}
