using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonManager
{
    //MyPlayer _myPlayer;
    Dictionary<int, CannonBall> _cannons = new Dictionary<int, CannonBall>();

    public static CannonManager Instance { get; } = new CannonManager(); //최초 싱글톤으로 만들어둔다

    //이걸 받는 시점은 최초 방에 들어갔을 때만 받는다. 따라서 내 포탄은 없다고 생각하면 돼.
    public void Add(BallList listPacket)
    {
        Object obj = Resources.Load("Ball");

        foreach (BallList.Ball b in listPacket.balls)
        {
            GameObject go = Object.Instantiate(obj) as GameObject;


            CannonBall ball = go.GetComponent<CannonBall>();
            ball.CopyBallData(b);
            _cannons.Add(b.ballId, ball);
        }
    }

    public void ReceiveFire(BroadCastBFire packet)
    {
        Object obj = Resources.Load("Ball");
        if (packet.owner == PlayerManager.Instance.myPlayer.PlayerId)
        {
            GameObject go = Object.Instantiate(obj) as GameObject;
            CannonBall ball = go.GetComponent<CannonBall>();
            ball.ballId = packet.ballId;
            PlayerManager.Instance.myPlayer.FireCannon(ball);
            _cannons.Add(packet.ballId, ball);
        }
        else
        {
            GameObject go = Object.Instantiate(obj) as GameObject;
            CannonBall ball = go.GetComponent<CannonBall>();

            ball.owner = packet.owner;
            ball.ballId = packet.ballId;
            Vector3 pos = new Vector3(packet.posX, packet.posY, packet.posZ);
            ball.transform.position = pos;
            ball.SetDestination(pos);
            _cannons.Add(packet.ballId, ball);
        }
    }

    public void BallDestroy(BallDestroy packet)
    {
        CannonBall ball = null;
        if(_cannons.TryGetValue(packet.ballId, out ball))
        {
            GameObject.Destroy(ball.gameObject);
            _cannons.Remove(packet.ballId);
        }
    }

    public void BallMove(BroadcastBMove packet)
    {
        CannonBall ball = null;
        if (_cannons.TryGetValue(packet.ballId, out ball))
        {
            if(ball.owner != PlayerManager.Instance.myPlayer.PlayerId)
            {
                //자기께 아닌 경우에만 처리
                ball.SetDestination(new Vector3(packet.posX, packet.posY, packet.posZ));
            }
        }
    }
}
