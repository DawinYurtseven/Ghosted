using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ghosted.Dialogue {
    public class DialogueEditorNode : ScriptableObject
    {

        public Rect rect = new Rect(0, 0, 200, 80);
        public List<string> onEnterActions = new List<string>();
        public List<string> onExitActions = new List<string>();
    }
}
