using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ghosted.Dialogue {

    [System.Serializable]
    public class DialogueNode : DialogueEditorNode, IHasChildren
    {
        public string text;
        public string Child { get => child; set => child = value; }

        private string child = "";
        public string speaker = "";
    }
}
