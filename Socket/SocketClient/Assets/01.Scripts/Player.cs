using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
   

    public int PlayerId { get; set; }

    public Color _color;

    MeshRenderer _renderer;

    protected Vector3 _destination;
    protected Quaternion _rot = Quaternion.identity;
    private float _lerpSpeed = 5f;
    [SerializeField]
    protected float _rotateSpeed = 8f;

    protected float _moveSpeed = 10f; 

    private void Awake()
    {
        _renderer = transform.Find("Body").GetComponent<MeshRenderer>(); 
    }

    public void SetColor(System.Numerics.Vector3 data)
    {
        _color = new Color(data.X, data.Y, data.Z);
        _renderer.material.SetColor("_Color", _color);
    }

    public void SetDestination(Vector3 pos)
    {
        _destination = pos;
    }

    protected virtual void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _destination, Time.deltaTime * _lerpSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, _rot, Time.deltaTime * _rotateSpeed);
    }

    public void SetRotation(Quaternion rot)
    {
        _rot = rot;
    }
}
