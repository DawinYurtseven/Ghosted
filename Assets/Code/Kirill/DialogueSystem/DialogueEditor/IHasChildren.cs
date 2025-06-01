using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ghosted.Dialogue
{
    public interface IHasChildren : IDialogueEditorInstance
    {
        public string Child { get; set; }
    }
}
