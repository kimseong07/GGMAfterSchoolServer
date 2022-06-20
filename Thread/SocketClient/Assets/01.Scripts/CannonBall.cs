using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    private bool mine = false;
    public int owner;
    public int ballId;

    Rigidbody _rigid;

    private Vector3 _destination;

    public void SetDestination(Vector3 pos)
    {
        _destination = pos;
    }

    //이걸 호출했다는건 자기꺼라는 뜻 나머지는 파워 없이 위치만 동기화 한다.
    public void SetPower(Vector3 dir)
    {
        _rigid = gameObject.AddComponent<Rigidbody>();
        _rigid.mass = 2f;
        _rigid.AddForce(dir, ForceMode.Impulse);
        mine = true;
        StartCoroutine(CoSendPacket());
    }

    private void Update()
    {
        if (mine) return;
        transform.position = Vector3.Lerp(transform.position, _destination, Time.deltaTime * 8f);
    }

    public void CopyBallData(BallList.Ball ballData)
    {
        owner = ballData.owner;
        ballId = ballData.ballId;
        Vector3 pos = new Vector3(ballData.posX, ballData.posY, ballData.posZ);
        transform.position = pos;
    }

    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);

            Vector3 pos = transform.position;
            BMove movePacket = new BMove();
            movePacket.posX = pos.x;
            movePacket.posY = pos.y;
            movePacket.posZ = pos.z;
            movePacket.ballId = ballId;

            Debug.Log($"{pos.y} 볼 위치 전송");
            NetworkManager.Instance.Send(movePacket.Write());
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (!collision.gameObject.CompareTag("Player")) //자기자신과의 충돌이 아니라면
    //    {
    //        Destroy(gameObject);
    //    }
    //}
}
