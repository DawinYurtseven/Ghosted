using DG.Tweening;
using UnityEngine;

public class UIFacePlayer : MonoBehaviour
{
    protected Transform mainCamera;
    
    public CanvasGroup canvasGroup;
    public float fadeDuration = 0.3f;
    
    protected Tween currentTween;

    void Awake()
    {
        canvasGroup.alpha = 0f;
    }
    void Start()
    {
        mainCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Make the UI look at the camera
        transform.LookAt(transform.position + mainCamera.forward);
    }
    
    public void Show()
    {
        if (currentTween != null) currentTween.Kill();
        //Debug.Log("Show UI");
        currentTween = canvasGroup.DOFade(1f, fadeDuration);
    }

    public void Hide()
    {
        if (currentTween != null) currentTween.Kill();
        //Debug.Log("Hide UI");
        currentTween = canvasGroup.DOFade(0f, fadeDuration);
    }
}