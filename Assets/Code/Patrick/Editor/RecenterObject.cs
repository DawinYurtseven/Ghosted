using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class RecenterObject : EditorWindow
{
    private static string customShortcut = "Ctrl+Shift+Z"; // Default shortcut display
    private static bool recursive = false;
    private static bool buttonMode = false;

    static RecenterObject()
    {
        // Subscribe to EditorApplication.update to listen for key events globally
        SceneView.duringSceneGui += OnSceneGUI;
    }
    
    private static void OnSceneGUI(SceneView sceneView)
    {
        if (buttonMode)
        {
            // Begin Toolbar GUI in Scene View
            Handles.BeginGUI();
        
            GUILayout.BeginArea(new Rect(10, 10, 150, 50)); // Adjust position and size
            if (GUILayout.Button("Center Parent", GUILayout.Width(120)))
            {
                GameObject selectedParent = Selection.activeGameObject;
                if (selectedParent == null)
                {
                    Debug.LogWarning("No object selected! Please select a valid parent object in the Hierarchy.");
                }
                else
                {
                    CenterParent(selectedParent, recursive);
                }
            }
            GUILayout.EndArea();

            Handles.EndGUI();
        }
        else    // The shortcut mode that definitly works
        {
            if (Event.current != null && Event.current.type == EventType.KeyDown)
            {
                // Detect if Ctrl + Shift + Z is pressed in SceneView
                if (Event.current.control && Event.current.shift && Event.current.keyCode == KeyCode.Z)
                {
                    Debug.Log("Ctrl+Shift+Z pressed in SceneView!");
                    CenterParentShortcut();
                    Event.current.Use(); // Prevent further event propagation
                }
            }
        }
    }

    [MenuItem("Tools/Center Parent to Children (Default Shortcut %#z)")] // Default shortcut: Ctrl+Shift+C
    public static void CenterParentShortcut()
    {
        GameObject selectedParent = Selection.activeGameObject;

        if (selectedParent == null)
        {
            Debug.LogWarning("No object selected! Please select a valid parent object in the Hierarchy.");
            return;
        }

        CenterParent(selectedParent, recursive);
    }

    [MenuItem("Tools/Center Parent to Children (Open Tool)")]
    public static void ShowWindow()
    {
        GetWindow<RecenterObject>("Center Parent to Children");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Center Parent to Children", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // Select parent object
        EditorGUILayout.LabelField("Selected Parent Object:", EditorStyles.boldLabel);
        GameObject selectedParent = Selection.activeGameObject;
        EditorGUILayout.ObjectField("Parent Object", selectedParent, typeof(GameObject), true);

        GUILayout.Space(10);

        // Recursive option
        recursive = EditorGUILayout.Toggle("Apply Recursively", recursive);
        
        // Button option
        buttonMode = EditorGUILayout.Toggle("Use Button-Mode", buttonMode);
        
        // Shortcut customization
        GUILayout.Label("Custom Shortcut (Info):", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Current Shortcut:", customShortcut);
        EditorGUILayout.HelpBox("To change the shortcut, manually edit the MenuItem attribute in the script.", MessageType.Info);

        
        GUILayout.Space(10);

        if (GUILayout.Button("Center Parent"))
        {
            if (selectedParent == null)
            {
                Debug.LogWarning("No object selected! Please select a valid parent object in the Hierarchy.");
                return;
            }

            CenterParent(selectedParent, recursive);
        }
    }
    
    private static void CenterParent(GameObject parent, bool recursive)
    {
        if (parent.transform.childCount == 0)
        {
            Debug.LogWarning($"The selected parent '{parent.name}' has no child objects!");
            return;
        }

        if (!IsEmptyGameObject(parent))
        {
            Debug.LogWarning(parent.name + " is not empty, this leads to errors!");
            return;
        }
        
        if (recursive)
        {
            // Perform a bottom-up traversal of all children
            foreach (Transform child in parent.transform)
            {
                CenterParent(child.gameObject, true); // Recursive call for children
            }
        }

        // Calculate the bounding box of all child objects in world space
        Bounds bounds = new Bounds(parent.transform.GetChild(0).position, Vector3.zero);
        foreach (Transform child in parent.transform)
        {
            bounds.Encapsulate(child.position);
        }

        Vector3 boundsCenter = bounds.center; // World-space center of the bounding box

        // Adjust parent position while keeping child world positions
        Vector3 parentOldPosition = parent.transform.position;
        parent.transform.position = boundsCenter;

        foreach (Transform child in parent.transform)
        {
            child.position -= (boundsCenter - parentOldPosition); // Adjust children to maintain their world positions
        }

        Debug.Log($"Parent object '{parent.name}' has been centered to {boundsCenter}.");
    }
    
    public static bool IsEmptyGameObject(GameObject obj)
    {
        if (!obj) return false;
        // Check if the object has more than just the Transform component
        if (obj.GetComponents<Component>().Length <= 1)
        {
            return true;
        }

        return false;
    }
}