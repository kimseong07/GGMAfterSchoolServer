using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager
{
    #region Singleton
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance
    {
        get { return _instance;}
    }
    #endregion

    //생성자에서 실행하도록 
    private PacketManager()
    {
        Register();
    }

    //받은 패킷을 리딩할 핸들러를 지정하는 딕셔너리
    Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket >> _makeFunc 
        = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
    //패킷번호에 따른 핸들러를 등록하는 딕셔너리
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler
        = new Dictionary<ushort, Action<PacketSession, IPacket>>();

    public void Register()
    {
        
        _makeFunc.Add((ushort)PacketID.BroadcastEnterGame, MakePacket<BroadcastEnterGame>);
        _handler.Add((ushort)PacketID.BroadcastEnterGame, PacketHandler.BroadcastEnterGameHandler);


        _makeFunc.Add((ushort)PacketID.BroadcastLeaveGame, MakePacket<BroadcastLeaveGame>);
        _handler.Add((ushort)PacketID.BroadcastLeaveGame, PacketHandler.BroadcastLeaveGameHandler);


        _makeFunc.Add((ushort)PacketID.PlayerList, MakePacket<PlayerList>);
        _handler.Add((ushort)PacketID.PlayerList, PacketHandler.PlayerListHandler);


        _makeFunc.Add((ushort)PacketID.BroadcastMove, MakePacket<BroadcastMove>);
        _handler.Add((ushort)PacketID.BroadcastMove, PacketHandler.BroadcastMoveHandler);


        _makeFunc.Add((ushort)PacketID.BroadCastBFire, MakePacket<BroadCastBFire>);
        _handler.Add((ushort)PacketID.BroadCastBFire, PacketHandler.BroadCastBFireHandler);


        _makeFunc.Add((ushort)PacketID.BallList, MakePacket<BallList>);
        _handler.Add((ushort)PacketID.BallList, PacketHandler.BallListHandler);


        _makeFunc.Add((ushort)PacketID.BroadcastBMove, MakePacket<BroadcastBMove>);
        _handler.Add((ushort)PacketID.BroadcastBMove, PacketHandler.BroadcastBMoveHandler);


        _makeFunc.Add((ushort)PacketID.BallDestroy, MakePacket<BallDestroy>);
        _handler.Add((ushort)PacketID.BallDestroy, PacketHandler.BallDestroyHandler);


    }

    public void OnRecvPacket (PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallBack = null)
    {
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
        if(_makeFunc.TryGetValue(id, out func))
        {
            IPacket packet = func(session, buffer);
            
            if(onRecvCallBack != null)
            {
                onRecvCallBack(session, packet);
            }
            else
            {
                HandlePacket(session, packet);
            }
        }
    }

    //IPacket 을 구현했으면서 new가 가능한 녀석이어야 한다.
    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T packet = new T();
        packet.Read(buffer);

        return packet;
    }

    public void HandlePacket(PacketSession session, IPacket packet)
    {
        Action<PacketSession, IPacket> action = null;

        if (_handler.TryGetValue(packet.Protocol, out action))
        {
            action(session, packet);
        }
    }
}