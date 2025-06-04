using System.Linq;
using UnityEngine;

namespace Ghosted.Dialogue
{
    public class AIConversant : Conversant
    {

        // Press e to interact, TODO: Combine with overall interact system
        public void Interact(PlayerConversant playerConversant)
        {
            if (curDialogue != null)
                playerConversant.StartDialogue(curDialogue);
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
