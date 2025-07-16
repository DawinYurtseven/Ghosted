using System;
using Unity.VisualScripting;
using UnityEngine;

public class SceneLoaderMock : MonoBehaviour
{
    public String introSceneName;
    public String trainSceneName;


    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    public void loadIntroScene()
    {
        if (string.IsNullOrEmpty(introSceneName))
        {
            Debug.LogError("Intro scene name is not set.");
            return;
        }
        
        UnityEngine.SceneManagement.SceneManager.LoadScene(introSceneName);
    }
    
    public void loadTrainScene()
    {
        if (string.IsNullOrEmpty(trainSceneName))
        {
            Debug.LogError("Train scene name is not set.");
            return;
        }
        
        UnityEngine.SceneManagement.SceneManager.LoadScene(trainSceneName);
    }
}
