using System.Collections;
using System.Collections.Generic;
using Ghosted.Dialogue;
using UnityEngine;

namespace Ghosted.Dialogue
{
    public class AIConversant : Conversant
    {

        public GameObject uiHint;

        // Press e to interact, TODO: Combine with overall interact system
        public void Interact(PlayerConversant playerConversant)
        {
            if (curDialogue != null)
                playerConversant.StartDialogue(curDialogue);
        }

        public void turnOnHint()
        {
            uiHint?.SetActive(true);
        }
        
        public void turnOffHint()
        {
            uiHint?.SetActive(false);
        }
    }
}
