using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ghosted.Dialogue {
    public class DialogueNode : ScriptableObject
    {
        public string text;
        public List<string> onEnterActions = new List<string>();
        public List<string> onExitActions = new List<string>();
        public string Child { get => child; set => child = value; }

        [SerializeField, HideInInspector]
        private string child = "";
        public string speaker = "";
        public FMODUnity.EventReference voiceClip;
    }
}
