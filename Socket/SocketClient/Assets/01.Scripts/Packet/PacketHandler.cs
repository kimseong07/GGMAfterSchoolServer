using DummyClient;
using ServerCore;
using System;
using UnityEngine;

class PacketHandler
{
    public static void BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        //여기는 그냥 에러 없애주는 용도. 진짜는 유니티에서 코딩한다.
        BroadcastEnterGame broadEnter = packet as BroadcastEnterGame;
        ServerSession serverSession = session as ServerSession;
        PlayerManager.Instance.Enter(broadEnter);
    }

    public static void BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        //여기는 그냥 에러 없애주는 용도. 진짜는 유니티에서 코딩한다.
        BroadcastLeaveGame broadLeave = packet as BroadcastLeaveGame;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.Leave(broadLeave);
    }

    public static void PlayerListHandler(PacketSession session, IPacket packet)
    {
        //여기는 그냥 에러 없애주는 용도. 진짜는 유니티에서 코딩한다.
        PlayerList pList = packet as PlayerList;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.Add(pList);
    }

    public static void BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        //여기는 그냥 에러 없애주는 용도. 진짜는 유니티에서 코딩한다.
        BroadcastMove move = packet as BroadcastMove;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.Move(move);
    }

    public static void BroadCastBFireHandler(PacketSession session, IPacket packet)
    {
        BroadCastBFire firePacket = packet as BroadCastBFire;
        CannonManager.Instance.ReceiveFire(firePacket);
    }

    public static void BroadcastBMoveHandler(PacketSession session, IPacket packet)
    {
        BroadcastBMove bMovePacket = packet as BroadcastBMove;
        CannonManager.Instance.BallMove(bMovePacket);
    }

    public static void BallDestroyHandler(PacketSession session, IPacket packet)
    {
        BallDestroy ballDestroyPacket = packet as BallDestroy;
        CannonManager.Instance.BallDestroy(ballDestroyPacket);
    }

    public static void BallListHandler(PacketSession session, IPacket packet)
    {
        //아직 안함 이건 니네가 해봐라
    }
}