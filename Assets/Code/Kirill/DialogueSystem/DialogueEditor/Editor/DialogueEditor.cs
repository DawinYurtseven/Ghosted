using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;

namespace Ghosted.Dialogue.Editor{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue;
        [NonSerialized]
        GUIStyle nodeStyle;
        [NonSerialized]
        DialogueNode draggingNode;
        [NonSerialized]
        Vector2 draggingOffset = Vector2.zero;
        [NonSerialized]
        DialogueNode creatingNode = null;
        [NonSerialized]
        DialogueNode toBeDeletedNode = null;
        [NonSerialized]
        DialogueNode linkingParentNode = null;
        Vector2 scrollPosition;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow() {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line) {
            var dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue != null) {
                ShowEditorWindow();
                return true;
            }
            return false;
        }

        private void OnEnable() {
            Debug.Log("OnEnableCalled");
            Selection.selectionChanged += OnSelectionChanged;

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
        }
        private void OnSelectionChanged() {
            Debug.Log("OnSelectionChangedCalled");
            var dialogue = Selection.activeObject as Dialogue;
            if (dialogue == null) {
                Debug.Log("It is not a dialog, I am sorry :(");
            } else {
                Debug.Log("It is indeed a dialog, good job!");
                selectedDialogue = dialogue;
                Repaint();
            }
        }

        void OnGUI() {
            if (selectedDialogue == null) {
                
            } else {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                Debug.Log("ScrollPosition: " + scrollPosition);

                GUILayoutUtility.GetRect(4000, 4000);

                foreach (var node in selectedDialogue.GetAllNodes()) {
                    DrawConnections(node);
                }
                foreach (var node in selectedDialogue.GetAllNodes()) {
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();

                if (creatingNode != null) {
                    Undo.RecordObject(selectedDialogue, "Added Dialogue Node");
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }
                if (toBeDeletedNode != null) {
                    Undo.RecordObject(selectedDialogue, "Deleted Dialogue Node");
                    selectedDialogue.DeleteNode(toBeDeletedNode);
                    toBeDeletedNode = null;
                }
            }
        }

        private void ProcessEvents() {
            switch (Event.current.type) {
                case EventType.MouseDown:
                    if (draggingNode == null) {
                        draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                        if (draggingNode != null) {
                            draggingOffset = draggingNode.rect.position - Event.current.mousePosition;
                        }
                    }
                    break;
                case EventType.MouseUp:
                    if (draggingNode != null) {
                        draggingNode = null;
                    }
                    break;
                case EventType.MouseDrag:
                    if (draggingNode != null) {
                        Undo.RecordObject(selectedDialogue, "Move Dialogue Node");
                        draggingNode.rect.position = Event.current.mousePosition + draggingOffset;
                        GUI.changed = true;
                    }
                    break;
            }
        }

        private void DrawNode(DialogueNode node) {
            GUILayout.BeginArea(node.rect, nodeStyle);
            EditorGUI.BeginChangeCheck();

            string newText = EditorGUILayout.TextField(node.text);

            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(selectedDialogue, "Update Dialoge Text");

                node.text = newText;
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("+")) {
                creatingNode = node;
            }
            if (linkingParentNode == null) {
                if(GUILayout.Button("link")) {
                    linkingParentNode = node;
                } 
            }
            else {
                if (linkingParentNode.id == node.id) {
                    if (GUILayout.Button("cancel")) {
                        linkingParentNode = null;
                        //creatingNode = node;
                    }
                } else {
                    if (linkingParentNode.children.Contains(node.id)) {
                        if (GUILayout.Button("unlink")) {
                            Undo.RecordObject(selectedDialogue, "Remove Dialogue Link");
                            linkingParentNode.children.Remove(node.id);
                            linkingParentNode = null;
                            //creatingNode = node;
                        }
                    } else {
                        if (GUILayout.Button("child")) {
                            Undo.RecordObject(selectedDialogue, "Add Dialogue Link");
                            linkingParentNode.children.Add(node.id);
                            linkingParentNode = null;
                            //creatingNode = node;
                        }
                    }
                }
            }

            if (GUILayout.Button("-")) {
                toBeDeletedNode = node;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawConnections(DialogueNode node) {
            Vector3 startPosition = new Vector2(node.rect.xMax, node.rect.center.y);
            foreach(DialogueNode childNode in selectedDialogue.GetAllChildren(node)) {
                Vector3 endPosition = new Vector2(childNode.rect.xMin, childNode.rect.center.y);
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.6f;
                Handles.DrawBezier(startPosition, endPosition,
                    startPosition + controlPointOffset,
                    endPosition - controlPointOffset,
                    Color.white, null, 4f);
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            return selectedDialogue.GetSelectedNode(point);
        }
    }
}
