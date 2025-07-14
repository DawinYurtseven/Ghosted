using UnityEngine;

public class TrainStartCutscene : MonoBehaviour
{
    public CutSceneTrigger cutSceneTrigger; 
    
    public void Awake()
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
            cutSceneTrigger.manuallyTrigger(enabledByTrain:true);
        }
        else
        {
            Debug.LogWarning("CutSceneTrigger is not assigned in TrainStartCutscene.");
        }
    }
}
