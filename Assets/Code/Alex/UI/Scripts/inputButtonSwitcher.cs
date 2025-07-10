using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InputButtonSwitcher : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite keyboardMouseSprite;
    [SerializeField] private Sprite gamepadSprite;

    [Header("Input References")]
    [SerializeField] private PlayerInput playerInput;

    private Image targetImage;

    private void OnEnable()
    {
        if (inputManager.Instance != null)
            inputManager.Instance.InputChanged += OnInputChanged;
    }

    private void OnDisable()
    {
        if (inputManager.Instance != null)
            inputManager.Instance.InputChanged -= OnInputChanged;
    }

    void Start()
    {
        targetImage = GetComponent<Image>();
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