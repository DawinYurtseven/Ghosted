using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueWindowController))]
public class DialogueWindowControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DialogueWindowController controller = (DialogueWindowController)target;

        GUILayout.Space(10);
        if (GUILayout.Button("Save Layout as ScriptableObject"))
        {
            SaveLayout(controller);
        }
    }

    private void SaveLayout(DialogueWindowController controller)
    {
        string folderPath = "Assets/Data/DialogueWindowLayoutData";
        string assetName = controller.gameObject.name + "_Layout.asset";
        string fullPath = System.IO.Path.Combine(folderPath, assetName);

        if (!AssetDatabase.IsValidFolder("Assets/Data"))
            AssetDatabase.CreateFolder("Assets", "Data");

        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets/Data", "DialogueWindowLayoutData");

        if (!string.IsNullOrEmpty(fullPath))
        {
            DialogueWIndowLayoutData layout = ScriptableObject.CreateInstance<DialogueWIndowLayoutData>();
            layout.widthPercent = controller.widthPercent;
            layout.heightPercent = controller.heightPercent;
            layout.leftPercent = controller.leftOffsetPercent;
            layout.topPercent = controller.topOffsetPercent;

            AssetDatabase.CreateAsset(layout, fullPath);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = layout;
        }
    }
}