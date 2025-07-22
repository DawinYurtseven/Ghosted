using Ghosted.Dialogue;
using UnityEngine;

public class DialogueControler : MonoBehaviour
{
    [Header("Going into Ghost Lock")]
    public AIConversant ghostConversant;
    public PlayerConversant playerConversant;
    public ghostOrb ghostOrb;
    public Transform ghostTarget;
    
    [Header("New Train Arriving")]
    public AIConversant ghostConversantNewtrain;
    public Transform ghostTargetNewtrain;
    
    public void forceDialogueStart(bool fastMove = false)
    {
        if (ghostConversant == null || playerConversant == null ) return;
        if(ghostTarget == null || ghostOrb == null) return;
        
        if(!fastMove)
            ghostOrb.move2TransformSlow(ghostTarget);
        else
            ghostOrb.MoveToTransform(ghostTarget);
        
        ghostConversant.Interact(playerConversant);
    }
    
    public void forceDialogueStartNewTrain(bool fastMove = false)
    {
        if (ghostConversantNewtrain == null || playerConversant == null ) return;
        if(ghostTargetNewtrain == null || ghostOrb == null) return;
        
        if(!fastMove)
            ghostOrb.move2TransformSlow(ghostTargetNewtrain);
        else
            ghostOrb.MoveToTransform(ghostTargetNewtrain);
        
        ghostConversantNewtrain.Interact(playerConversant);
    }
}
