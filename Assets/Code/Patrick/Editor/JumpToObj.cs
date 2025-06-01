using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class JumpToObj : EditorWindow
{
    private static bool focusInHierarchy = false; 
    private static bool focusInSceneView = false; 

    [MenuItem("Tools/Auto Focus Selected Object")]
    public static void ShowWindow()
    {
        GetWindow<JumpToObj>("Auto Focus Selected Object");
    }

    private void OnGUI()
    {
        GUILayout.Label("Auto Focus Settings", EditorStyles.boldLabel);

        focusInHierarchy = EditorGUILayout.Toggle("Focus in Hierarchy", focusInHierarchy);

        focusInSceneView = EditorGUILayout.Toggle("Focus in Scene View", focusInSceneView);

        // Display current status
        GUILayout.Space(10);
        GUILayout.Label($"Focus in Hierarchy: {(focusInHierarchy ? "Enabled" : "Disabled")}", EditorStyles.wordWrappedLabel);
        GUILayout.Label($"Focus in Scene View: {(focusInSceneView ? "Enabled" : "Disabled")}", EditorStyles.wordWrappedLabel);

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("When enabled, selecting an object will automatically focus it in the chosen areas.", MessageType.Info);
    }
    
    static JumpToObj()
    {
        Selection.selectionChanged += OnSelectionChanged;
    }
    
    private static void OnSelectionChanged()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if (!selectedObject) return;
        EditorWindow focusedWindow = EditorWindow.focusedWindow;
        if (!focusedWindow) return;
        
        //Actual focus
        if (focusInHierarchy && focusedWindow.titleContent.text != "Hierarchy")
        {
            EditorGUIUtility.PingObject(selectedObject);
            //Debug.Log($"Focused in Hierarchy: {selectedObject.name}");
        }
        if (focusInSceneView)
        {
            SceneView.lastActiveSceneView.FrameSelected();
            //Debug.Log($"Focused in Scene View: {selectedObject.name}");
        }
    }
}