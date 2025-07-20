using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderMock : MonoBehaviour
{
    public String introSceneName;
    public String trainSceneName;


    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    
    // quick mockup for start menu 
    public void loadIntroScene()
    {
        if (string.IsNullOrEmpty(introSceneName))
        {
            Debug.LogError("Intro scene name is not set.");
            return;
        }
        
        SceneManager.LoadScene(introSceneName);
    }
    
    public void loadTrainScene()
    {
        if (string.IsNullOrEmpty(trainSceneName))
        {
            Debug.LogError("Train scene name is not set.");
            return;
        }
        
        SceneManager.LoadScene(trainSceneName);
    }
    
    //actual cool scene loader
    private List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    public void StartGame()
    {
        loadIntroScene();
    }
    public void loadNextScene(string sceneName = "")
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            // Load the specified scene by name
            scenesToLoad.Add(SceneManager.LoadSceneAsync(sceneName));
            return;
        }

        // If no scene name is provided, load the next scene in the build settings
        LoadNextSceneInBuildSettings();
    }
    
    private void LoadNextSceneInBuildSettings()
    {
        throw new NotImplementedException();
    }
}
