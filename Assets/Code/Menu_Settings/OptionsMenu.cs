using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using System.Collections.Generic; // For List


public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown graphicsDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sensitivitySlider;

    

    void Start()
    {
        // Fullscreen
        SetFullScreen(Screen.fullScreen);

        // Graphics Quality
        SetGraphicsQuality(PlayerPrefs.GetInt("Quality") == 0 ? 2 : PlayerPrefs.GetInt("Quality"));

        // Resolutions
        SetResolution(PlayerPrefs.GetInt("Resolution") == 0 ? 1 : PlayerPrefs.GetInt("Resolution"));

        // Volume
        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(SetVolume);
        
        //Sensitivity
        SetSensitivity(PlayerPrefs.GetFloat("sensitivity") == 0f ? 30f : PlayerPrefs.GetFloat("sensitivity"));
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        fullscreenToggle.isOn = isFullScreen;
    }

    public void SetGraphicsQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("Quality", index);
        PlayerPrefs.Save();
        graphicsDropdown.value = index;
    }

    public void SetResolution(int index)
    {
        switch (index)
        {
            case 0:
                Screen.SetResolution(800, 600, Screen.fullScreen);
                break;
            case 1:
                Screen.SetResolution(1980, 1080, Screen.fullScreen);
                break;
            case 2:
                Screen.SetResolution(2160, 1440, Screen.fullScreen);
                break;
            case 3:
                Screen.SetResolution(2160, 1600, Screen.fullScreen);

                break;
            case 4:
                Screen.SetResolution(3840, 2160, Screen.fullScreen);
                break;
        }
        PlayerPrefs.SetInt("Resolution", index);
        PlayerPrefs.Save();
        resolutionDropdown.value = index;
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }
    
    public void SetSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("sensitivity", sensitivity);
        PlayerPrefs.Save();
        sensitivitySlider.value = sensitivity;
    }
}
