using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneTriggerzone : TriggerZone
{
    [SerializeField] private string[] scenesToLoad;
    [SerializeField] private string[] scenesToUnload;

    [SerializeField] private GameObject joyParent;
    [SerializeField] private GameObject fearParent;
    
    private GameObject player;
    
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found in the scene. Please ensure the player has the 'Player' tag.");
        }
        else
        {
            Debug.Log("Player found: " + player.name);
        }
    }

    private void Start()
    {
        Debug.LogWarning("SceneTriggerzone System is used. Player needs to be tagged as 'Player'.\n" +
                         "Also, any managers should be placed in a empty scene that never will be unloaded, "+
                         "otherwise they will be unloaded when the scene is unloaded.");
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            Debug.Log("Player entered the scene trigger zone, loading scenes: " + string.Join(", ", scenesToLoad));
            loadScenes();
            unloadScenes();
        }
    }
    
    public void loadScenes()
    {
        foreach (string s in scenesToLoad)
        {
            if(s == null || s.Trim() == "")
            {
                Debug.LogError("Scene name is empty or null. Please check the scenesToLoad array.");
                continue;
            }
            
            bool isSceneLoaded = SceneManager.GetSceneByName(s).isLoaded;
            if(isSceneLoaded) break;
            
            // Load the scene asynchronously, as "a chunk" additively to the current scene
            Debug.Log("Loading scene: " + s);
            StartCoroutine(LoadSceneSmoothly(s));
            
        }
    }
    
    IEnumerator LoadSceneSmoothly(string sceneName) {
        if(sceneName == null || sceneName.Trim() == "")
        {
            Debug.LogError("Scene name is empty or null. Please check the scene name.");
            yield break;
        }
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        if(asyncLoad == null)
        {
            Debug.LogError("Failed to load scene: " + sceneName);
            yield break;
        }
        
        asyncLoad.allowSceneActivation = false;
        asyncLoad.completed += (AsyncOperation op) =>
        {
            Debug.Log("Scene " + sceneName + " loaded successfully.");
            onSceneLoaded(op, sceneName);
        };
        
        while (asyncLoad.progress < 0.9f)
            yield return null;

        // Optional delay/UI transition here
        asyncLoad.allowSceneActivation = true;
        Debug.Log("Scene " + sceneName + " fully loaded.");
    }
    
    private void onSceneLoaded(AsyncOperation op, string s)
    {
        // Move Joy
        GameObject joy = getObjectInSceneByName(s, "Joy");
        if(!joy) Debug.LogError("No joy object found in scene " + s);
        moveObjectsToParent(joy, joyParent);
                
        //Move Fear
        GameObject fear = getObjectInSceneByName(s, "Fear");
        if(!fear) Debug.LogError("No fear object found in scene " + s);
        moveObjectsToParent(fear, fearParent);
                
        Debug.Log("Moved Joy and Fear objects to their respective parents.");
    }

    private void unloadScenes()
    {
        foreach (string s in scenesToUnload)
        {
            bool isSceneLoaded = SceneManager.GetSceneByName(s).isLoaded;
            
            if(!isSceneLoaded) continue;
            
            // Unload the scene asynchronously
            SceneManager.UnloadSceneAsync(s).completed += (AsyncOperation op) =>
            {
                Debug.Log("Scene " + s + " unloaded successfully.");
            };
        }
    }

    public void loadEmotion(GameObject emotionParent)
    {
        if (emotionParent == null)
        {
            Debug.LogError("Emotion parent is not set.");
            return;
        }
        
        // move all children of the emotion parent to the correct parent object in the scene
        if (emotionParent.name == "Joy")
        {
            Debug.Log("Loading JOY Emotion into single scene toggle object");
            moveObjectsToParent(emotionParent, joyParent);
        }
        else if (emotionParent.name == "Fear")
        {
            Debug.Log("Loading FEAR Emotion");
            moveObjectsToParent(emotionParent, fearParent);
        }
    }
    
    private static async void moveObjectsToParent(GameObject parent, GameObject newParent)
    {
        if (parent == null || newParent == null)
        {
            Debug.LogError("Parent or new parent is not set.");
            return;
        }

        Debug.Log("Children in parent: " + parent.transform.childCount);
        foreach (Transform child in parent.transform)
        {
            child.SetParent(newParent.transform);
        }

        Debug.Log("Children in new parent: " + newParent.transform.childCount);
        // Optionally, deactivate the old parent
        parent.SetActive(false);
    }
    
    private static GameObject getObjectInSceneByName(string sceneName, string name)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.isLoaded)
        {
            Debug.LogError("Scene " + sceneName + " is not loaded.");
            return null;
        }

        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            if (obj.name == name)
            {
                return obj;
            }
        }

        Debug.LogWarning("Object with name " + name + " not found in scene " + sceneName);
        return null;
    }
}
