using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class CameraZoom : MonoBehaviour
{
    public CinemachineFreeLook freeLookCam;
    public float zoomDuration = 0.5f;

    private CinemachineFreeLook.Orbit[] originalOrbits;

    void Start()
    {
        originalOrbits = freeLookCam.m_Orbits;
    }

    public void ZoomIn(float zoomAmount)
    {
        for (int i = 0; i < freeLookCam.m_Orbits.Length; i++)
        {
            float newHeight = originalOrbits[i].m_Height - zoomAmount;
            float newRadius = originalOrbits[i].m_Radius - zoomAmount;
            DOTween.To(() => freeLookCam.m_Orbits[i].m_Height,
                x => freeLookCam.m_Orbits[i].m_Height = x,
                newHeight, zoomDuration);
            DOTween.To(() => freeLookCam.m_Orbits[i].m_Radius,
                x => freeLookCam.m_Orbits[i].m_Radius = x,
                newRadius, zoomDuration);
        }
    }

    public void ZoomOut(float zoomAmount)
    {
        for (int i = 0; i < freeLookCam.m_Orbits.Length; i++)
        {
            float newHeight = originalOrbits[i].m_Height;
            float newRadius = originalOrbits[i].m_Radius;
            DOTween.To(() => freeLookCam.m_Orbits[i].m_Height,
                x => freeLookCam.m_Orbits[i].m_Height = x,
                newHeight, zoomDuration);
            DOTween.To(() => freeLookCam.m_Orbits[i].m_Radius,
                x => freeLookCam.m_Orbits[i].m_Radius = x,
                newRadius, zoomDuration);
        }
    }
}