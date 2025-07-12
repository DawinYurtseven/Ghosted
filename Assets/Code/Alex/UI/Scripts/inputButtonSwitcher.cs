using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InputButtonSwitcher : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite keyboardMouseSprite;

    [SerializeField] private Sprite gamepadSprite;

    private Image targetImage;

    private bool subscribed = false;

    private void OnEnable()
    {
        if (inputManager.Instance != null)
        {
            inputManager.Instance.InputChanged += OnInputChanged;
            Debug.Log($"{gameObject.name} subscribed to InputChanged");
            subscribed = true;
        }
            
    }

    private void OnDisable()
    {
        if (inputManager.Instance != null && subscribed)
        {
            inputManager.Instance.InputChanged -= OnInputChanged;
            inputManager.Instance.InputChanged += OnInputChanged;
            Debug.Log($"{gameObject.name} unsubscribed from InputChanged");
            subscribed = false;
        }
            
    }

    void Start()
    {
        targetImage = GetComponent<Image>();
        if (!subscribed && inputManager.Instance != null)
        {
            inputManager.Instance.InputChanged += OnInputChanged;
            Debug.Log($"{gameObject.name} subscribed to InputChanged");
            subscribed = true;
        }
    }

    private void OnInputChanged(string scheme)
    {
        Debug.Log("reacted to input change");
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