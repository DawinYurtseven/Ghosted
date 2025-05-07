using System.Linq;
using UnityEngine;
using UnityEditor;

public class PlayerPositionTool : EditorWindow
{
    private static PlayerConfig config;         // Stored config
    private static bool snapToSurface = true;   // Default snapping 
    private static KeyCode HotKey = KeyCode.U;  // Hotkey to move the player
    private static bool notInPlayMode = false;  // If it should work in playmode
    
    [MenuItem("Tools/Player Position Tool")]
    public static void ShowWindow()
    {
        GetWindow<PlayerPositionTool>("Player Position Tool");
    }
    
    [InitializeOnLoadMethod]
    private static void EnsureConfigIsLoaded()
    {
        LoadOrCreateConfig();
    }
    
    private void OnEnable()
    {
        if (config == null)
            LoadOrCreateConfig();
    }

    private static void LoadOrCreateConfig()
    {
        string[] guids = AssetDatabase.FindAssets("t:PlayerConfig");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            config = AssetDatabase.LoadAssetAtPath<PlayerConfig>(path);
        }
        else
        {
            config = CreateInstance<PlayerConfig>();
            AssetDatabase.CreateAsset(config, "Assets/PlayerConfig.asset");
            AssetDatabase.SaveAssets();
            AssignPlayerFromLayer();
        }
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Player Position Tool", EditorStyles.boldLabel);
        
        // Show the config (player and offset)
        config.player = (GameObject)EditorGUILayout.ObjectField("Player", config.player, typeof(GameObject), true);
        config.offset = EditorGUILayout.Vector3Field("Offset", config.offset);
        
        snapToSurface = EditorGUILayout.Toggle("Snap to Surface", snapToSurface);
        notInPlayMode = EditorGUILayout.Toggle("Teleport not play-mode", notInPlayMode);
        HotKey = (KeyCode)EditorGUILayout.EnumPopup("Hotkey", HotKey);
        
        EditorGUILayout.HelpBox($"Press {HotKey} in the Scene View to move the player.\nHold CTRL+{HotKey} to 'play from here'.", MessageType.Info);
        
        if (GUILayout.Button("Auto-Assign Player from Layer"))
            AssignPlayerFromLayer();
        
        if (GUI.changed)
            EditorUtility.SetDirty(config);
    }
    
    private static void AssignPlayerFromLayer()
    {
        var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene()
            .GetRootGameObjects()
            .SelectMany(root => root.GetComponentsInChildren<Transform>(true));
        foreach (var t in roots)
        {
            GameObject obj = t.gameObject;
            if (obj.layer == LayerMask.NameToLayer("Player") && obj.GetComponent<Collider>() != null)
            {
                config.player = obj;
                Debug.Log($"Player auto-assigned: {config.player.name}");
                return;
            }
        }
        Debug.LogWarning("No GameObject found with the layer 'Player'.");
    }
    
    [InitializeOnLoadMethod]
    private static void InitializeHotkeyListener()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }
    
    private static void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.keyCode == HotKey)
        {
            if (EditorApplication.isPlaying && notInPlayMode)
                return;
            
            if (config.player == null)
            {
                AssignPlayerFromLayer();
                if (config.player == null)
                {
                    Debug.LogWarning("No player found. Assign a GameObject to the 'Player' layer.");
                    return;
                }
            }
            
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Undo.RecordObject(config.player.transform, "Move Player");
                
                // Compute target position
                Vector3 targetPos = hit.point;
                if (snapToSurface)
                {
                    Collider col = config.player.GetComponent<Collider>();
                    Vector3 surfaceOffset = (col != null ? col.bounds.extents.y * Vector3.up : Vector3.zero);
                    targetPos += surfaceOffset;
                }
                targetPos += config.offset;
                
                config.player.transform.position = targetPos;
                Debug.Log($"Player moved to {targetPos}");
                
                // CTRL modifier triggers play mode
                if (e.control)
                    EditorApplication.EnterPlaymode();
                
                e.Use();
            }
        }
    }
}
