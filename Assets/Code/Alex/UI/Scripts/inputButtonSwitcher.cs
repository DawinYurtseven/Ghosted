using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InputButtonSwitcher : MonoBehaviour
{
    [Header("Sprites")] [SerializeField] private Sprite keyboardMouseSprite;
    [SerializeField] private Sprite gamepadSprite;

    private Image targetImage;
    private bool subscribed;


    private void OnEnable()
    {
        if (inputManager.Instance != null)
        {
            OnInputChanged(inputManager.Instance.currentScheme);
            
            inputManager.Instance.InputChanged += OnInputChanged;
            subscribed = true;
        }
    }

    public void Start()
    {
        if (inputManager.Instance != null)
        {
            OnInputChanged(inputManager.Instance.currentScheme);
            
            inputManager.Instance.InputChanged += OnInputChanged;
            subscribed = true;
        }
    }

    private void OnDisable()
    {
        if (inputManager.Instance != null && subscribed)
        {
            inputManager.Instance.InputChanged -= OnInputChanged;
            inputManager.Instance.InputChanged += OnInputChanged;
            subscribed = false;
        }
    }

    private void OnInputChanged(string scheme)
    {
        targetImage = GetComponent<Image>();
        Debug.Log(targetImage.sprite.name);
        if (targetImage == null)
            return;

        if (scheme == "Gamepad" || scheme == "Controller")
        {
            targetImage.sprite = gamepadSprite;
        }
        else
        {
            targetImage.sprite = keyboardMouseSprite;
        }
    }
}