using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class buttonSelect : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("Animated Elements")]
    public RectTransform buttonTransform;
    public CanvasGroup outline;

    [Header("Animation Settings")]
    public float fadeDuration = 0.25f;
    public float scaleAmount = 1.1f;
    public float scaleDuration = 0.2f;

    private Vector3 originalScale;
    public UnityEvent onSelect, onDeselect;

    void Awake()
    {
        if (buttonTransform == null)
            buttonTransform = GetComponent<RectTransform>();

        originalScale = buttonTransform.localScale;
        if (outline != null)
            outline.alpha = 0f;
    }

    public void OnSelect(BaseEventData eventData)
    {
        
        buttonTransform.DOScale(originalScale * scaleAmount, scaleDuration).SetEase(Ease.OutBack);
        if (outline != null)
            outline.DOFade(1f, fadeDuration);
        onSelect?.Invoke();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        buttonTransform.DOScale(originalScale, scaleDuration).SetEase(Ease.OutBack);

        if (outline != null)
            outline.DOFade(0f, fadeDuration);
        onDeselect?.Invoke();
    }
}
