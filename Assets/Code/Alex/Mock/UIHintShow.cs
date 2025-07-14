using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UIHintShow : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI hintText;
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float defaultHintDuration = 4f;
    [Header("Mapping Settings")]
    [SerializeField] private InputIconMappings iconMappings;

    [SerializeField] private BrushStrokeCanvas brushStroke;

    private Tween currentTween;
    private string currentExpectedAction;
    private string currentHintTemplate;
    private string currentControlScheme = "MK";
        
    private bool hintActive;

    public static UIHintShow Instance { get; private set; }
    
    [SerializeField] private readonly Dictionary<string, string> actionHintTemplates = new()
    {
        { "Jump", "Press [Jump] to leap" },
        { "Move", "Use [Move] to move" },
        { "Shoot", "Press [Shoot] to throw a talisman to or recall from an object when you see the circle" },
        { "Recall", "Use [Recall] at a shrine to recall all talismans at once" },
    };
    
    

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        brushStroke.Reset();

        
        if (inputManager.Instance != null)
        {
            Debug.Log("Subscribed to InputManager!");
            inputManager.Instance.InputChanged += OnInputChanged;
            currentControlScheme = inputManager.Instance.currentScheme;
        }
    }

    void OnDestroy()
    {
        if (inputManager.Instance != null)
            inputManager.Instance.InputChanged -= OnInputChanged;
    }

    private void OnInputChanged(string newScheme)
    {
        currentControlScheme = newScheme;

        if (hintActive)
        {
            string updatedHint = HintBuilder.BuildHint(currentHintTemplate, iconMappings, newScheme);
            hintText.text = updatedHint;
        }
    }
    public void ShowHint(string template)
    {
        ShowHint(template, defaultHintDuration);
        
    }

    public void ShowHint(string template, float duration)
    {
        ClearHint();

        currentHintTemplate = template;
        string message = HintBuilder.BuildHint(template, iconMappings, currentControlScheme);
        hintText.text = message;
        hintActive = true;
        canvasGroup.alpha = 1f;
        brushStroke.Animate(message);
        currentTween = DOVirtual.DelayedCall(duration, HideHint);
        
    }
    public void ShowHintUntilAction(string template, string actionKey)
    {
        ClearHint();
        currentExpectedAction = actionKey;
        currentHintTemplate = template;
        string message = HintBuilder.BuildHint(template, iconMappings, currentControlScheme);
        hintText.text = message;
        hintActive = true;
        brushStroke.Animate(message);
    }
    

    public void ShowHintUntilAction(string actionKey)
    {
        ClearHint();
        currentExpectedAction = actionKey;
        if (!actionHintTemplates.TryGetValue(actionKey, out var template))
        {
            Debug.LogWarning($"No hint template found for action key: {actionKey}");
            return;
        }
        currentHintTemplate = template;
        string message = HintBuilder.BuildHint(template, iconMappings, currentControlScheme);
        hintText.text = message;
        hintActive = true;
        brushStroke.Animate(message);
    }
    

    public void NotifyActionPerformed(string actionKey)
    {
        if (!hintActive || actionKey != currentExpectedAction)
            return;

        HideHint();
    }

    private void HideHint()
    {
        currentTween?.Kill();
        currentTween = canvasGroup.DOFade(0, fadeDuration);
        currentExpectedAction = null;
        hintActive = false;
        currentHintTemplate = null;
    }

    private void ClearHint()
    {
        currentTween?.Kill();
        currentExpectedAction = null;
        currentHintTemplate = null;
        hintActive = false;
    }
}
