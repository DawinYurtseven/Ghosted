using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ghosted.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<string> dialogueSpeakers = new List<string>();
        [SerializeField] private List<DialogueNode> dialogueNodes = new List<DialogueNode>();
        [SerializeField] private List<ReplyNode> replyNodes = new List<ReplyNode>();

        private Dictionary<string, DialogueNode> dialogueNodeLookup = new Dictionary<string, DialogueNode>();
        private Dictionary<string, ReplyNode> replyNodeLookup = new Dictionary<string, ReplyNode>();

        private void OnEnable()
        {
            if (dialogueNodes == null) dialogueNodes = new List<DialogueNode>();
            if (replyNodes == null) replyNodes = new List<ReplyNode>();
        }

        private void OnValidate()
        {
            dialogueNodeLookup.Clear();
            replyNodeLookup.Clear();

            foreach (DialogueNode node in dialogueNodes)
                dialogueNodeLookup[node.name] = node;

            foreach (ReplyNode node in replyNodes)
                replyNodeLookup[node.name] = node;
        }

        public IEnumerable<DialogueEditorNode> GetAllNodes()
        {
            foreach (var node in dialogueNodes)
                yield return node;
            foreach (var node in replyNodes)
                yield return node;
        }

        public DialogueNode GetRootNode()
        {
            return dialogueNodes.Count > 0 ? dialogueNodes[0] : null;
        }

        public DialogueEditorNode GetSelectedNode(Vector2 point)
        {
            foreach (var node in GetAllNodes())
            {
                if (node.rect.Contains(point))
                    return node;
            }
            return null;
        }

        public DialogueEditorNode GetChild(DialogueNode parentNode)
        {
            if (string.IsNullOrEmpty(parentNode?.Child))
                return null;

            if (dialogueNodeLookup.TryGetValue(parentNode.Child, out var dialogueChild))
                return dialogueChild;
            if (replyNodeLookup.TryGetValue(parentNode.Child, out var replyChild))
                return replyChild;

            return null;
        }

        public DialogueNode GetDialogueNodeFromId(string id)
        {
            return dialogueNodeLookup.TryGetValue(id, out var node) ? node : null;
        }

        public DialogueEditorNode GetNodeFromId(string id)
        {
            if (dialogueNodeLookup.TryGetValue(id, out var dialogueNode))
                return dialogueNode;
            if (replyNodeLookup.TryGetValue(id, out var replyNode))
                return replyNode;

            return null;
        }

        public DialogueNode CreateDialogueNode(IHasChildren parent)
        {
            DialogueNode node = CreateInstance<DialogueNode>();
            node.name = Guid.NewGuid().ToString();

#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(node, "Created Dialogue Node");
#endif

            if (parent != null)
                parent.Child = node.name;

            dialogueNodes.Add(node);

            if (dialogueSpeakers.Count > 0)
                node.speaker = dialogueSpeakers[0];

#if UNITY_EDITOR
            if (Event.current != null)
                node.rect.position = Event.current.mousePosition;
#endif

            OnValidate();
            return node;
        }

        public ReplyNode CreateReplyNode(IHasChildren parent)
        {
            ReplyNode node = CreateInstance<ReplyNode>();
            node.name = Guid.NewGuid().ToString();

#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(node, "Created Reply Node");
#endif

            node.rect.height += 150;

            if (parent != null)
                parent.Child = node.name;

            replyNodes.Add(node);

#if UNITY_EDITOR
            if (Event.current != null)
                node.rect.position = Event.current.mousePosition;
#endif

            OnValidate();
            return node;
        }

        public void DeleteDialogueNode(DialogueNode nodeToDelete)
        {
            dialogueNodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);

#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(nodeToDelete);
#endif
        }

        public void DeleteReplyNode(ReplyNode nodeToDelete)
        {
            List<Reply> replies = new List<Reply>(nodeToDelete.replies);
            foreach (Reply reply in replies)
                DeleteReply(nodeToDelete, reply);

            replyNodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);

#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(nodeToDelete);
#endif
        }

        public Reply CreateReply(ReplyNode replyNode)
        {
            Reply reply = CreateInstance<Reply>();
            reply.name = Guid.NewGuid().ToString();

#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(reply, "Created Reply");
#endif

            replyNode.replies.Add(reply);
            OnValidate();
            return reply;
        }

        public void DeleteReply(ReplyNode replyNode, Reply reply)
        {
            RemoveChild(reply);
            replyNode.replies.Remove(reply);

#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(reply);
#endif
        }

        public void RemoveChild(IHasChildren parentNode)
        {
            parentNode.Child = "removed";
        }

        public void AddChild(IHasChildren parentNode, DialogueEditorNode node)
        {
            RemoveChild(parentNode);
            parentNode.Child = node.name;
        }

        private void CleanDanglingChildren(DialogueEditorNode nodeToDelete)
        {
            foreach (DialogueEditorNode editorNode in GetAllNodes())
            {
                if (editorNode is DialogueNode node && node.Child == nodeToDelete.name)
                    RemoveChild(node);
            }
        }

        public IEnumerable<string> GetSpeakers()
        {
            foreach (string speaker in dialogueSpeakers)
                yield return speaker;
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

#if UNITY_EDITOR
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode node in dialogueNodes)
                {
                    if (AssetDatabase.GetAssetPath(node) == "")
                        AssetDatabase.AddObjectToAsset(node, this);
                }

                foreach (ReplyNode node in replyNodes)
                {
                    if (AssetDatabase.GetAssetPath(node) == "")
                        AssetDatabase.AddObjectToAsset(node, this);
                }

                foreach (ReplyNode replyNode in replyNodes)
                {
                    foreach (Reply reply in replyNode.replies)
                    {
                        if (AssetDatabase.GetAssetPath(reply) == "")
                            AssetDatabase.AddObjectToAsset(reply, replyNode);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize() { }
    }
}
