using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIInteractionHint : UIFacePlayer
{
    private Transform mainCamera;
    
    [SerializeField] private CanvasGroup canvasGroup_text;
    [SerializeField] private float drawDuration = 1.0f;
    
    private Material brushMaterial; //Material with shader to appear from the left to the right
    [SerializeField] private Image brushImage; 
    private Tween currentTween;
    private Tween drawTween;

    public bool hasText = true;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        
        
        if (hasText)
        {
            if (!brushImage || !brushImage.material || !canvasGroup_text)
            {
                Debug.Log("No text canvas found for interaction");
                hasText = false;
            }
            else
            {
                canvasGroup_text.alpha = 0f;
                brushMaterial = Instantiate(brushImage.material);
                brushImage.material = brushMaterial;
                brushMaterial.SetFloat("_Progress", 0f);
            }
            
        }
    }


    public void Show()
    {
        Debug.Log("Show UI");
        if (currentTween != null) currentTween.Kill();
        if (drawTween != null) drawTween.Kill();
        if (hasText)
        {
            brushMaterial.SetFloat("_Progress", 0f);
            canvasGroup_text.alpha = 0f;
        }
        currentTween = canvasGroup.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            if (hasText)
            { // make brush animation
                drawTween = DOTween.To(
                        () => brushMaterial.GetFloat("_Progress"),
                        value => brushMaterial.SetFloat("_Progress", value),
                        1f,
                        drawDuration
                    )
                    .SetEase(Ease.Linear).OnComplete(() =>
                    {
                        canvasGroup_text.DOFade(1f, fadeDuration); 
                    });
            }
        });
    }

    public void Hide()
    {
        if (currentTween != null) currentTween.Kill();
        if (drawTween != null) drawTween.Kill();

        Debug.Log("Hide UI");

        currentTween = canvasGroup.DOFade(0f, fadeDuration)
            .OnComplete(() =>
            {
                // Reset brush stroke
                if (hasText)
                {
                    brushMaterial.SetFloat("_Progress", 0f);
                    canvasGroup_text.alpha = 0f;
                }
            });
    }
}