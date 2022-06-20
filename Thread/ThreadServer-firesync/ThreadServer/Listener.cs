using System;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Listener
    {
        Socket _listenSocket;
        //Action<Socket> _onAcceptHandler;
        Func<Session> _sessionFactory; //새션 생성

        //초기화 함수 지정
        public void Init(EndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.Bind(endPoint);  //주소, 포트를 소켓에 바인드 시켜서 들을 준비를 시킴
            
            _listenSocket.Listen(backlog); //대기 - 들어가는 숫자는 최대 대기수

            _sessionFactory += sessionFactory;

            for(int i = 0; i < register; i++)
            {
                //하나 만들어주면 계속 재사용 가능
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                //args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                args.Completed += OnAcceptCompleted;

                RegisterAccept(args); //최초 한번은 Accept하라고 등록해줌.
            }
        }

        private void RegisterAccept(SocketAsyncEventArgs args)
        {
            //기존통신에서 사용한 잔재는 깨끗하게 지워져야한다. 안그러면
            // 찌꺼기가 남게 된다.
            args.AcceptSocket = null;  //어셉트 소켓은 지워주고

            bool pending = _listenSocket.AcceptAsync(args); //비동기로 어셉트 한다. 
            if (!pending)
            {
                //pending이 false라는 이야기는 대기없이 바로 accept된거임 그럼 바로 여기서 처리해주면 돼
                OnAcceptCompleted(null, args); //성공호출 바로 해주면 된다.
            }
            else
            {

            }
        }

        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory();
                session.Init(args.AcceptSocket);

                session.OnConnected(args.RemoteEndPoint);
                //에러 없이 처리되었을 경우
                //_onAcceptHandler(args.AcceptSocket); //수락된 소켓을 뱉는다
            }
            else
            {
                //에러를 출력해보자
                Console.WriteLine(args.SocketError.ToString());
            }
            RegisterAccept(args); //다시 대기 . 스스로 계속 반복해서 이 작업을 하게 만든다.
            //이렇게 하면 Accept 매서드가 필요가 없어진다.
        }

    }
}
