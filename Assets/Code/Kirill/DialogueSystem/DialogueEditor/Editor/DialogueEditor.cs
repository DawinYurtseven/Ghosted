using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using Codice.Client.Common.TreeGrouper;

namespace Ghosted.Dialogue.Editor{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue;
        [NonSerialized]
        GUIStyle nodeStyle;
        [NonSerialized]
        GUIStyle textStyle;
        [NonSerialized]
        DialogueEditorNode draggingNode;
        [NonSerialized]
        Vector2 draggingOffset = Vector2.zero;
        [NonSerialized]
        DialogueNode creatingDialogueNode = null;
        [NonSerialized]
        DialogueNode creatingReplyNode = null;
        [NonSerialized]
        ReplyNode creatingReplyNodeChild = null;
        [NonSerialized]
        ReplyNode deletingReplyNode = null;
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

                GUILayoutUtility.GetRect(4000, 4000);

                foreach (var node in selectedDialogue.GetAllNodes()) {
                    DrawConnections(node);
                }
                foreach (var node in selectedDialogue.GetAllNodes()) {
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();

                if (creatingDialogueNode != null) {
                    Undo.RecordObject(selectedDialogue, "Added Dialogue Node");
                    selectedDialogue.CreateDialogueNode(creatingDialogueNode);
                    creatingDialogueNode = null;
                } else if (creatingReplyNode != null) {
                    Undo.RecordObject(selectedDialogue, "Added Reply Node");
                    selectedDialogue.CreateReplyNode(creatingReplyNode);
                    creatingReplyNode = null;
                }
                if (toBeDeletedNode != null) {
                    Undo.RecordObject(selectedDialogue, "Deleted Dialogue Node");
                    selectedDialogue.DeleteDialogueNode(toBeDeletedNode);
                    toBeDeletedNode = null;
                }
                if (creatingReplyNodeChild != null) {
                    Undo.RecordObject(selectedDialogue, "Added New Reply");
                    selectedDialogue.AddReply(creatingReplyNodeChild);
                    creatingReplyNodeChild = null;
                }
                if (deletingReplyNode != null) {
                    Undo.RecordObject(selectedDialogue, "Deleted Reply Node");
                    selectedDialogue.DeleteReplyNode(deletingReplyNode);
                    creatingReplyNodeChild = null;
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
                        if (draggingNode as ReplyNode != null) {
                            selectedDialogue.UpdateChildrenPosition((ReplyNode) draggingNode);
                        }
                        GUI.changed = true;
                    }
                    break;
            }
        }

        private void DrawNode(DialogueEditorNode potentialNode) {
            if (potentialNode is DialogueNode dialogueNode) {
                if (!((DialogueNode)potentialNode).isReplyNodeChild) {
                    DrawDialogueNode(dialogueNode);
                } // Other will be drawn as reply node children
            } else if (potentialNode is ReplyNode replyNode) {
                DrawReplyNode(replyNode);
            }
        }

        private void DrawDialogueNode(DialogueNode node) {
            Debug.Log("Draw dialogue Node: " + node);

            textStyle = GUI.skin.textArea; // call only in OnGUI?
            float width = 160;
            float height = textStyle.CalcHeight(new GUIContent(node.text), width);
            Rect newRect = new Rect(node.rect);
            newRect.height += height;

            GUILayout.BeginArea(newRect, nodeStyle);
            EditorGUI.BeginChangeCheck();

            //float width = EditorGUIUtility.currentViewWidth - 40;
            string newText = EditorGUILayout.TextArea(node.text, textStyle, GUILayout.Height(height));

            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(selectedDialogue, "Update Dialoge Text");

                node.text = newText;
            }

            GUILayout.BeginHorizontal();
            if (selectedDialogue.GetChild(node) == null) {
                if (GUILayout.Button("+D")) {
                    creatingDialogueNode = node;
                }
                if (GUILayout.Button("+R")) {
                    creatingReplyNode = node;
                }
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
                    if (linkingParentNode.child.Contains(node.id)) {
                        if (GUILayout.Button("unlink")) {
                            Undo.RecordObject(selectedDialogue, "Remove Dialogue Link");
                            selectedDialogue.RemoveChild(linkingParentNode);
                            linkingParentNode = null;
                            //creatingNode = node;
                        }
                    } else {
                        if (GUILayout.Button("child")) {
                            Undo.RecordObject(selectedDialogue, "Add Dialogue Link");
                            selectedDialogue.RemoveChild(linkingParentNode);
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

        private void DrawReplyNode(ReplyNode node) {
            GUILayout.BeginArea(node.rect, nodeStyle);
            GUILayout.BeginVertical();
            foreach (string dialogueNodeId in node.replies) {
                DialogueNode dialogueNode = selectedDialogue.GetDialogueNode(dialogueNodeId);
                if (dialogueNode != null) {
                    DrawReplyFromDialogueNode(dialogueNode);
                }
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+")) {
                creatingReplyNodeChild = node;
            }
            if (GUILayout.Button("-")) {
                deletingReplyNode = node;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            /*GUILayout.BeginHorizontal();
            if (selectedDialogue.GetChild(node) == null) {
                if (GUILayout.Button("+D")) {
                    creatingDialogueNode = node;
                }
                if (GUILayout.Button("+R")) {
                    creatingReplyNode = node;
                }
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
                    if (linkingParentNode.child.Contains(node.id)) {
                        if (GUILayout.Button("unlink")) {
                            Undo.RecordObject(selectedDialogue, "Remove Dialogue Link");
                            selectedDialogue.RemoveChild(linkingParentNode);
                            linkingParentNode = null;
                            //creatingNode = node;
                        }
                    } else {
                        if (GUILayout.Button("child")) {
                            Undo.RecordObject(selectedDialogue, "Add Dialogue Link");
                            selectedDialogue.RemoveChild(linkingParentNode);
                            linkingParentNode = null;
                            //creatingNode = node;
                        }
                    }
                }
            }

            if (GUILayout.Button("-")) {
                toBeDeletedNode = node;
            }

            GUILayout.EndHorizontal();*/

            GUILayout.EndArea();
        }

        private void DrawReplyFromDialogueNode(DialogueNode node) {
            EditorGUI.BeginChangeCheck();

            //float width = EditorGUIUtility.currentViewWidth - 40;
            string newText = EditorGUILayout.TextField(node.text);

            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(selectedDialogue, "Update Dialogue Text");

                node.text = newText;
            }

            GUILayout.BeginHorizontal();
            if (selectedDialogue.GetChild(node) == null) {
                if (GUILayout.Button("+D")) {
                    creatingDialogueNode = node;
                }
                if (GUILayout.Button("+R")) {
                    creatingReplyNode = node;
                }
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
                    if (linkingParentNode.child.Contains(node.id)) {
                        if (GUILayout.Button("unlink")) {
                            Undo.RecordObject(selectedDialogue, "Remove Dialogue Link");
                            selectedDialogue.RemoveChild(linkingParentNode);
                            linkingParentNode = null;
                            //creatingNode = node;
                        }
                    } else {
                        if (GUILayout.Button("child")) {
                            Undo.RecordObject(selectedDialogue, "Add Dialogue Link");
                            selectedDialogue.RemoveChild(linkingParentNode);
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
        }

        private void DrawConnections(DialogueEditorNode potentialNode) {
            if (potentialNode is DialogueNode dialogueNode) {
                DrawDialogueNodeConnections(dialogueNode);
            } else if (potentialNode is ReplyNode replyNode) {
                DrawReplyNodeConnections(replyNode);
            }
        }

        private void DrawDialogueNodeConnections(DialogueNode node) {
            Vector3 startPosition = new Vector2(node.rect.xMax, node.rect.center.y);
            DialogueEditorNode childNode = selectedDialogue.GetChild(node);
            if (childNode != null) {
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

        private void DrawReplyNodeConnections(ReplyNode node) {
            foreach (string dialogueNode in node.replies) {
                selectedDialogue.GetDialogueNode(dialogueNode);
            }
        }

        private DialogueEditorNode GetNodeAtPoint(Vector2 point)
        {
            DialogueEditorNode node = selectedDialogue.GetSelectedNode(point);
            if (node as DialogueNode != null) {
                if (((DialogueNode)node).isReplyNodeChild) {
                    return selectedDialogue.GetNodeFromId(((DialogueNode)node).replyNodeParentId);
                }
            }
            return selectedDialogue.GetSelectedNode(point);
        }
    }
}
