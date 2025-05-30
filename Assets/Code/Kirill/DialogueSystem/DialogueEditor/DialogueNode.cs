using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ghosted.Dialogue {
    public class DialogueNode : DialogueEditorNode, IHasChildren
    {
        public string text;
        public string Child { get => child; set => child = value; }

        [SerializeField, HideInInspector]
        private string child = "";
        public string speaker = "";
    }
}
