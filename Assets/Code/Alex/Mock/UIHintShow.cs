using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class UIHintShow : MonoBehaviour
{
    [Header("UI Components")]
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI hintText;

    [Header("Animation Settings")]
    public float fadeDuration = 0.5f;
    public float standardTime = 6f;
    private Tween currentTween;
    
    public static UIHintShow Instance { get; private set; }
    
    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        canvasGroup.alpha = 0;
        // No need to touch interactable or raycasts
    }

    public void showHintMessage(string message)
    {
        ShowHint(message, standardTime);
    }
    public void ShowHint(string message, float duration)
    {
        currentTween?.Kill();

        hintText.text = message;

        currentTween = canvasGroup
            .DOFade(1, fadeDuration)
            .OnComplete(() =>
            {
                currentTween = DOVirtual.DelayedCall(duration, () =>
                {
                    currentTween = canvasGroup.DOFade(0, fadeDuration);
                });
            });
    }

}