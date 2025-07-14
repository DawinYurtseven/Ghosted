using DG.Tweening;
using UnityEngine;

public class UIInteractionHint : UIFacePlayer
{
    [SerializeField] private BrushStrokeCanvas brushStroke;
    private void Awake()
    {
        canvasGroup.alpha = 0f;
    }
    
    public void Show()
    {
        Debug.Log("Show UI");
        if (currentTween != null) currentTween.Kill();
        
        if (brushStroke)
        {
            brushStroke.Reset();
        }

        currentTween = canvasGroup.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            if (brushStroke)
            {
                brushStroke.Animate();
            }
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
                if (brushStroke)
                {
                    brushStroke.Reset();
                }
            });
    }
}