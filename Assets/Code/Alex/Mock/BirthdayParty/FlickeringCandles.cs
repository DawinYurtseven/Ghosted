using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class FlickeringCandles : MonoBehaviour
{
    
    //DEPRECATED
    [Header("Flicker Settings")]
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.2f;
    public float minSpeed = 0.3f;
    public float maxSpeed = 0.8f;
    private Light candleLight;
    private Tween flickerTween;

    private void Awake()
    {
        candleLight = GetComponent<Light>();
    }

    private void OnEnable()
    {
        StartFlicker();
    }

    private void OnDisable()
    {
        StopFlicker();
    }

    private void StartFlicker()
    {
        // Kill any existing tween
        flickerTween?.Kill();

        // Start a new tween
        flickerTween = DOTween.To(
                () => candleLight.intensity,
                x => candleLight.intensity = x,
                Random.Range(minIntensity, maxIntensity),
                Random.Range(minSpeed, maxSpeed)
            )
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void StopFlicker()
    {
        if (flickerTween != null && flickerTween.IsActive())
        {
            flickerTween.Kill();
            flickerTween = null;
        }
    }
}
