using UnityEngine;
using UnityEditor;

public class ReplaceWithPrefabTool : EditorWindow
{
    private GameObject prefabToReplaceWith;
    private Vector3 positionOffset = Vector3.zero;
    private Vector3 rotationOffset = Vector3.zero;
    private Vector3 scaleOffset = Vector3.one;

    [MenuItem("GameObject/Replace With Prefab...", false, 0)]
    static void ShowWindow()
    {
        GetWindow<ReplaceWithPrefabTool>("Replace With Prefab");
    }

    private void OnGUI()
    {
        GUILayout.Label("Replacement Settings", EditorStyles.boldLabel);

        prefabToReplaceWith = (GameObject)EditorGUILayout.ObjectField("Prefab", prefabToReplaceWith, typeof(GameObject), false);
        positionOffset = EditorGUILayout.Vector3Field("Position Offset", positionOffset);
        rotationOffset = EditorGUILayout.Vector3Field("Rotation Offset", rotationOffset);
        scaleOffset = EditorGUILayout.Vector3Field("Scale Multiplier", scaleOffset);

        if (GUILayout.Button("Replace Selected"))
        {
            if (prefabToReplaceWith == null)
            {
                Debug.LogError("No prefab selected.");
                return;
            }

            ReplaceSelectedObjects();
        }
    }

    private void ReplaceSelectedObjects()
    {
        GameObject[] selected = Selection.gameObjects;

        foreach (GameObject obj in selected)
        {
            Transform t = obj.transform;

            GameObject newObj = (GameObject) PrefabUtility.InstantiatePrefab(prefabToReplaceWith, t.parent);
            Undo.RegisterCreatedObjectUndo(newObj, "Replace With Prefab");

            newObj.transform.localPosition = t.localPosition + positionOffset;
            newObj.transform.localRotation = t.localRotation * Quaternion.Euler(rotationOffset);
            newObj.transform.localScale = Vector3.Scale(t.localScale, scaleOffset);
            
            Undo.DestroyObjectImmediate(obj);
        }

        Debug.Log($"Replaced {selected.Length} object(s) with prefab.");
    }
}

