using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ghosted.Dialogue {
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] Dialogue currentDialogue;
        DialogueNode currentNode = null;

        void Awake()
        {
            currentNode = currentDialogue.GetRootNode();
        }
        public string GetText() {
            return currentNode.text;
        }

        public IEnumerable<string> GetChoices() {
            yield return "Hello!";
            yield return "Get prepared to be destroyed!";
        }

        public void Next() {
            currentNode = currentDialogue.GetAllChildren(currentNode).First();
        }

        public bool HasNext() {
            return currentDialogue.GetAllChildren(currentNode).Any();
        }
    }
}

