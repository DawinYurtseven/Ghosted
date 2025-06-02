using System.Collections;
using System.Collections.Generic;
using Ghosted.Dialogue;
using UnityEngine;

namespace Ghosted.Dialogue
{
    public class AreaConversant : Conversant
    {
        private bool isDialogueEnabled = true;

        //If player arrives to a certain area, the dialogue starts. The area has to contain a trigger
        void OnTriggerEnter(Collider other)
        {
            if (isDialogueEnabled)
            {
                var playerConversant = other.gameObject.GetComponent<PlayerConversant>();
                if (playerConversant != null)
                {
                    if (curDialogue == null)
                    {
                        Debug.LogError("Dialogue in area conversant is null! Please attach the dialogue");
                        return;
                    }
                    playerConversant.EnteredConversantArea(this);
                    playerConversant.StartDialogue(curDialogue);
                }
            }
        }

        // In case you want this dialogue to start once, you can directly after start of the dialogue disable it, so that this dialogue
        // doesnt appear multiple times.
        public void DisableDialogue()
        {
            Debug.Log("I am disabling dialogue");
            isDialogueEnabled = false;
        }

        //In case you will need to start the dialogue again
        public void EnableDialogue()
        {
            isDialogueEnabled = true;
        }
    }
}

