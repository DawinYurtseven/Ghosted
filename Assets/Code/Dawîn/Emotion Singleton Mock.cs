using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public enum Emotion
{
    Joy,
    Fear,
    Lonely,
    Love,
    None
}

public class EmotionSingletonMock : MonoBehaviour
{
    public static EmotionSingletonMock Instance { get;  set; }

    public GameObject talismanCounter;
    public bool disableAll = false;
    public FMOD.Studio.EventInstance music;
    
    #region Emotions

    /*
     * this Area is to change the emotion through the whole area!
     */
    
    public Subject<Emotion> EmotionSubject = new Subject<Emotion>();
    [SerializeField] private Emotion initialEmotion = Emotion.Joy;
     private Emotion currentEmotion = Emotion.None; //should be different from initial emotion
    public UnityEvent<Emotion> emotionChanged = new UnityEvent<Emotion>();      // Trigger an event each time the emotion is changed
    [SerializeField] private GameObject joyGameObject, fearGameObject;

    public void ChangeEmotion(Emotion emotion)
    {
        if (emotion == currentEmotion) return;

        if (currentEmotion == Emotion.None)
        {
            currentEmotion = (emotion == Emotion.Joy) ? Emotion.Fear : Emotion.Joy;
        }
        if (currentEmotion == Emotion.Fear)
        {
            changeToJoy();
        }

        else
        {
            changeToFear();
        }
        
        EmotionSubject.OnNext(currentEmotion);
        // Trigger event with the current emotion switched to
        emotionChanged?.Invoke(currentEmotion);
    }

    private void changeToJoy()
    {
        if (currentEmotion == Emotion.Joy) return;
        joyGameObject.SetActive(true);
        fearGameObject.SetActive(false);
        currentEmotion = Emotion.Joy;
        
        if (Application.isPlaying)
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("CurrentEmotion", 0f);
    }
    
    private void changeToFear()
    {
        if (currentEmotion == Emotion.Fear) return;
        joyGameObject.SetActive(false);
        fearGameObject.SetActive(true);
        currentEmotion = Emotion.Fear;
        
        if (Application.isPlaying)
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("CurrentEmotion", 1f);
    }
    
    public Emotion getCurrentEmotion() => currentEmotion;

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

    public void enableTargets()
    {
        disableAll = false;
        talismanCounter.SetActive(true);
    }
    
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
            DontDestroyOnLoad(this);
            Instance = this;
        }

        mainCamera = Camera.main;
        if (disableAll) talismanCounter.SetActive(false);
        
    }

    void Start()
    {
        ChangeEmotion(initialEmotion);
        music = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Main");
        music.start();
    }

    // Update is called once per frame
    void Update()
    {
        CheckTargets();
    }
}