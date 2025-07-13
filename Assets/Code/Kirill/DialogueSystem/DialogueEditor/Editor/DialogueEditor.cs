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
        GUIStyle textStyle;
        [NonSerialized]
        DialogueEditorNode draggingNode;
        [NonSerialized]
        Vector2 draggingOffset = Vector2.zero;
        [NonSerialized]
        IHasChildren creatingDialogueNode = null;
        [NonSerialized]
        IHasChildren creatingReplyNode = null;
        [NonSerialized]
        ReplyNode creatingReplyNodeChild = null;
        [NonSerialized]
        ReplyNode toBeDeletedReplyNode = null;
        [NonSerialized]
        DialogueNode toBeDeletedDialogueNode = null;
        [NonSerialized]
        Reply toBeDeletedReply = null;
        [NonSerialized]
        ReplyNode deletingReplyReplyNode = null;
        [NonSerialized]
        IHasChildren linkingParentNode = null;
        Vector2 scrollPosition;
        [NonSerialized]
        bool draggingCanvas = false;
        [NonSerialized]
        Vector2 draggingCanvasOffseet;

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
            var dialogue = Selection.activeObject as Dialogue;
            if (dialogue == null) {
            } else {
                selectedDialogue = dialogue;
                Repaint();
            }
        }

        void OnGUI() {
            if (selectedDialogue == null)
            {

            }
            else
            {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                GUILayoutUtility.GetRect(4000, 4000);

                foreach (var node in selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }
                foreach (var node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();

                if (creatingDialogueNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Added Dialogue Node");
                    selectedDialogue.CreateDialogueNode(creatingDialogueNode);
                    creatingDialogueNode = null;
                }
                else if (creatingReplyNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Added Reply Node");
                    selectedDialogue.CreateReplyNode(creatingReplyNode);
                    creatingReplyNode = null;
                }
                if (toBeDeletedDialogueNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Deleted Dialogue Node");
                    selectedDialogue.DeleteDialogueNode(toBeDeletedDialogueNode);
                    toBeDeletedDialogueNode = null;
                }
                if (creatingReplyNodeChild != null)
                {
                    Undo.RecordObject(selectedDialogue, "Added New Reply");
                    selectedDialogue.CreateReply(creatingReplyNodeChild);
                    creatingReplyNodeChild = null;
                }
                if (toBeDeletedReplyNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Deleted Reply Node");
                    selectedDialogue.DeleteReplyNode(toBeDeletedReplyNode);
                    creatingReplyNodeChild = null;
                }
                if (toBeDeletedReply != null)
                {
                    Undo.RecordObject(selectedDialogue, "Deleted Reply");
                    selectedDialogue.DeleteReply(deletingReplyReplyNode, toBeDeletedReply);
                    toBeDeletedReply = null;
                    deletingReplyReplyNode = null;
                }
            }
        }

        private void ProcessEvents() {
            switch (Event.current.type) {
                case EventType.MouseDown:
                    if (draggingNode == null)
                    {
                        draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);

                        if (draggingNode != null)
                        {
                            draggingOffset = draggingNode.rect.position - Event.current.mousePosition;
                            Selection.activeObject = draggingNode;
                        }
                        else
                        {
                            draggingCanvas = true;
                            draggingCanvasOffseet = Event.current.mousePosition + scrollPosition;
                            Selection.activeObject = selectedDialogue;
                        }
                    }
                    break;
                case EventType.MouseUp:
                    if (draggingNode != null) {
                        draggingNode = null;
                    }
                    if (draggingCanvas)
                    {
                        draggingCanvas = false;
                    }
                    break;
                case EventType.MouseDrag:
                    if (draggingNode != null) {
                        Undo.RecordObject(selectedDialogue, "Move Dialogue Node");
                        draggingNode.rect.position = Event.current.mousePosition + draggingOffset;
                        GUI.changed = true;
                    }
                    if (draggingCanvas)
                    {
                        scrollPosition = draggingCanvasOffseet - Event.current.mousePosition;
                        GUI.changed = true;
                    }
                    break;
            }
        }

        private void DrawNode(DialogueEditorNode potentialNode) {
            if (potentialNode is DialogueNode dialogueNode)
            {
                DrawDialogueNode(dialogueNode);
            }
            else if (potentialNode is ReplyNode replyNode)
            {
                DrawReplyNode(replyNode);
            }
        }

        private void DrawDialogueNode(DialogueNode node) {

            textStyle = GUI.skin.textArea; // call only in OnGUI?
            float width = 160;
            float height = textStyle.CalcHeight(new GUIContent(node.text), width);
            Rect newRect = new Rect(node.rect);
            newRect.height += height + 20; // for audioclip reference

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
                if (GUILayout.Button("link")) {
                    linkingParentNode = node;
                } 
            }
            else {
                if (((DialogueEditorInstance)linkingParentNode).name == node.name) {
                    if (GUILayout.Button("cancel")) {
                        linkingParentNode = null;
                        //creatingNode = node;
                    }
                } else {
                    if (linkingParentNode.Child == node.name) {
                        if (GUILayout.Button("unchild")) {
                            Undo.RecordObject(selectedDialogue, "Remove Dialogue Link");
                            selectedDialogue.RemoveChild(linkingParentNode);
                            linkingParentNode = null;
                            //creatingNode = node;
                        }
                    } else {
                        if (GUILayout.Button("child")) {
                            Undo.RecordObject(selectedDialogue, "Add Dialogue Link");
                            selectedDialogue.AddChild(linkingParentNode, node);
                            linkingParentNode = null;
                            //creatingNode = node;
                        }
                    }
                }
            }

            if (GUILayout.Button("-")) {
                toBeDeletedDialogueNode = node;
            }

            GUILayout.EndHorizontal();

            // Creating Drop Menu for choosing Speakers
            if (GUILayout.Button(node.speaker)) {
                GenericMenu menu = new GenericMenu();
                foreach (string speaker in selectedDialogue.GetSpeakers()) {
                    string capturedSpeaker = speaker;
                    menu.AddItem(new GUIContent(capturedSpeaker), capturedSpeaker == node.speaker, () => {
                        node.speaker = capturedSpeaker;}
                    );
                }
                menu.ShowAsContext();
            }
            node.voiceClip = (AudioClip)EditorGUILayout.ObjectField(node.voiceClip, typeof(AudioClip), false, GUILayout.MinWidth(150));
            GUILayout.EndArea();
        }

        private void DrawReplyNode(ReplyNode node) {
            GUILayout.BeginArea(node.rect, nodeStyle);
            GUILayout.BeginVertical();
            foreach (Reply reply in node.replies)
            {
                DrawReply(node, reply);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+")) {
                creatingReplyNodeChild = node;
            }
            if (GUILayout.Button("-")) {
                toBeDeletedReplyNode = node;
            }
            if (linkingParentNode != null) {
                if (linkingParentNode.Child == node.name) {
                    if (GUILayout.Button("unchild")) {
                        Undo.RecordObject(selectedDialogue, "Remove Dialogue Link");
                        selectedDialogue.RemoveChild(linkingParentNode);
                        linkingParentNode = null;
                        //creatingNode = node;
                    }
                } else {
                    if (GUILayout.Button("child")) {
                        Undo.RecordObject(selectedDialogue, "Add Dialogue Link");
                        selectedDialogue.AddChild(linkingParentNode, node);
                        linkingParentNode = null;
                        //creatingNode = node;
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.EndArea();
        }

        private void DrawReply(ReplyNode node, Reply reply)
        {
            EditorGUI.BeginChangeCheck();

            //float width = EditorGUIUtility.currentViewWidth - 40;
            string newText = EditorGUILayout.TextField(reply.text);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, "Update Dialogue Text");

                reply.text = newText;
            }

            GUILayout.BeginHorizontal();
            if (selectedDialogue.GetNodeFromId(reply.Child) == null)
            {
                if (GUILayout.Button("+D"))
                {
                    creatingDialogueNode = reply;
                }
                if (GUILayout.Button("+R"))
                {
                    creatingReplyNode = reply;
                }
            }
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("link"))
                {
                    linkingParentNode = reply;
                }
            }
            else
            {
                if (((ScriptableObject)linkingParentNode).name == reply.name)
                {
                    if (GUILayout.Button("cancel"))
                    {
                        linkingParentNode = null;
                        //creatingNode = node;
                    }
                }
            }

            if (GUILayout.Button("-"))
            {
                toBeDeletedReply = reply;
                deletingReplyReplyNode = node;
            }

            GUILayout.EndHorizontal();
        }

        private void DrawReplyFromDialogueNode(DialogueNode node)
        {
            EditorGUI.BeginChangeCheck();

            //float width = EditorGUIUtility.currentViewWidth - 40;
            string newText = EditorGUILayout.TextField(node.text);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, "Update Dialogue Text");

                node.text = newText;
            }

            GUILayout.BeginHorizontal();
            if (selectedDialogue.GetChild(node) == null)
            {
                if (GUILayout.Button("+D"))
                {
                    creatingDialogueNode = node;
                }
                if (GUILayout.Button("+R"))
                {
                    creatingReplyNode = node;
                }
            }
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("link"))
                {
                    linkingParentNode = node;
                }
            }
            else
            {
                if (((ScriptableObject)linkingParentNode).name == node.name)
                {
                    if (GUILayout.Button("cancel"))
                    {
                        linkingParentNode = null;
                        //creatingNode = node;
                    }
                }
            }

            if (GUILayout.Button("-"))
            {
                toBeDeletedDialogueNode = node;
            }

            GUILayout.EndHorizontal();
        }

        private void DrawConnections(DialogueEditorNode potentialNode) {
            if (potentialNode is DialogueNode dialogueNode)
            {
                DrawDialogueNodeConnections(dialogueNode);
            }
            else if (potentialNode is ReplyNode replyNode)
            {
                DrawReplyNodeConnections(replyNode);
            }
        }

        private void DrawDialogueNodeConnections(DialogueNode node) {
            Vector3 startPosition = new Vector2(node.rect.xMax, node.rect.center.y);
            DialogueEditorNode childNode = selectedDialogue.GetNodeFromId(node.Child);
            if (childNode != null) {
                DrawBezierConnectionToNode(startPosition, childNode);
            }
        }

        private void DrawReplyNodeConnections(ReplyNode node)
        {
            for (int i = 0; i < node.replies.Count; i++)
            {
                Reply reply = node.replies[i];
                Vector3 startPosition = new Vector2(node.rect.max.x, node.rect.min.y) + new Vector2(5f, 40f + i * 40f);
                DialogueEditorNode childNode = selectedDialogue.GetNodeFromId(reply.Child);
                if (childNode != null)
                {
                    DrawBezierConnectionToNode(startPosition, childNode);
                }
            }
        }

        private void DrawBezierConnectionToNode(Vector3 startPosition, DialogueEditorNode node)
        {
            Vector3 endPosition = new Vector2(node.rect.xMin, node.rect.center.y);
            Vector3 controlPointOffset = endPosition - startPosition;
            controlPointOffset.y = 0;
            controlPointOffset.x *= 0.6f;
            Handles.DrawBezier(startPosition, endPosition,
                startPosition + controlPointOffset,
                endPosition - controlPointOffset,
                Color.white, null, 4f);
        }

        private DialogueEditorNode GetNodeAtPoint(Vector2 point)
        {
            return selectedDialogue.GetSelectedNode(point);
        }
    }
}
