using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class textChanger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI targetText;
    
    [Header("Animation Settings")]
    public float fadeDuration = 0.3f;
    public float fastFadeDuration = 0.15f;

    private bool isVisible = false;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        if (targetText == null)
            targetText = GetComponent<TextMeshProUGUI>();

        canvasGroup = targetText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = targetText.gameObject.AddComponent<CanvasGroup>();
        }
        
        canvasGroup.alpha = 0f;
        isVisible = false;
    }

    public void Show()
    {
        if (isVisible) return;
        canvasGroup.DOFade(1f, fadeDuration);
        isVisible = true;
    }

    public void Hide()
    {
        if (!isVisible) return;
        canvasGroup.DOFade(0f, fadeDuration);
        isVisible = false;
    }

    public void ChangeText(string newText)
    {
        if (!isVisible)
        {
            targetText.text = newText;
            Show();
        }
        else
        {
            //fade out and then fade in again
            canvasGroup.DOFade(0f, fastFadeDuration).OnComplete(() =>
            {
                targetText.text = newText;
                canvasGroup.DOFade(1f, fadeDuration);
            });
            isVisible = true;
        }
    }
}
