using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenuController : MonoBehaviour
{
    private Toggle fullscreenToggle;
    private DropdownField resolutionDropdown;
    private Slider volumeSlider;
    private Slider sensitivitySlider;
    private Toggle invertToggle;
    private Button quitButton;
    private VisualElement settingsWindow;

    [SerializeField] private UIDocument document;

    void OnEnable()
    {
        var root = document.rootVisualElement;

        settingsWindow = root.Q<VisualElement>("SettingsWindow");
        fullscreenToggle = root.Q<Toggle>("FullScreenToggle");
        resolutionDropdown = root.Q<DropdownField>("ResolutionDropdown");
        volumeSlider = root.Q<Slider>("VolumeSlider");
        sensitivitySlider = root.Q<Slider>("MouseSensitivitySlider");
        invertToggle = root.Q<Toggle>("InvertMouseToggle");
        quitButton = root.Q<Button>("CloseButton");

        // Populate resolution options
        resolutionDropdown.choices = new List<string> { "1920x1080", "1280x720", "640x480" };

        // Load saved settings
        fullscreenToggle.value = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        resolutionDropdown.value = PlayerPrefs.GetString("resolution", "1920x1080");
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 1f);
        sensitivitySlider.value = PlayerPrefs.GetFloat("sensitivity", 0.5f); // multiply it with camera rotation speed
        invertToggle.value = PlayerPrefs.GetInt("invert", 0) == 1; // muliply the y calue with -1, if toggled

        quitButton.clicked += () =>
        {
            settingsWindow.style.display = DisplayStyle.None;
        };

        // Apply saved resolution
        resolutionDropdown.RegisterValueChangedCallback(evt =>
        {
            SetResolution(evt.newValue);
            PlayerPrefs.SetString("resolution", evt.newValue);
        });

        // Hook up events
        fullscreenToggle.RegisterValueChangedCallback(evt =>
        {
            SetFullscreen(evt.newValue);
            PlayerPrefs.SetInt("fullscreen", evt.newValue ? 1 : 0);
        });

        volumeSlider.RegisterValueChangedCallback(evt =>
        {
            SetVolume(evt.newValue);
            PlayerPrefs.SetFloat("volume", evt.newValue);
        });

        sensitivitySlider.RegisterValueChangedCallback(evt =>
        {
            PlayerPrefs.SetFloat("sensitivity", evt.newValue);
        });

        invertToggle.RegisterValueChangedCallback(evt =>
        {
            PlayerPrefs.SetInt("invert", evt.newValue ? 1 : 0);
        });
    }

    private void SetResolution(string resolutionString)
    {
        string[] dims = resolutionString.Split('x');
        int width = int.Parse(dims[0]);
        int height = int.Parse(dims[1]);
        bool fullscreen = Screen.fullScreen;

        Screen.SetResolution(width, height, fullscreen);
    }

    private void SetFullscreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    private void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void CloseWindow()
    {
        settingsWindow.style.display = DisplayStyle.None;
    }
    public void OpenWindow()
    {
        Debug.Log("Make the window flex");
        settingsWindow.style.display = DisplayStyle.Flex;
    }
}
