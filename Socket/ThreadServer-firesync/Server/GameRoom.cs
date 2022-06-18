using ServerCore;
using System;
using System.Collections.Generic;

namespace Server
{
    class GameRoom : IJobQueue
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        //딕셔너리로 해서 아이디랑 세션을 저장해도 돼(이게 속도는 빨라)
        JobQueue _jobQueue = new JobQueue();

        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        

        Random rand = new Random();
        Dictionary<int, CannonBall> _balls = new Dictionary<int, CannonBall>();

        private int cannonId = 0;

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Enter(ClientSession session)
        {
            _sessions.Add(session);
            session.Room = this;

            // 신규로 들어온 플레이어에게 기존 플레이어 리스트를 줘야 한다.
            PlayerList list = new PlayerList();
            _sessions.ForEach(s =>
            {
                list.players.Add(new PlayerList.Player()
                {
                    isSelf = (s.SessionId == session.SessionId),
                    playerId = s.SessionId,
                    posX = s.PosX,
                    posY = s.PosY,
                    posZ = s.PosZ,
                    color = s.color
                });
            });
            session.Send(list.Write());

            // 다른 모두에겐 새로 들어온 플레이어를 알려준다.
            //시작 위치는 0,0,0으로 셋팅
            BroadcastEnterGame enter = new BroadcastEnterGame();
            enter.playerId = session.SessionId;
            enter.posX = 0;
            enter.posY = 0;
            enter.posZ = 0;
            enter.color = session.color;

            BroadCast(enter.Write());
        }

        public void Leave(ClientSession session)
        {
            _sessions.Remove(session);

            //모두에게 특정유저가 나갔음을 알려야 한다.
            BroadcastLeaveGame leave = new BroadcastLeaveGame();
            leave.playerId = session.SessionId;
            BroadCast(leave.Write());
        }

        public void BroadCast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);       
        }

        public void Flush()
        {
            _sessions.ForEach (x => {
                x.Send(_pendingList);
            });

            _pendingList.Clear();
        }

        public void Move(ClientSession session, Move packet)
        {
            //좌표를 바꿔주고
            session.PosX = packet.posX;
            session.PosY = packet.posY;
            session.PosZ = packet.posZ;

            //모두에게 이를 알려준다.

            BroadcastMove bMove = new BroadcastMove() { playerId = session.SessionId, posX = session.PosX, posY = session.PosY, posZ = session.PosZ };

            BroadCast(bMove.Write());
        }

        public void FireCannon(ClientSession session, BFire packet)
        {
            CannonBall ball = new CannonBall();
            ball.owner = session.SessionId;
            ball.ballId = ++cannonId;
            ball.posX = packet.posX;
            ball.posY = packet.posY;
            ball.posZ = packet.posZ;

            _balls.Add(ball.ballId, ball);

            BroadCastBFire broadBFire = new BroadCastBFire()
            {
                owner = session.SessionId,
                ballId = ball.ballId,
                posX = ball.posX,
                posY = ball.posY,
                posZ = ball.posZ
            };

            BroadCast(broadBFire.Write());
        }

        public void MoveCannonBall(ClientSession session, BMove packet)
        {
            //여기서 다른 클라이언트와의 충돌, 좌표의 z가 0보다 떨어질 경우 삭제 등의 행동을 해줘야 한다.
            CannonBall ball = null;
            if(_balls.TryGetValue(packet.ballId, out ball))
            {
                ball.posX = packet.posX;
                ball.posY = packet.posY;
                ball.posZ = packet.posZ;

                //볼이 파괴된 경우
                if(ball.posY <= 0.5f)
                {
                    Console.WriteLine($"{ball.ballId} 파괴됨" );
                    BallDestroy bDestroy = new BallDestroy() {
                        ballId = ball.ballId,
                        posX = ball.posX,
                        posY = ball.posY,
                        posZ = ball.posZ
                    };
                    BroadCast(bDestroy.Write());
                }
                else
                {
                    //여기서 적과 충돌했는지 검사해야함.

                    BroadcastBMove broadBMove = new BroadcastBMove()
                    {
                        ballId = ball.ballId,
                        posX = ball.posX,
                        posY = ball.posY,
                        posZ = ball.posZ
                    };
                    BroadCast(broadBMove.Write());
                }
            }
        }
    }
}

