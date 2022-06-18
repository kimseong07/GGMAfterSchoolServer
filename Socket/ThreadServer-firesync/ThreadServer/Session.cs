using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;
        //sealed를 붙이면 다른 클래스가 OnRecv를 override 하면 안된다. 
        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            //여기서 먼저 처음에 온 2바이트를 까서 size가 몇인지를 파악해서 받는다.
            int processLen = 0;

            int packetCount = 0;
            while(true)
            {
                if(buffer.Count < HeaderSize)  //2보다 작으면 사이즈를 읽을 헤더도 안온거니까 다음번에 다시 읽어
                {
                    break;
                }

                //만약 2바이트는 왔다면 다음 패킷은 완전체로 온건지 확인
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                //UsingedInt16비트면 2바이트다.

                //아직 전체 패킷이 온건 아니니까 멈춰
                if(buffer.Count < dataSize)
                {
                    break;
                }

                processLen += dataSize;
                //한개 패킷의 공간을 찝어서 OnRecvPacket로 보내준다.
                //ArraySegment는 struct에요. Heap할당 안됩니다. 찍어내도 되요.
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
                packetCount++;
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);

            }

            if(packetCount > 1)
            {
                Console.WriteLine($"패킷 모아 보냈음 : {packetCount}");
            }

            return processLen; //내가 처리한 바이트수를 리턴한다.
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }

    public abstract class Session
    {
        Socket _socket;
        
        int _disconnected = 0;

        RecvBuffer _recvBuffer = new RecvBuffer(65535);

        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();

        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>(); // 전송을 한꺼번에 하기 위한 리스트
        //배열의 일부를 만들어주는 어레이 세그먼트를 객체 (어떤 배열의 일부만을 가져오는 형태로 Stack에 할당되는 녀석이다. 값을 복사해옴)

        //bool _pending = false; // 현재 펜딩되고 있는가? // 필요없어짐

        object _lock = new object(); //락킹용 오브젝트

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);

        public void Init(Socket socket)
        {
            _socket = socket;
            SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
            //데이터가 다 받아지면 OnReceiveComplete를 실행하고
            receiveArgs.Completed += OnReceiveCompleted;
            
            //이제 버퍼는 삭제

            RegisterReceive(receiveArgs); //대기 시작

            _sendArgs.Completed += OnSendCompleted;
        }
        public void Send(List<ArraySegment<byte>> sendBufferList)
        {
            //우리가 지금 만드는 사례에선 없지만 250ms 동안 아무런 패킷이 없었다면 이런일이 발생해.
            // 그럼 전송오류 나면서 애들이 튕겨
            if (sendBufferList.Count == 0)
                return;

            lock(_lock)
            {
                sendBufferList.ForEach(x => _sendQueue.Enqueue(x));

                if (_pendingList.Count == 0)   //대기중인 애가 없다면 바로 RegisterSend를 호출하도록
                {
                    RegisterSend();
                }
            }
        }
        
        public void Send(ArraySegment<byte> sendBuffer)
        {
            lock (_lock)
            {
                //데이터 전송할 것들을 큐에 쌓아두고 보낸다.
                _sendQueue.Enqueue(sendBuffer);

                //if (!_pending)
                if (_pendingList.Count == 0)   //대기중인 애가 없다면 바로 RegisterSend를 호출하도록
                {
                    RegisterSend();
                }
            }
            
        }

        public void Disconnect()
        {

            if(Interlocked.Exchange(ref _disconnected, 1) == 1)
            {
                return;
            }

            OnDisconnected(_socket.RemoteEndPoint); //여기서 인터페이스 호출
            //종료시키는 것으로 코딩
            _socket.Shutdown(SocketShutdown.Both);
            //더이상 듣지 않을거고 보내지도 않을꺼라 예고
            _socket.Close();
            Clear();
        }

        public void Clear()
        {
            lock(_lock)
            {
                _sendQueue.Clear();
                _pendingList.Clear();
            }
        }

        #region 네트워크 통신

        void RegisterSend()
        {
            if (_disconnected == 1)
                return;

            //한번에 보내는거. 다만 주의사항은 리스트와 버퍼를 둘 다 쓰면 안된다는 것.
            while(_sendQueue.Count >0 )
            {
                ArraySegment<byte> buffer = _sendQueue.Dequeue();
                //_sendArgs.BufferList.Add(new ArraySegment<byte>(buffer, 0, buffer.Length));  => 이런짓은 하면 안돼
                // 애초에 BufferList가 그렇게 만들어진 녀석이기 때문이다
                _pendingList.Add(buffer);
            }
            _sendArgs.BufferList = _pendingList;

            try
            {
                bool pending = _socket.SendAsync(_sendArgs);
                if (!pending)
                {
                    OnSendCompleted(null, _sendArgs);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"RegisterSend Failed {e}");
            }
            
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            //RegisterSend가 실행되는 건 Send 에서만 실행되며 Send는 이미 lock으로 보호되고 있지만
            //OnSendCompleted의 경우 다른 쓰레드에서 실행할 수 도 있으며 이때는 lock으로 보호되지 않을 수 있다.
            //따라서 이부분은 lock으로 묶어주어야 한다
            lock(_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendingList.Clear(); //다 보내졌으면

                        OnSend(_sendArgs.BytesTransferred);

                        if (_sendQueue.Count > 0)
                        {
                            RegisterSend();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"On Send Completed Filed {e}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }

        void RegisterReceive(SocketAsyncEventArgs args)
        {
            if (_disconnected == 1)
                return;

            _recvBuffer.Clean();
            ArraySegment<byte> segment = _recvBuffer.WriteSegment;
            args.SetBuffer(segment.Array, segment.Offset, segment.Count);

            try
            {
                bool pending = _socket.ReceiveAsync(args);
                if (!pending)
                {
                    OnReceiveCompleted(null, args);
                }
            }catch(Exception e)
            {
                Console.WriteLine($"RegisterRecv Failed {e}");
            }
            
        }

        void OnReceiveCompleted(object sender, SocketAsyncEventArgs args)
        {
            //전송된 데이터가 0보다 큰지. 연결을 끊을경우 바이트 0으로 오기도 함
            if(args.BytesTransferred > 0)
            {
                try
                {
                    //WriteCursor를 이동한다.
                    if(_recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }

                    //컨텐츠로 데이터를 넘겨주고 얼마나 처리했는지 받자
                    //상속을 통해 받았을때 일을 지정해야 한다.
                    //OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));
                    int processLen = OnRecv(_recvBuffer.ReadSegment);
                    if(processLen < 0 || _recvBuffer.DataSize < processLen)
                    { //문제가 되는 상황
                        Disconnect();
                        return;
                    }

                    if(_recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }
                    
                    RegisterReceive(args); //다시 받기 대기
                }catch (Exception e)
                {
                    Console.WriteLine($"OnReceive Failed : {e}");
                }
            }
            else
            {
                //이경우는 이 소켓을 종료시켜야 할경우다.
                Disconnect();
            }
        }

        #endregion
    }
}
