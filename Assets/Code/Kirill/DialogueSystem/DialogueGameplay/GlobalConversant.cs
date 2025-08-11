/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ghosted.Dialogue
{
    public class GlobalConversant : Conversant
    {
        // Inherits from Conversant, so look at functions there. 
        // Has to be attached to some script holder game object.
        public void StartGlobalDialogue(PlayerConversant playerConversant)
        {
            if (curDialogue == null)
            {
                Debug.LogError("No dialogue in Global Conversant");
                return;
            }
            playerConversant.SetGlobalConversant(this);
            playerConversant.StartDialogue(curDialogue);
        }
    }
}
*/
