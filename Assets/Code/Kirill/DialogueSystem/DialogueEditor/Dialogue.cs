using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ghosted.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private List<string> dialogueSpeakers = new List<string>();
        [SerializeField] private List<DialogueNode> dialogueNodes = new List<DialogueNode>();
        [SerializeField] private List<ReplyNode> replyNodes = new List<ReplyNode>();

        private Dictionary<string, DialogueNode> dialogueNodeLookup = new Dictionary<string, DialogueNode>();
        private Dictionary<string, ReplyNode> replyNodeLookup = new Dictionary<string, ReplyNode>();
        
        
        public List<DialogueNode> GetAllNodes()
        {
            return dialogueNodes;
        }
        
    }
}
