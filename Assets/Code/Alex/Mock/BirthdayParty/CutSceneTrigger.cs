using System;
using UnityEngine;
using UnityEngine.Splines;

public class CutSceneTrigger : MonoBehaviour
{
    public static event Action<CutSceneName> OnCutScenePlayerTriggered;
    public static event Action<CutSceneName> OnCutSceneTrainTriggered;
    [SerializeField] CutSceneName cutSceneName;
    //-1 -> no limit
    [SerializeField] private int repeatTimes = -1;
    
    
    private int currentRepeat = 0;
    
    public bool enabledByPlayer = true;
    public bool enabledByTrain = true;

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Entered CutScene trigger of " + cutSceneName);
        
        if (enabledByPlayer && other.gameObject.GetComponent<CharacterControllerMockup>() != null)
        {
            Debug.Log("Detected Player");
            
            OnCutScenePlayerTriggered?.Invoke(cutSceneName);
            if (repeatTimes != -1 && ++currentRepeat >= repeatTimes)
                Destroy(gameObject);
        }
        
        else if (enabledByTrain && other.gameObject.GetComponent<SplineAnimate>() != null)
        {
            Debug.Log("Detected Train");
            
            OnCutSceneTrainTriggered?.Invoke(cutSceneName);
            if (repeatTimes != -1 && ++currentRepeat >= repeatTimes)
                Destroy(gameObject);
        }
    }
    
    public void manuallyTrigger(bool enabledByPlayer = false, bool enabledByTrain = false)
    {
        Debug.Log("Manually triggered CutScene: " + cutSceneName);
        if (enabledByPlayer)
        {
            Debug.Log("Manually triggered CutScene: " + cutSceneName + " by Player");
            OnCutScenePlayerTriggered?.Invoke(cutSceneName);
        }
        if (enabledByTrain)
        {
            Debug.Log("Manually triggered CutScene: " + cutSceneName + " by Train");
            OnCutSceneTrainTriggered?.Invoke(cutSceneName);
        }
        // If repeatTimes is set, check if we need to destroy the object
        if (repeatTimes != -1 && ++currentRepeat >= repeatTimes)
            Destroy(gameObject);
    }
}


public enum CutSceneName
{
    Train,
    TakeDocuments,
    EnterNextLevel,
    ChangeTrain,
    CuckooClock,
    
    //Patrick:
    Train2Station,
    Exit2Station,
    Start2StartStation,
    Exit2StartStation,
}