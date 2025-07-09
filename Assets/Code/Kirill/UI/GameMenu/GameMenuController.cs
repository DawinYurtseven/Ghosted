using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameMenuController : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    [SerializeField] private SettingsMenuController settingsMenuController;

    private Button settingsButton;

    void OnEnable()
    {
        var root = document.rootVisualElement;

        settingsButton = root.Q<Button>("SettingsButton");

        settingsButton.clicked += () =>
        {
            settingsMenuController.OpenWindow();
        };
    }
}
