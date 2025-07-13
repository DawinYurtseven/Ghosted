using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueMenuController))]
public class DialogueMenuControllerEditor : Editor
{
public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DialogueMenuController controller = (DialogueMenuController)target;

        GUILayout.Space(10);
        if (GUILayout.Button("Save Layout as ScriptableObject"))
        {
            SaveLayout(controller);
        }
    }

    private void SaveLayout(DialogueMenuController controller)
    {
        string folderPath = "Assets/Data/DialogueMenuLayoutData";
        string assetName = controller.gameObject.name + "_LayoutTYPE.asset";
        string fullPath = System.IO.Path.Combine(folderPath, assetName);

        if (!AssetDatabase.IsValidFolder("Assets/Data"))
            AssetDatabase.CreateFolder("Assets", "Data");

        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets/Data", "DialogueMenuLayoutData");

        if (!string.IsNullOrEmpty(fullPath))
        {
            DialogueMenuLayoutData layout = ScriptableObject.CreateInstance<DialogueMenuLayoutData>();
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
