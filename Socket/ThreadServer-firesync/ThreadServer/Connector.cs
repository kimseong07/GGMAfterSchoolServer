using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Connector
    {
        Func<Session> _sessionFactory;

        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, int count = 1)
        {
            for(int i = 0; i < count; i++)
            {
                _sessionFactory = sessionFactory;
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += OnConnectCompleted;
                args.RemoteEndPoint = endPoint;
                //유저 토큰을 이용해서 소켓을 넘기자
                args.UserToken = socket;

                RegisterConnect(args);
            }
            
        }

        private void RegisterConnect(SocketAsyncEventArgs args)
        {
            //socket
            Socket socket = (Socket)args.UserToken;
            if (socket == null) return;

            bool pending = socket.ConnectAsync(args);
            if (pending == false)
                OnConnectCompleted(null, args);

        }

        private void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory();
                session.Init(args.ConnectSocket);
                session.OnConnected(args.RemoteEndPoint);
            }else
            {
                Console.WriteLine($"OnConnectionCompleted Fail : {args.SocketError}");
            }
        }
    }
}
