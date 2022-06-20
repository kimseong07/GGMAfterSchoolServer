using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{

    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private float _timeBetFire = 1f; //초당 한발씩
    private float _lastFireTime;

    [SerializeField] private GameObject ballPrefab;

    private Transform _firePos;

    private void Start()
    {
        _whatIsGround = 1 << LayerMask.NameToLayer("GROUND");
        ballPrefab = Resources.Load<GameObject>("Ball");
        StartCoroutine(CoSendPacket());
        _lastFireTime = Time.time;
        _firePos = transform.Find("FirePos");
    }

    protected override void Update()
    {
        HandleMove();
        HandleFire();
        

        Vector3 dir = _destination - transform.position;
        if (dir.sqrMagnitude >= 0.25f * 0.25f)
            transform.position = transform.position + dir.normalized * _moveSpeed * Time.deltaTime;

        transform.rotation = Quaternion.Lerp(transform.rotation, _rot, Time.deltaTime * _rotateSpeed);
    }

    private void HandleFire()
    {
        if (Input.GetMouseButtonDown(1) && _lastFireTime + _timeBetFire <= Time.time)
        {
            BFire firePacket = new BFire();
            Vector3 firePos = _firePos.position;
            firePacket.posX = firePos.x;
            firePacket.posY = firePos.y;
            firePacket.posZ = firePos.z;

            NetworkManager.Instance.Send(firePacket.Write());

            //GameObject ballObj = Instantiate(ballPrefab, _firePos.position, Quaternion.identity);
            //CannonBall ball = ballObj.GetComponent<CannonBall>();
            //ball.SetPower(_firePos.forward * 50f);
            _lastFireTime = Time.time;
        }
    }

    private void HandleMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(camRay, out hit, 100f, _whatIsGround))
            {
                Vector3 dest = hit.point;
                dest.y = 0.5f;
                SetDestination(dest);
                SetRotation(Quaternion.LookRotation(dest - transform.position));
            }
        }
    }
    public void FireCannon(CannonBall ball)
    {
        ball.transform.position = _firePos.position;
        ball.SetPower(_firePos.forward * 50f);
    }

    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);

            Vector3 pos = transform.position;
            Move movePacket = new Move();
            movePacket.posX = pos.x;
            movePacket.posY = pos.y;
            movePacket.posZ = pos.z;

            NetworkManager.Instance.Send(movePacket.Write());
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_destination, 0.2f);
    }
}
