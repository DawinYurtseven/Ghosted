using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ghosted.Dialogue {
    [System.Serializable]
    public class ReplyNode : DialogueEditorNode
    {
        public List<Reply> replies = new List<Reply>();
    }
}

