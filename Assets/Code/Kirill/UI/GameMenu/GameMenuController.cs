using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class GameMenuController : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    [SerializeField] private SettingsMenuController settingsMenuController;
    [SerializeField] private DialogueMenuController dialogueMenuController;
    [SerializeField] private InputActionReference uiEscAction;

    void OnEnable()
    {
        var root = document.rootVisualElement;
        uiEscAction.action.performed += EscapeAction;
    }

    void Start()
    {
        settingsMenuController.CloseWindow();
    }

    void EscapeAction(InputAction.CallbackContext context)
    {
        if (settingsMenuController.IsOpen)
            settingsMenuController.CloseWindow();
        else
            settingsMenuController.OpenWindow();
    }
}
