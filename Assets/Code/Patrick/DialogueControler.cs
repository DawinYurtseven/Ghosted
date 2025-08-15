using Ghosted.Dialogue;
using UnityEngine;

public class DialogueControler : MonoBehaviour
{
    [Header("Going into Ghost Lock")] 
    public ThisIsAProperDialogueSystem ghostdialogue;
    public ghostOrb ghostOrb;
    public Transform ghostTarget;
    
    [Header("New Train Arriving")]
    public ThisIsAProperDialogueSystem ghostDialogueNewtrain;
    public Transform ghostTargetNewtrain;
    
    public void forceDialogueStart(bool fastMove = false)
    {
        if (!ghostdialogue) return;
        if(ghostTarget == null || ghostOrb == null) return;
        
        if(!fastMove)
            ghostOrb.move2TransformSlow(ghostTarget);
        else
            ghostOrb.MoveToTransform(ghostTarget);
        
        ghostdialogue.StartDialogue();
    }
    
    public void forceDialogueStartNewTrain(bool fastMove = false)
    {
        if (!ghostDialogueNewtrain) return;
        if(ghostTargetNewtrain == null || ghostOrb == null) return;
        
        if(!fastMove)
            ghostOrb.move2TransformSlow(ghostTargetNewtrain);
        else
            ghostOrb.MoveToTransform(ghostTargetNewtrain);
        
        ghostDialogueNewtrain.StartDialogue();
    }
}
