using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CandlesMoving : MonoBehaviour
{
    [Header("Platform Movement")]
    public float moveHeight = 3f;
    public float moveDuration = 2f;
    public float moveDurationDown = 1f;
    public float pauseTime = 1f;

    [Header("Candle Lights")]
    public List<GameObject> candleLights;
    private Vector3 initialPosition;
    private void Start()
    {
        initialPosition = transform.position;
        StartCandleCycle();
    }

    private void StartCandleCycle()
    {
        Sequence candleSequence = DOTween.Sequence();

        // Lights On
        candleSequence.AppendCallback(TurnOnLights);

        //Move Up
        candleSequence.Append(transform.DOMoveY(initialPosition.y + moveHeight, moveDuration).SetEase(Ease.InOutSine));

        // Pause at top
        candleSequence.AppendInterval(pauseTime);

        // Lights Off
        candleSequence.AppendCallback(TurnOffLights);

        //  Move Down
        candleSequence.Append(transform.DOMoveY(initialPosition.y, moveDurationDown).SetEase(Ease.InOutSine));

        //  Pause at bottom
        candleSequence.AppendInterval(pauseTime);
        
        candleSequence.SetLoops(-1);
    }

    private void TurnOnLights()
    {
        foreach (var light in candleLights)
        {
            if (light != null)
                light.SetActive(true);
        }
    }

    private void TurnOffLights()
    {
        foreach (var light in candleLights)
        {
            if (light != null)
                light.SetActive(false);
        }
    }
}
