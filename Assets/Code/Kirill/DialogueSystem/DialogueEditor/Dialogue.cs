using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace Ghosted.Dialogue {
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
    public class Dialogue : ScriptableObject 
    {
        [SerializeField] List<DialogueNode> dialogueNodes = new List<DialogueNode>();

        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        void Awake()
        {
            if (dialogueNodes.Count == 0) {
                DialogueNode rootNode = new DialogueNode();
                rootNode.id = Guid.NewGuid().ToString();
                dialogueNodes.Add(rootNode);
            }
            OnValidate();
        }
#endif

        private void OnValidate() {
            nodeLookup.Clear();
            foreach(DialogueNode node in GetAllNodes()) {
                nodeLookup[node.id] = node;
                
            }
        }
        public IEnumerable<DialogueNode> GetAllNodes () {
            return dialogueNodes;
        }

        public DialogueNode GetRootNode() {
            return dialogueNodes[0];
        }

        public DialogueNode GetSelectedNode(Vector2 point) {
            DialogueNode foundNode = null;
            foreach (var node in dialogueNodes) {
                if (node.rect.Contains(point))
                    foundNode = node;
            }
            return foundNode;
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            if (parentNode.children != null) {
                foreach (string child in parentNode.children) {
                    if (nodeLookup.ContainsKey(child))
                        yield return nodeLookup[child];
                }
            }
        }

        public void CreateNode(DialogueNode parent)
        {
            DialogueNode node = new DialogueNode();
            node.id = Guid.NewGuid().ToString();
            parent.children.Add(node.id);
            dialogueNodes.Add(node);
            OnValidate();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            dialogueNodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.children.Remove(nodeToDelete.id);
            }
        }
    }
}

