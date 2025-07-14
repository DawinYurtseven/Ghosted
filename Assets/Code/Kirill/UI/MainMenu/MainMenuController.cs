using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    [SerializeField] private SettingsMenuController settingsMenuController;
    private Button startGameButton;
    private Button settingsButton;
    private Button quitButton;
    void OnEnable()
    {
        var root = document.rootVisualElement;

        startGameButton = root.Q<Button>("StartGameButton");
        settingsButton = root.Q<Button>("SettingsButton");
        quitButton = root.Q<Button>("QuitButton");

        settingsMenuController.OnQuitClickSubscribe(OnCloseSettings);

        startGameButton.clicked += () =>
        {
            SceneLoader.LoadScene(1); // Make Sure you have scene 1
        };
        settingsButton.clicked += () =>
        {
            OnOpenSettings();
            settingsMenuController.OpenWindow();
        };
        quitButton.clicked += () =>
        {
            Application.Quit();
        };
    }

    void Start()
    {
        settingsMenuController.CloseWindow();
        startGameButton.Focus();
    }

    private void OnOpenSettings()
    {
        startGameButton.style.display = DisplayStyle.None;
        settingsButton.style.display = DisplayStyle.None;
        quitButton.style.display = DisplayStyle.None;
    }
    private void OnCloseSettings()
    {
        startGameButton.style.display = DisplayStyle.Flex;
        settingsButton.style.display = DisplayStyle.Flex;
        quitButton.style.display = DisplayStyle.Flex;
        startGameButton.Focus();
    }
}
