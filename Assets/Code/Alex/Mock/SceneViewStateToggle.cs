using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class SceneViewStateToggle
{
    static GameObject stateJoy;
    static GameObject stateFear;
    static bool toggleState = false;

    static SceneViewStateToggle()
    {
        // Search for objects named "Joy" and "Fear" when the editor starts or scene view is updated
        FindObjectsInScene();

        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();

        GUILayout.BeginArea(new Rect(60, 10, 250, 100), "State Toggle", GUI.skin.window);

        // Display the names of the found objects
        if (stateJoy != null)
            EditorGUILayout.LabelField("Joy Object: " + stateJoy.name);
        else
            EditorGUILayout.LabelField("Joy Object: Not Found");

        if (stateFear != null)
            EditorGUILayout.LabelField("Fear Object: " + stateFear.name);
        else
            EditorGUILayout.LabelField("Fear Object: Not Found");

        if (GUILayout.Button("Toggle"))
        {
            if (stateJoy != null && stateFear != null)
            {
                // Toggle the state of both GameObjects
                toggleState = !toggleState;
                stateJoy.SetActive(toggleState);
                stateFear.SetActive(!toggleState);

                // Mark the objects as dirty so that the changes are saved
                EditorUtility.SetDirty(stateJoy);
                EditorUtility.SetDirty(stateFear);
            }
            else
            {
                EditorGUILayout.HelpBox("Couldn't find both 'Joy' and 'Fear' objects in the scene.", MessageType.Warning);
            }
        }

        GUILayout.EndArea();
        Handles.EndGUI();
    }

    private static void FindObjectsInScene()
    {
        stateJoy = GameObject.Find("Joy");
        stateFear = GameObject.Find("Fear");

        if (stateJoy == null)
            Debug.LogWarning("No GameObject named 'Joy' found in the scene.");
        
        if (stateFear == null)
            Debug.LogWarning("No GameObject named 'Fear' found in the scene.");
    }
}
