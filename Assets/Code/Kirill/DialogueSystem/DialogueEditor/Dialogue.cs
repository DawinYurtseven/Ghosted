using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;

namespace Ghosted.Dialogue {
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] List<string> dialogueSpeakers = new List<string>();
        [SerializeField] List<DialogueNode> dialogueNodes = new List<DialogueNode>();
        [SerializeField] List<ReplyNode> replyNodes = new List<ReplyNode>();

        Dictionary<string, DialogueNode> dialogueNodeLookup = new Dictionary<string, DialogueNode>();
        Dictionary<string, ReplyNode> replyNodeLookup = new Dictionary<string, ReplyNode>();

#if UNITY_EDITOR
        void Awake()
        {
            //OnValidate();
        }
#endif
        private void OnEnable()
        {

            if (dialogueNodes == null)
                dialogueNodes = new List<DialogueNode>();
            if (replyNodes == null)
                replyNodes = new List<ReplyNode>();
        }

        private void OnValidate()
        {
            dialogueNodeLookup.Clear();
            replyNodeLookup.Clear();
            foreach (DialogueNode node in dialogueNodes)
            {
                dialogueNodeLookup[node.name] = node;
            }
            foreach (ReplyNode node in replyNodes)
            {
                replyNodeLookup[node.name] = node;
            }
        }
        public IEnumerable<DialogueEditorNode> GetAllNodes()
        {
            foreach (var dialogueNode in dialogueNodes)
                yield return dialogueNode;
            foreach (var replyNode in replyNodes)
                yield return replyNode;
        }

        public DialogueNode GetRootNode() {
            return dialogueNodes[0];
        }

        public DialogueEditorNode GetSelectedNode(Vector2 point) {
            DialogueEditorNode foundNode = null;
            foreach (var node in GetAllNodes()) {
                if (node.rect.Contains(point))
                    foundNode = node;
            }
            return foundNode;
        }

        public DialogueEditorNode GetChild(DialogueNode parentNode)
        {
            if (parentNode.Child == null)
                return null;
            if (dialogueNodeLookup.ContainsKey(parentNode.Child))
                return dialogueNodeLookup[parentNode.Child];
            else if (replyNodeLookup.ContainsKey(parentNode.Child))
                return replyNodeLookup[parentNode.Child];
            else
                return null;
        }

        public DialogueNode GetDialogueNodeFromId(string id) {
            if (dialogueNodeLookup.ContainsKey(id))
                return dialogueNodeLookup[id];
            else
                return null;
        }

        public DialogueEditorNode GetNodeFromId(string id) {
            //Debug.Log("I search for: "  + id);
            //Debug.Log(dialogueNodes.Count);
            foreach (var node in dialogueNodes)
            {
              //  Debug.Log(node.Id);
            }
            //Debug.Log(replyNodes.Count);

            foreach (var node in replyNodes)
            {
              //  Debug.Log(node.Id);
            }
            if (dialogueNodeLookup.ContainsKey(id))
                return dialogueNodeLookup[id];
            else if (replyNodeLookup.ContainsKey(id))
                return replyNodeLookup[id];
            else
            {
                //Debug.Log("I return null");
                return null;
            }
        }

        public DialogueNode CreateDialogueNode(IHasChildren parent)
        {
            DialogueNode node = CreateInstance<DialogueNode>();
            node.name = Guid.NewGuid().ToString();
            Undo.RegisterCreatedObjectUndo(node, "Created Dialogue Node");
            if (parent != null)
                parent.Child = node.name;
            dialogueNodes.Add(node);
            if (dialogueSpeakers.Count != 0)
            {
                node.speaker = dialogueSpeakers[0];
            }

            if (Event.current != null)
                node.rect.position = Event.current.mousePosition;

            OnValidate();

            return node;
        }

        public ReplyNode CreateReplyNode(IHasChildren parent)
        {
            ReplyNode node = CreateInstance<ReplyNode>();
            node.name = Guid.NewGuid().ToString();
            Undo.RegisterCreatedObjectUndo(node, "Created Reply Node");
            node.rect.height += 150; // has to be adjusted
            if (parent != null)
                parent.Child = node.name;
            replyNodes.Add(node);

            node.rect.position = Event.current.mousePosition;

            OnValidate();

            return node;
        }

        public void DeleteDialogueNode(DialogueNode nodeToDelete)
        {
            dialogueNodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        public void DeleteReplyNode(ReplyNode nodeToDelete)
        {
            List<Reply> replies = new List<Reply>(nodeToDelete.replies);
            foreach (Reply reply in replies)
            {
                DeleteReply(nodeToDelete, reply);
            }
            replyNodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        public Reply CreateReply(ReplyNode replyNode)
        {
            Reply reply = CreateInstance<Reply>();
            reply.name = Guid.NewGuid().ToString();
            Undo.RegisterCreatedObjectUndo(reply, "Created Reply");
            replyNode.replies.Add(reply);
            OnValidate();
            //UpdateChildrenPosition(replyNode);

            return reply;
        }

        public void DeleteReply(ReplyNode replyNode, Reply reply)
        {
            if (replyNodeLookup.ContainsKey(reply.Child) || dialogueNodeLookup.ContainsKey(reply.Child))
            {
                RemoveChild(reply);
            }
            replyNode.replies.Remove(reply);
            Undo.DestroyObjectImmediate(reply);
        }

        /*public void UpdateChildrenPosition(ReplyNode replyNode)
        {
            float curHeight = 0;
            for (int i = 0; i < replyNode.replies.Count; i++)
            {
                DialogueNode dialogueNode = (DialogueNode)nodeLookup[replyNode.replies[i]];
                dialogueNode.rect.position = new Vector2(replyNode.rect.position.x, replyNode.rect.position.y + curHeight);
                curHeight += dialogueNode.rect.height - 40;
            }
        }*/

        public void RemoveChild(IHasChildren parentNode) {
            parentNode.Child = "removed";
        }

        public void AddChild(IHasChildren parentNode, DialogueEditorNode node) {
            RemoveChild(parentNode);
            parentNode.Child = node.name;
        }

        private void CleanDanglingChildren(DialogueEditorNode nodeToDelete)
        {
            foreach (DialogueEditorNode editorNode in GetAllNodes()) // Only DialogueNodes can bind children
            {
                DialogueNode node = editorNode as DialogueNode;
                if (node != null && node.Child == nodeToDelete.name)
                    RemoveChild(node);
            }
        }

        private void AddRect (Rect to, Rect from) {
            to.height += from.height;
        }
        private void SubRect (Rect from, Rect what) {
            from.height -= what.height;
        }

        public IEnumerable<string> GetSpeakers() {
            foreach (string speaker in dialogueSpeakers) {
                yield return speaker;
            }
        }

        public void OnBeforeSerialize()
        {
            if (dialogueSpeakers.Count == 0)
                dialogueSpeakers.Add("Player");
            if (dialogueNodes.Count == 0)
            {
                var node = CreateDialogueNode(null);
                node.rect.position = new Vector2(0, 0);
            }
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode dialogueNode in dialogueNodes)
                {
                    if (AssetDatabase.GetAssetPath(dialogueNode) == "")
                    {
                        AssetDatabase.AddObjectToAsset(dialogueNode, this);
                    }
                }

                foreach (ReplyNode replyNode in replyNodes)
                {
                    if (AssetDatabase.GetAssetPath(replyNode) == "")
                    {
                        AssetDatabase.AddObjectToAsset(replyNode, this);
                    }
                }

                foreach (ReplyNode replyNode in replyNodes)
                {
                    foreach (Reply reply in replyNode.replies)
                    {
                        if (AssetDatabase.GetAssetPath(reply) == "")
                        {
                            AssetDatabase.AddObjectToAsset(reply, replyNode);
                        }
                    }
                }
            }
        }

        public void OnAfterDeserialize()
        {

        }
    }
}

