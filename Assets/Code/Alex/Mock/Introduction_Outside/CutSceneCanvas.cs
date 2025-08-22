using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CutSceneCanvas : MonoBehaviour
{
    [Header("Introduction Sequence")]
    public CanvasGroup mainCanvasGroup;
    public CanvasGroup[] groups;
    public TextFadeReveal[] textAnimator;
    public float fadeDuration = 0.5f;
    [SerializeField] private int _currentIndex = 0;
     private Action onCompleted;
   

    public void Initialize(Action onCompleted = null)
    {
        this.onCompleted = onCompleted;
        PlayerInputDisabler.Instance.SwitchInputMap("CutScene");
        mainCanvasGroup.gameObject.SetActive(true);
        foreach (CanvasGroup group in groups)
        {
            group.alpha = 0f;
        }
        
        Show();
    }


    private void Show()
    {
        groups[_currentIndex].DOFade(1f, fadeDuration);
        textAnimator[_currentIndex].animateText();
    }

    public void onAccepted()
    {
        if (textAnimator[_currentIndex].isAnimating)
        {
            textAnimator[_currentIndex].Complete();
        }

        else
        {
            Hide();
        }
        
    }

    public void Hide()
    {
        groups[_currentIndex].DOFade(0f, fadeDuration).OnComplete(() =>
        {
            groups[_currentIndex].gameObject.SetActive(false);
            _currentIndex++;
            if (_currentIndex >= groups.Length)
            {
                PlayerInputDisabler.Instance.SwitchInputMapDelayed("Character Control");
                mainCanvasGroup.DOFade(0f, fadeDuration).OnComplete(() =>
                {
                    mainCanvasGroup.gameObject.SetActive(false);
                                    onCompleted?.Invoke();
                });
            }

            else
            {
                Show();
            }
        });
    }
}
