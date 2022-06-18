using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    public MyPlayer myPlayer;
    Dictionary<int, Player> _players = new Dictionary<int, Player>();

    public static PlayerManager Instance { get; } = new PlayerManager(); //√÷√  ΩÃ±€≈Ê¿∏∑Œ ∏∏µÈæÓµ–¥Ÿ

    public void Add(PlayerList listPacket)
    {
        Object obj = Resources.Load("Player");

        foreach (PlayerList.Player p in listPacket.players)
        {
            GameObject go = Object.Instantiate(obj) as GameObject;

            if(p.isSelf)
            {
                myPlayer = go.AddComponent<MyPlayer>();
                myPlayer.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                myPlayer.PlayerId = p.playerId;
                myPlayer.SetColor(p.color);
                myPlayer.SetDestination(myPlayer.transform.position);
                myPlayer.gameObject.tag = "Player";
                CameraManager.Instance.SetMyPlayer(myPlayer.transform);
            }
            else
            {
                Player player = go.AddComponent<Player>();
                player.PlayerId = p.playerId;
                player.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                player.SetColor(p.color);
                _players.Add(p.playerId, player);
               
            }
            
        }
    }

    public void Enter(BroadcastEnterGame packet)
    {
        if (packet.playerId == myPlayer.PlayerId) return; //Ω∫≈µ

        Object obj = Resources.Load("Player");
        GameObject go = Object.Instantiate(obj) as GameObject;

        Player player = go.AddComponent<Player>();
        player.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
        player.SetColor(packet.color);
        _players.Add(packet.playerId, player);
    }

    public void Leave(BroadcastLeaveGame packet)
    {
        if(myPlayer.PlayerId == packet.playerId)
        {
            GameObject.Destroy(myPlayer.gameObject);
            myPlayer = null;
        }
        else
        {
            Player player = null;
            if(_players.TryGetValue(packet.playerId, out player))
            {
                GameObject.Destroy(player.gameObject);
                _players.Remove(packet.playerId);
            }
        }
    }

    public void Move(BroadcastMove packet)
    {
        if (myPlayer.PlayerId == packet.playerId)
        {
            //_myPlayer.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
        }
        else
        {
            Player player = null;
            if(_players.TryGetValue(packet.playerId, out player))
            {
                //player.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
                player.SetDestination(new Vector3(packet.posX, packet.posY, packet.posZ));
            }
        }
    }
}
