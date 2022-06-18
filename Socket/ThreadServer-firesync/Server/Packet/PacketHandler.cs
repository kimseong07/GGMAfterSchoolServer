using Server;
using ServerCore;
using System;

class PacketHandler
{
    public static void LeaveGameHandler(PacketSession session, IPacket packet)
    {

        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.Leave(clientSession));

    }

    public static void MoveHandler(PacketSession session, IPacket packet)
    {
        Move movePacket = packet as Move;

        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        //Console.WriteLine($"{movePacket.posX}, {movePacket.posY}, {movePacket.posZ}");

        GameRoom room = clientSession.Room;
        room.Push(() => room.Move(clientSession, movePacket) );

    }

    public static void BFireHandler(PacketSession session, IPacket packet)
    {
        BFire firePacket = packet as BFire;

        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;
        GameRoom room = clientSession.Room;
        room.Push(() => room.FireCannon(clientSession, firePacket));
    }

    public static void BMoveHandler(PacketSession session, IPacket packet)
    {
        BMove bMovePakcet = packet as BMove;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;
        GameRoom room = clientSession.Room;
        room.Push(() => room.MoveCannonBall(clientSession, bMovePakcet));
    }
}