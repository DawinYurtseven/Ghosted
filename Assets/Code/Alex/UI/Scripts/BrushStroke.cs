using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BrushStroke : MonoBehaviour
{
    private Material brushMaterial; //Material with shader to appear from the left to the right
    private Image brushImage;
    [SerializeField] private float drawDuration = 1.0f;
    private Tween drawTween;
    
    void Awake()
    {
        brushImage = gameObject.GetComponent<Image>();
        if (!brushImage || !brushImage.material)
        {
            Debug.Log("brushStroke setup false");
            Destroy(this);
        }
        brushMaterial = Instantiate(brushImage.material);
        brushImage.material = brushMaterial;
        brushMaterial.SetFloat("_Progress", 0f);
        
    }
    public void AnimateBrush(Action onComplete)
    {
        
        if (drawTween != null) drawTween.Kill();
        drawTween = DOTween.To(
                () => brushMaterial.GetFloat("_Progress"),
                value => brushMaterial.SetFloat("_Progress", value),
                1f,
                drawDuration
            )
            .SetEase(Ease.Linear).OnComplete(() => onComplete?.Invoke());
    }

    public void ResetStroke()
    {
        brushMaterial.SetFloat("_Progress", 0f);
    }
}
