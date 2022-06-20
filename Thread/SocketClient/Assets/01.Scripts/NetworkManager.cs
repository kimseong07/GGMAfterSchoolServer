using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    ServerSession _session = new ServerSession();

    private static NetworkManager _instance;

    public static NetworkManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null)
            Debug.LogError("다수의 네트워크 매니저가 실행중입니다.");
        _instance = this;
    }

    public void Send(ArraySegment<byte> sendBuff)
    {
        _session.Send(sendBuff);
    }

    void Start()
    {
        Screen.SetResolution(640, 480, false);
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 54000);

        Connector connector = new Connector();

        connector.Connect(endPoint, () => _session);

        
    }

    private void OnDestroy()
    {
        _session.Disconnect();
    }

    private void Update()
    {
        List<IPacket> list = PacketQueue.Instance.PopAll();
        
        foreach(IPacket p in list)
        {
            PacketManager.Instance.HandlePacket(_session, p);
        }
    }


    
}
