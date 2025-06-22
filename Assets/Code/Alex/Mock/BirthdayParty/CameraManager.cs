using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera faceCamera, ghostCamera,lookGhostCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void turnOnCamera(string camera)
    {
        faceCamera.Priority = camera == "face" ? 100 : 0;
        ghostCamera.Priority = camera == "ghost" ? 100 : 0;
        lookGhostCamera.Priority = camera == "look"? 100 : 0;
    }

    public void turnOffAll()
    {
        faceCamera.Priority = 0;
        ghostCamera.Priority = 0;
        lookGhostCamera.Priority = 0;
    }

    public void turnOnOtherCamera(CinemachineVirtualCamera otherCamera)
    {
        otherCamera.Priority = 100;
        turnOffAll();
    }
    
    public void turnoOffOtherCamera(CinemachineVirtualCamera otherCamera)
    {
        otherCamera.Priority = 0;
    }
}


