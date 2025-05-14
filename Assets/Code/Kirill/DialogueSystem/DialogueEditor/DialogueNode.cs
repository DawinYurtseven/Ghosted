using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ghosted.Dialogue {

    [System.Serializable]
    public class DialogueNode : DialogueEditorNode
    {
        public string text;
        public string child = "";
        public bool isReplyNodeChild = false;
        public string replyNodeParentId = "";
    }
}
