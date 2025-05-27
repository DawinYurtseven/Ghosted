using System.Collections;
using System.Collections.Generic;
using Ghosted.Dialogue;
using UnityEngine;

namespace Ghosted.Dialogue
{
    public class AIConversant : MonoBehaviour
    {
        [SerializeField] Dialogue dialogue;
        private Dialogue curDialogue;
        void Awake()
        {
            curDialogue = dialogue;
        }
        public void Interact(PlayerConversant playerConversant)
        {
            playerConversant.StartDialogue(curDialogue);
        }
    }
}
