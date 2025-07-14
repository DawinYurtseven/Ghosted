using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    
    public CinemachineVirtualCamera faceCamera, ghostCamera,lookGhostCamera;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void turnOnCamera(string camera)
    {
        if (!hasCameras()) return;
        faceCamera.Priority = camera == "face" ? 100 : 0;
        ghostCamera.Priority = camera == "ghost" ? 100 : 0;
        lookGhostCamera.Priority = camera == "look"? 100 : 0;
    }

    public void turnOffAll()
    {
        if (!hasCameras()) return;
        faceCamera.Priority = 0;
        ghostCamera.Priority = 0;
        lookGhostCamera.Priority = 0;
    }

    public void turnOnOtherCamera(CinemachineVirtualCamera otherCamera)
    {
        if (!hasCameras()) return;
        otherCamera.Priority = 100;
        turnOffAll();
    }
    
    public void turnoOffOtherCamera(CinemachineVirtualCamera otherCamera)
    {
        otherCamera.Priority = 0;
    }

    private bool hasCameras()
    {
        return ghostCamera != null && lookGhostCamera != null && faceCamera != null;
    }
}


