using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UniRx;
using UnityEngine;

public enum Emotion
{
    Joy,
    Fear,
    Lonely,
    Love,
}

public class EmotionSingletonMock : MonoBehaviour
{
    public static EmotionSingletonMock Instance { get; private set; }

    public bool disableAll = false;
    #region Emotions

    /*
     * this Area is to change the emotion through the whole area!
     */
    
    public Subject<Emotion> EmotionSubject = new Subject<Emotion>();
    [SerializeField] private Emotion currentEmotion = Emotion.Fear;


    [SerializeField] private GameObject joyGameObject, fearGameObject;

    public void ChangeEmotion(Emotion emotion)
    {
        if (currentEmotion == Emotion.Fear)
        {
            joyGameObject.SetActive(true);
            fearGameObject.SetActive(false);
            currentEmotion = Emotion.Joy;
        }

        else
        {
            joyGameObject.SetActive(false);
            fearGameObject.SetActive(true);
            currentEmotion = Emotion.Fear;
        }
        EmotionSubject.OnNext(currentEmotion);
    }
    
    

    #endregion
    

    #region LockOn

    /*
     * This will be a short version of the EmotionSingleton class.
     * here will be a reactive function that will help to create the lock on
     * for when an object is in the center or not.
     */

    public readonly Subject<TalismanTargetMock> CurrentTarget = new Subject<TalismanTargetMock>();
    [SerializeField] private TalismanTargetMock currentTarget;
    [SerializeField] private float range = 15.0f;
    [SerializeField] private List<TalismanTargetMock> availableTalismanTargetMocks;
    private Camera mainCamera;

    public void AddTarget(TalismanTargetMock target)
    {

        availableTalismanTargetMocks.Add(target);
    }
    
    private void CheckTargets()
    {
        if (disableAll)
        {
            foreach (var target in availableTalismanTargetMocks)
            {
                if (target != null)
                {
                    target.turnOff();
                }
            }
            CurrentTarget.OnNext(null);
            return;
        }

        if (availableTalismanTargetMocks.Count == 0)
        {
            CurrentTarget.OnNext(null);
            return;
        }
        
        TalismanTargetMock closestTarget = null;
        Vector3 closestTargetScreenPoint = Vector3.zero;
        float closestDistance = float.MaxValue;
  
        foreach (var target in availableTalismanTargetMocks)
        {
            if (target == null) continue;
            float distanceToCamera = Vector3.Distance(mainCamera.transform.position, target.transform.position);
            if (distanceToCamera > range)
            {
                target.turnOff();
                continue;
            }
            target.turnOn();
            var screenPoint = (mainCamera.WorldToScreenPoint(target.transform.position) -
                              new Vector3((Screen.width - 1) / 2, (Screen.height-1) / 2, 0));
            screenPoint.z = 0;
            float screenDistance = screenPoint.magnitude;
            if (screenDistance < closestDistance)
            {
                closestTarget = target;
                closestTargetScreenPoint = screenPoint;
                closestDistance = screenDistance;
            }
        }
        if (!closestTarget)
        {
            CurrentTarget.OnNext(null);
            return;
        }
        closestTarget.Highlight();
        var temp = currentTarget;
        currentTarget = closestTarget;
        if (temp != null && temp != currentTarget)
        {
            temp.UnHighlight();
        }
        CurrentTarget.OnNext(currentTarget);
    }

    public void RemoveTarget(TalismanTargetMock target)
    {
        availableTalismanTargetMocks.Remove(target);
        if (availableTalismanTargetMocks.Count == 0)
        {
            currentTarget = null;
        }
    }

    #endregion

    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        mainCamera = Camera.main;
        ChangeEmotion(Emotion.Joy);
    }

    // Update is called once per frame
    void Update()
    {
        CheckTargets();
    }
}