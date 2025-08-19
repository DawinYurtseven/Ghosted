using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;


public class SceneViewStateToggle : EditorWindow
{
    [MenuItem("Project Windows/Scene View State Toggle")]
    public static void ShowExample()
    {
        SceneViewStateToggle wnd = GetWindow<SceneViewStateToggle>();
        wnd.titleContent = new GUIContent("Scene State Toggle");
    }
    
    static GameObject stateJoy;
    static GameObject stateFear;

    public void CreateGUI()
    {
        //Debug.Log("CreateGUI called");
        VisualElement root = rootVisualElement;
        
        Button joyButton = new Button();
        joyButton.name = "Joy";
        joyButton.text = "Joy";
        joyButton.clicked += () => SetEmotion(Emotion.Joy); 
        
        root.Add(joyButton);
        
        Button fearButton = new Button();
        fearButton.name = "Fear";
        fearButton.text = "Fear";
        fearButton.clicked += () =>  SetEmotion(Emotion.Fear);
        root.Add(fearButton);
    }

    void SetEmotion(Emotion emotion)
    {
        if (EmotionSingletonMock.Instance == null)
        {
            Debug.LogWarning("EmotionSingletonMock instance not found. Attempting to find it in the scene.");
            EmotionSingletonMock.Instance = FindObjectOfType<EmotionSingletonMock>(true);
        }

        if (EmotionSingletonMock.Instance != null)
            EmotionSingletonMock.Instance.ChangeEmotion(emotion);

        ApplyEmotionToMaterialChangers(emotion);

        if (FindObjectsInScene())
        {
            bool isJoy = (emotion == Emotion.Joy);
            if (stateJoy) stateJoy.SetActive(isJoy);
            if (stateFear) stateFear.SetActive(!isJoy);
        }

        SceneView.RepaintAll();
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.IsValid()) EditorSceneManager.MarkSceneDirty(scene);

    }

    static void ApplyEmotionToMaterialChangers(Emotion emotion)
    {
        // Grab ALL MaterialChanger instances, including inactive
        var changers = Resources.FindObjectsOfTypeAll<MaterialChanger>();

        foreach (var mc in changers)
        {
            var go = mc.gameObject;

            // Skip objects not in an open scene (e.g., assets/prefabs)
            if (!go.scene.IsValid() || !go.scene.isLoaded) continue;

            // Skip hidden editor objects if any
            if ((mc.hideFlags & HideFlags.NotEditable) != 0) continue;

            var r = mc.GetComponent<Renderer>();
            if (!r) continue;

            // Record Undo on the Renderer (material swap happens there)
            Undo.RecordObject(r, "Swap Emotion Material");

            // Apply via component API (handles sharedMaterial in edit mode)
            mc.ChangeMaterial(emotion);

            // Persist the change
            EditorUtility.SetDirty(r);
        }
    }

    private void OnEnable()
    {
        FindObjectsInScene();
    }

    private static bool FindObjectsInScene()
    {
        // Find both active and inactive objects with the specified name
        GameObject[] allObjects = FindObjectsOfType<GameObject>(true); // True to include inactive objects

        var scene = EditorSceneManager.GetActiveScene();
        var rootObjects = scene.GetRootGameObjects();

        stateJoy = null;
        stateFear = null;

        foreach (var root in rootObjects)
        {
            foreach (var obj in root.GetComponentsInChildren<Transform>(true))
            {
                if (obj.name == "Joy")
                    stateJoy = obj.gameObject;
                else if (obj.name == "Fear")
                    stateFear = obj.gameObject;

                if (stateJoy && stateFear) break;
            }
            if (stateJoy && stateFear) break;
        }

        if (stateJoy == null)
            Debug.LogWarning("No GameObject named 'Joy' found in the scene.");

        if (stateFear == null)
            Debug.LogWarning("No GameObject named 'Fear' found in the scene.");
        return stateJoy != null && stateFear != null;
    }
}