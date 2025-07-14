using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BrushStrokeCanvas : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup_text;
    [SerializeField] private BrushStroke stroke;
    [SerializeField] private bool charReveal = false;
    [SerializeField] private TextFadeReveal textFade;
    [SerializeField] private string textHint;

    [SerializeField] private float fadeDuration;
    // Start is called before the first frame update
    void Start()
    {
        stroke.ResetStroke();
        if (!textFade) charReveal = false;
        if (charReveal)
        {
            textFade.Reset();
        }
        else
        {
            canvasGroup_text.alpha = 0f;
        }
            
    }

    public void Reset()
    {
        stroke.ResetStroke();
        if (charReveal)
        {
            textFade.Reset();
        }
        else
        {
            canvasGroup_text.alpha = 0f;
        }  
    }

    public void Animate(Action onCompleted = null)
    {
        stroke.AnimateBrush(() =>
        {
            if (!charReveal)
            {
                canvasGroup_text.DOFade(1f, fadeDuration);
            }
            onCompleted?.Invoke();
        });
        if (charReveal)
        {
            textFade.animateText(textHint);
        }
    }
    
    public void Animate(string withText)
    {
        stroke.AnimateBrush(() =>
        {
            if (!charReveal)
            {
                canvasGroup_text.GetComponent<TMP_Text>().text = withText;
                canvasGroup_text.DOFade(1f, fadeDuration);
            }
        });
        if (charReveal)
        {
            textFade.animateText(withText);
        }
    }
}
