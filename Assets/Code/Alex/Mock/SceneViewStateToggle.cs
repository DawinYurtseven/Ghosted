﻿using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[InitializeOnLoad]
public static class SceneViewStateToggle
{
    static GameObject stateJoy;
    static GameObject stateFear;
    static bool toggleState = false;

    static SceneViewStateToggle()
    {
        // Search for objects named "Joy" and "Fear" when the editor starts or scene view is updated
        if(FindObjectsInScene()) 
            // Listen to the SceneView GUI updates
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
                EditorGUILayout.HelpBox("Couldn't find both 'Joy' and 'Fear' objects in the scene.",
                    MessageType.Warning);
            }
        }

        GUILayout.EndArea();
        Handles.EndGUI();
    }

    private static bool FindObjectsInScene()
    {
        // Find both active and inactive objects with the specified name
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(true); // True to include inactive objects

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "Joy")
            {
                stateJoy = obj;
            }
            else if (obj.name == "Fear")
            {
                stateFear = obj;
            }

            if (stateFear && stateJoy) break;
        }

        if (stateJoy == null)
            Debug.LogWarning("No GameObject named 'Joy' found in the scene.");

        if (stateFear == null)
            Debug.LogWarning("No GameObject named 'Fear' found in the scene.");
        return stateJoy != null && stateFear != null;
    }
}

/*
[CustomEditor(typeof(SceneViewStateToggle))]
public class SceneViewStateToggleEditor : Editor
{

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        root.Add(new PropertyField(GameObject));
    }
}*/