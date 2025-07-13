using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIInteractionHint : UIFacePlayer
{
    private Transform mainCamera;
    
    [SerializeField] private CanvasGroup canvasGroup_text;
    [SerializeField] private BrushStroke stroke;
    private Tween currentTween;
    

    public bool hasText = true;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        
        if (hasText)
        {
            if (!stroke || !canvasGroup_text)
            {
                Debug.Log("No text canvas found for interaction");
                hasText = false;
            }
            else
            {
                stroke.ResetStroke();
                canvasGroup_text.alpha = 0f;
                
            }
        }
    }


    public void Show()
    {
        Debug.Log("Show UI");
        if (currentTween != null) currentTween.Kill();
        
        if (hasText)
        {
            stroke.ResetStroke();
            canvasGroup_text.alpha = 0f;
        }

        currentTween = canvasGroup.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            if (hasText) stroke.AnimateBrush(() => canvasGroup_text.DOFade(1f, fadeDuration));
        });
    }

    public void Hide()
    {
        if (currentTween != null) currentTween.Kill();
        

        Debug.Log("Hide UI");

        currentTween = canvasGroup.DOFade(0f, fadeDuration)
            .OnComplete(() =>
            {
                // Reset brush stroke
                if (hasText)
                {
                    stroke.ResetStroke();
                    canvasGroup_text.alpha = 0f;
                }
            });
    }
}