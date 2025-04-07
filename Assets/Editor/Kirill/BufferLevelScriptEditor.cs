using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BufferLevelScript))]
public class BufferLevelScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        //DrawDefaultInspector(); // basically the same as base.OnInspectorGUI()
        BufferLevelScript myLevelScript = (BufferLevelScript) target;

        myLevelScript.exp = EditorGUILayout.
            IntField("Experience", myLevelScript.exp);

        EditorGUILayout.LabelField("Level", myLevelScript.Level.ToString());
        if (GUILayout.Button("Press to do something")) {
            myLevelScript.DoSomething();
        }
    }
}
