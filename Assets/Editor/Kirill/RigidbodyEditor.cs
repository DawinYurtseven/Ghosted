using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Rigidbody))]
public class RigidbodyEditor : Editor
{
    // Everything with defaultEditor: Default values of Rigidbody from Unity
    private Editor defaultEditor;
    void OnEnable()
    {
        defaultEditor = CreateEditor(targets, typeof(Editor).Assembly.GetType("UnityEditor.RigidbodyEditor"));
    }

    void OnDisable()
    {
        if (defaultEditor != null)
            DestroyImmediate(defaultEditor);
    }

public override void OnInspectorGUI()
    {
        if (defaultEditor != null)
            defaultEditor.OnInspectorGUI();

        Rigidbody rb = target as Rigidbody;

        if (Application.isPlaying) {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Live Rigidbody Info", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Velocity", rb.velocity.ToString("F3"));
            EditorGUILayout.LabelField("Angular Velocity", rb.angularVelocity.ToString("F3"));
            EditorGUILayout.LabelField("Is Sleeping", rb.IsSleeping().ToString());
            EditorGUILayout.LabelField("Use Gravity", rb.useGravity.ToString());
        }
        else {
            EditorGUILayout.HelpBox("Live Rigidbody Info in playmode only.", MessageType.Info);
            rb.velocity = EditorGUILayout.Vector3Field("Start Velocity", rb.velocity);
        }
        SceneView.RepaintAll();
    }

    [DrawGizmo(GizmoType.Pickable | GizmoType.Selected)]
    static void DrawGizmosSelected(Rigidbody rb, GizmoType gizmoType)
    {
        var startPos = rb.gameObject.transform.position;
        var endPos = startPos + rb.velocity;
        Handles.DrawDottedLine(startPos, endPos, 3);
    }
    void OnSceneGUI() {
        
    }
}
