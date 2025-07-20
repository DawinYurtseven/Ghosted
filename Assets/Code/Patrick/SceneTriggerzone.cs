using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneTriggerzone : TriggerZone
{
    [SerializeField] private string[] scenesToLoad;
    [SerializeField] private string[] scenesToUnload;
    
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
    
    private void loadScenes()
    {
        foreach (string s in scenesToLoad)
        {
            bool isSceneLoaded = SceneManager.GetSceneByName(s).isLoaded;
            
            if(isSceneLoaded) break;
            
            // Load the scene asynchronously, as "a chunk" additively to the current scene 
            SceneManager.LoadSceneAsync(s, LoadSceneMode.Additive).completed += (AsyncOperation op) =>
            {
                Debug.Log("Scene " + s + " loaded successfully.");
            };
        }
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
}
