using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ghosted.Dialogue {
    [System.Serializable]
    public class DialogueEditorNode : IDialogueEditorInstance
    {

        public Rect rect = new Rect(0, 0, 200, 80);
        public List<string> onEnterActions = new List<string>();
        public List<string> onExitActions = new List<string>();
        public string Id { get => id; set { Debug.Log("I set new ID value  " + value); id = value; } }

        private string id;
    }
}
