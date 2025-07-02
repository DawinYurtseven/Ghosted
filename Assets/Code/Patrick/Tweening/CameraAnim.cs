using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class CameraZoom : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCam;
    public float zoomInDistance = 2f;
    public float zoomOutDistance = 6f;
    public float zoomDuration = 0.5f;

    private Cinemachine3rdPersonFollow follow;
    private float defaultDistance;
    
    void Start()
    {
        follow = virtualCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        defaultDistance = follow.CameraDistance;
    }

    public void ZoomIn()
    {
        DOTween.To(() => follow.CameraDistance, x => follow.CameraDistance = x, zoomInDistance, zoomDuration);
    }

    public void ZoomOut()
    {
        DOTween.To(() => follow.CameraDistance, x => follow.CameraDistance = x, zoomOutDistance, zoomDuration);
    }
    
    public void ResetZoom()
    {
        DOTween.To(() => follow.CameraDistance, x => follow.CameraDistance = x, defaultDistance, zoomDuration);
    }
}