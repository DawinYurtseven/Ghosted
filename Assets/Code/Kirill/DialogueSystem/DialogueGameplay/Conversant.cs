using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ghosted.Dialogue
{
    public class Conversant : MonoBehaviour
    {
        [SerializeField] Dialogue[] dialogues;
        protected Dialogue curDialogue;
        protected int dialogueId = -1;
        void Awake()
        {
            if (dialogues != null && dialogues.Length > 0)
            {
                curDialogue = dialogues[0];
                dialogueId = 0;
            }
        }

        // Choosee the next Dialogue in the array. Make sure, that none of the fields is null
        public void ChooseNextDialogue()
        {
            if (dialogues == null)
            {
                Debug.LogError("Dialogue array wasnt created");
                return;
            }
            if (dialogues.Length > dialogueId + 1)
            {
                dialogueId++;
                curDialogue = dialogues[dialogueId];
            }
            if (curDialogue == null)
            {
                Debug.LogError("The Dialogue doesnt exist. Please make sure, that all dialogues are in the fields");
            }
        }

        // Choose current dialogue. Use if you want to "jump through" some dialogues in between.
        public void ChooseDialogue(Dialogue dialogue)
        {
            for (int i = 0; i < dialogues.Length; i++)
            {
                if (dialogues[i].name == dialogue.name)
                {
                    dialogueId = i;
                    curDialogue = dialogue;
                    return;
                }
            }
            Debug.LogError("I dont have this dialogue in my array of dialogues. Please add this dialogue in to the array");
        }
    }
}
