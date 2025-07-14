using UnityEngine;

public class TrainStartCutscene : MonoBehaviour
{
    public CutSceneTrigger cutSceneTrigger; 
    
    public void Start()
    {
        if (cutSceneTrigger != null)
        {
            StartCutscene();
        }
    }
    
    public void StartCutscene()
    {
        if (cutSceneTrigger != null)
        {
            cutSceneTrigger.manuallyTrigger(enabledByPlayer:true);
        }
        else
        {
            Debug.LogWarning("CutSceneTrigger is not assigned in TrainStartCutscene.");
        }
    }
}
