using System;
using UnityEngine;
using DG.Tweening;

public class UIAnimator : MonoBehaviour
{
    [Header("Canvas Groups to Animate")]
    public CanvasGroup[] canvasGroups;

    [Header("Behavior Settings")]
    public bool initializeAtStart = true;
    public bool initializeTogether = true;
    public float offset = 0.1f;

    [Header("Animation Options")]
    public bool resizing = true;
    public bool fading = true;
    public float duration = 0.4f;
    public float startScale = 0.8f;

    public bool hidden = true;

    private void Awake()
    {
        if (hidden)
        {
            foreach (CanvasGroup cg in canvasGroups)
            {
                cg.alpha = 0f;
            }
        }
        else
        {
            foreach (CanvasGroup cg in canvasGroups)
            {
                cg.alpha = 1f;
            }
        }
    }
    private void Start()
    {
        if (initializeAtStart)
        {
            Show();
        }
    }

    public void Show(Action onComplete = null)
    {
        if (!hidden)
        {
            onComplete?.Invoke();
            return;
        }
        hidden = false;
        if (initializeTogether)
        {
            AnimateTogether(true, onComplete);
        }
        else
        {
            AnimateInSequence(true, onComplete);
        }
    }

    public void Hide(Action onComplete = null)
    {
        if (hidden)
        {
            onComplete?.Invoke();
            return;
        }

        hidden = true;
        if (initializeTogether)
        {
            AnimateTogether(false, onComplete);
        }
        else
        {
            AnimateInSequence(false, onComplete);
        }
    }

    private void AnimateTogether(bool show, Action onComplete = null)
    {
        int completedTweens = 0;
        int totalTweens = 0;

        foreach (var cg in canvasGroups)
        {
            if (cg == null) continue;

            RectTransform rect = cg.GetComponent<RectTransform>();
            
            if (resizing && rect != null)
                rect.DOKill();

            if (fading)
                cg.DOKill();

            if (resizing && rect != null)
            {
                rect.localScale = show ? Vector3.one * startScale : Vector3.one;
                totalTweens++;
                rect.DOScale(show ? 1f : startScale, duration)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        completedTweens++;
                        if (completedTweens == totalTweens)
                            onComplete?.Invoke();
                    });
            }

            if (fading)
            {
                cg.alpha = show ? 0f : 1f;
                totalTweens++;
                cg.DOFade(show ? 1f : 0f, duration)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        completedTweens++;
                        if (completedTweens == totalTweens)
                            onComplete?.Invoke();
                    });
            }
        }

        if (totalTweens == 0)
            onComplete?.Invoke();
    }

    private void AnimateInSequence(bool show, Action onComplete = null)
    {
        Sequence seq = DOTween.Sequence();

        foreach (var cg in canvasGroups)
        {
            if (cg == null) continue;

            RectTransform rect = cg.GetComponent<RectTransform>();

            // Kill existing tweens before adding new ones
            if (resizing && rect != null)
                rect.DOKill();

            if (fading)
                cg.DOKill();

            seq.AppendCallback(() =>
            {
                if (resizing && rect != null)
                {
                    rect.localScale = show ? Vector3.one * startScale : Vector3.one;
                    rect.DOScale(show ? 1f : startScale, duration).SetEase(Ease.OutBack);
                }

                if (fading)
                {
                    cg.alpha = show ? 0f : 1f;
                    cg.DOFade(show ? 1f : 0f, duration).SetEase(Ease.InOutSine);
                }
            });

            seq.AppendInterval(offset);
        }

        seq.OnComplete(() => onComplete?.Invoke());
    }
}
