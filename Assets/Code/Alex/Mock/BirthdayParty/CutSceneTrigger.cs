using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

public class CutSceneTrigger : MonoBehaviour
{
    public static event Action<CutSceneName> OnCutSceneTriggered;
    
    [SerializeField] CutSceneName cutSceneName;

    [SerializeField] private int repeatTimes;
    private int currentRepeat = 0;
    
    private void OnTriggerEnter(Collider other) {

        Debug.Log("Entered CutScene trigger");
        if (other.gameObject.GetComponent<CharacterControllerMockup>()!= null || other.gameObject.GetComponent<SplineAnimate>() != null) {
            OnCutSceneTriggered?.Invoke(cutSceneName);
            if (currentRepeat++ == repeatTimes)
            Destroy(gameObject);
        }
        
        
    }
    
}


public enum CutSceneName
{
    Train,
    TakeDocuments,
    EnterNextLevel
}