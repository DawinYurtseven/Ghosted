using System.Collections;
using System.Collections.Generic;
using Ghosted.Dialogue;
using UnityEngine;
using System.Linq;

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
            if (uiHint != null)
            {
                uiHint.SetActive(true);
            }

        }
        
        public void turnOffHint()
        {
            if (uiHint != null)
            {
                uiHint.SetActive(false);
            }
           
        }

        public void deleteLastDialogue()
        {
            var list = this.dialogues.ToList();
            list.Remove(curDialogue);
            this.dialogues = list.ToArray();
            
            this.ChooseDialogueByID(this.dialogueId);

            
            // Debug.Log("Now: " + this.dialogues);
            // Debug.Log("Current Dialogue");
        }
    }
}
