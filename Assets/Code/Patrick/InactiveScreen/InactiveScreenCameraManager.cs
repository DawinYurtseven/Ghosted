using UnityEngine;
using UnityEngine.SceneManagement;

public class InactiveScreenCameraManager : MonoBehaviour
{
    public string inactiveSceneName = "DemoScene";
    private Camera mainCam;
    private Behaviour cinemachineBrain;

    void Start()
    {
        mainCam = Camera.main;
        if (mainCam != null)
        {
            Debug.Log("Main camera found: " + mainCam.name);
            cinemachineBrain = mainCam.GetComponent<Behaviour>(); //CinemachineBrain is a Behaviour
        }
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == inactiveSceneName)
        {
            if (mainCam != null) mainCam.enabled = false;
            if (cinemachineBrain != null) cinemachineBrain.enabled = false;
        }
    }

    void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == inactiveSceneName)
        {
            if (mainCam != null) mainCam.enabled = true;
            if (cinemachineBrain != null) cinemachineBrain.enabled = true;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

}
