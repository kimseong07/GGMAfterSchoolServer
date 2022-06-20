using DummyClient;
using ServerCore;
using System;

class PacketHandler
{ 
    public static void BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        //여기는 그냥 에러 없애주는 용도. 진짜는 유니티에서 코딩한다.
        BroadcastEnterGame broadEnter = packet as BroadcastEnterGame;
        ServerSession serverSession = session as ServerSession;

    }

    public static void BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        //여기는 그냥 에러 없애주는 용도. 진짜는 유니티에서 코딩한다.
        BroadcastLeaveGame broadLeave = packet as BroadcastLeaveGame;
        ServerSession serverSession = session as ServerSession;
    }

    public static void PlayerListHandler(PacketSession session, IPacket packet)
    {
        //여기는 그냥 에러 없애주는 용도. 진짜는 유니티에서 코딩한다.
        PlayerList pList = packet as PlayerList;
        ServerSession serverSession = session as ServerSession;
    }

    public static void BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        //여기는 그냥 에러 없애주는 용도. 진짜는 유니티에서 코딩한다.
        BroadcastMove move = packet as BroadcastMove;
        ServerSession serverSession = session as ServerSession;
    }
}