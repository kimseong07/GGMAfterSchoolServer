using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    private CinemachineVirtualCamera vCam;
    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError($"Error: Multiple camera manager is running");
        }
        Instance = this;
    }

    private void Start()
    {
        vCam = GameObject.Find("FollowCam").GetComponent<CinemachineVirtualCamera>();
    }

    public void SetMyPlayer(Transform target)
    {
        vCam.m_Follow = target;
        vCam.m_LookAt = target;
    }
}
