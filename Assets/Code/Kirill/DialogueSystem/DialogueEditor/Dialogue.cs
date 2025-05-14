using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace Ghosted.Dialogue {
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
    public class Dialogue : ScriptableObject 
    {
        [SerializeField] List<DialogueEditorNode> dialogueEditorNodes = new List<DialogueEditorNode>();

        Dictionary<string, DialogueEditorNode> nodeLookup = new Dictionary<string, DialogueEditorNode>();

#if UNITY_EDITOR
        void Awake()
        {
            if (dialogueEditorNodes.Count == 0) {
                DialogueNode rootNode = new DialogueNode();
                rootNode.id = Guid.NewGuid().ToString();
                dialogueEditorNodes.Add(rootNode);
            }
            OnValidate();
        }
#endif

        private void OnValidate() {
            nodeLookup.Clear();
            foreach(DialogueEditorNode node in GetAllNodes()) {
                nodeLookup[node.id] = node;
            }
        }
        public IEnumerable<DialogueEditorNode> GetAllNodes () {
            return dialogueEditorNodes;
        }

        public DialogueEditorNode GetRootNode() {
            return dialogueEditorNodes[0];
        }

        public DialogueEditorNode GetSelectedNode(Vector2 point) {
            DialogueEditorNode foundNode = null;
            foreach (var node in dialogueEditorNodes) {
                if (node.rect.Contains(point))
                    foundNode = node;
            }
            return foundNode;
        }

        public DialogueEditorNode GetChild(DialogueNode parentNode)
        {
            if (nodeLookup.ContainsKey(parentNode.child))
                return nodeLookup[parentNode.child];
            else
                return null;
            /*if (parentNode.child != "") {
                foreach (string child in parentNode.child) {
                    if (nodeLookup.ContainsKey(child))
                        yield return nodeLookup[child];
                }
            }*/
        }

        public DialogueNode GetDialogueNode(string id) {
            if (nodeLookup.ContainsKey(id))
                return (DialogueNode) nodeLookup[id];
            else
                return null;
        }

        public DialogueEditorNode GetNodeFromId(string id) {
            if (nodeLookup.ContainsKey(id))
                return nodeLookup[id];
            else
                return null;
        }

        public void CreateDialogueNode(DialogueNode parent)
        {
            DialogueNode node = new DialogueNode();
            node.id = Guid.NewGuid().ToString();
            parent.child = node.id;
            dialogueEditorNodes.Add(node);
            node.isReplyNodeChild = false;
            OnValidate();
        }

        public void CreateReplyNode (DialogueNode parent) {
            ReplyNode node = new ReplyNode();
            node.rect.height += 150;
            node.id = Guid.NewGuid().ToString();
            parent.child = node.id;
            dialogueEditorNodes.Add(node);
            OnValidate();
        }

        public void DeleteDialogueNode(DialogueNode nodeToDelete)
        {
            dialogueEditorNodes.Remove(nodeToDelete);
            if (nodeToDelete.isReplyNodeChild == true) {
                ReplyNode replyNode = (ReplyNode) nodeLookup[nodeToDelete.replyNodeParentId];
                RemoveDialogueNodeFromReplyNode(replyNode, nodeToDelete);
            }
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
        }

        public void DeleteReplyNode(ReplyNode nodeToDelete) {
            List<string> replies = new List<string>(nodeToDelete.replies);
            foreach (string childNodeId in replies) {
                DeleteDialogueNode((DialogueNode) nodeLookup[childNodeId]);
            }
            dialogueEditorNodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
        }

        public void UpdateChildrenPosition(ReplyNode replyNode) {
            float curHeight = 0;
            for (int i = 0; i < replyNode.replies.Count; i++) {
                DialogueNode dialogueNode = (DialogueNode) nodeLookup[replyNode.replies[i]];
                dialogueNode.rect.position = new Vector2(replyNode.rect.position.x, replyNode.rect.position.y + curHeight);
                curHeight += dialogueNode.rect.height - 40;
            }
        }

        public void RemoveChild(DialogueNode parentNode) {
            parentNode.child = null;
        }

        public void AddReply(ReplyNode replyNode) {
            DialogueNode node = new DialogueNode();
            node.id = Guid.NewGuid().ToString();
            node.isReplyNodeChild = true;
            AddDialogueNodeToReplyNode(replyNode, node);
            dialogueEditorNodes.Add(node);
            OnValidate();
            UpdateChildrenPosition(replyNode);
        }

        /*public void DeleteReply(ReplyNode replyNode) {
            DialogueNode nodeToDelete = (DialogueNode) nodeLookup[replyNode.replies[0]];
            if (nodeToDelete == null)
                Debug.LogError("I can't detect the node, that has to be deleted");
            DeleteDialogueNode(nodeToDelete);
        }*/

        private void CleanDanglingChildren(DialogueEditorNode nodeToDelete)
        {
            foreach (DialogueEditorNode editorNode in GetAllNodes()) // Only DialogueNodes can bind children
            {
                DialogueNode node = editorNode as DialogueNode;
                if (node != null && node.child == nodeToDelete.id)
                    node.child = "null";
            }
        }

        private void AddDialogueNodeToReplyNode(ReplyNode replyNode, DialogueNode dialogueNode) {
            dialogueNode.replyNodeParentId = replyNode.id;
            AddRect(replyNode.rect, dialogueNode.rect);
            replyNode.replies.Add(dialogueNode.id);
        }

        private void RemoveDialogueNodeFromReplyNode(ReplyNode replyNode, DialogueNode dialogueNode) {
            SubRect(replyNode.rect, dialogueNode.rect);
            replyNode.replies.Remove(dialogueNode.id);
        }

        private void AddRect (Rect to, Rect from) {
            to.height += from.height;
        }
        private void SubRect (Rect from, Rect what) {
            from.height -= what.height;
        }
    }
}

