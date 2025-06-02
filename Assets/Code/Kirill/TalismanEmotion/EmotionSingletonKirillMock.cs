using System.Collections.Generic;
using UniRx;
using UnityEngine;


public class EmotionSingletonKirillMock : MonoBehaviour
{
public static EmotionSingletonKirillMock Instance { get; private set; }


    #region LockOn

    /*
     * This will be a short version of the EmotionSingleton class.
     * here will be a reactive function that will help to create the lock on
     * for when an object is in the center or not.
     */

    //---// EmotionProperties //---//
    private EmotionEnumKirillMock curEmotion = EmotionEnumKirillMock.JOY;

    private readonly Subject<TalismanTargetKirillMock> TalismanTargetToChangeEmotion = new Subject<TalismanTargetKirillMock>();

    //---// SceneProperties //---//

    public readonly Subject<TalismanTargetKirillMock> CurrentTarget = new Subject<TalismanTargetKirillMock>();
    [SerializeField] private TalismanTargetKirillMock currentTarget;
    
    [SerializeField] private List<TalismanTargetKirillMock> availableTalismanTargetMocks;

    private Camera mainCamera;

    public void AddTarget(TalismanTargetKirillMock target)
    {

        availableTalismanTargetMocks.Add(target);
    }

    private void CheckTargets()
    {
        if (availableTalismanTargetMocks.Count == 0)
        {
            CurrentTarget.OnNext(null);
            return;
        }

        TalismanTargetKirillMock closestTarget = availableTalismanTargetMocks[0];
        Vector3 closestTargetScreenPoint = Vector3.one;
        foreach (var target in availableTalismanTargetMocks)
        {
            if (target == null) continue;
            var screenPoint = (mainCamera.WorldToScreenPoint(target.transform.position) -
                              new Vector3((Screen.width - 1) / 2, (Screen.height-1) / 2, 0)).normalized;
            if (screenPoint.magnitude < closestTargetScreenPoint.magnitude)
            {
                closestTarget = target;
                closestTargetScreenPoint = screenPoint;
            }
        }

        print(closestTarget.gameObject.name);
        closestTarget.Highlight();
        var temp = currentTarget;
        currentTarget = closestTarget;
        if (temp != null && temp != currentTarget)
        {
            temp.UnHighlight();
        }
        CurrentTarget.OnNext(currentTarget);
    }

    public void RemoveTarget(TalismanTargetKirillMock target)
    {
        availableTalismanTargetMocks.Remove(target);
        if (availableTalismanTargetMocks.Count == 0)
        {
            currentTarget = null;
        }
    }

    #endregion

    #region Emotion

    public void ChangeEmotion(EmotionEnumKirillMock newEmotion) {
        if (newEmotion != curEmotion) {
            // Update eveny TalismanObject
            foreach (var target in availableTalismanTargetMocks) {
                TalismanTargetToChangeEmotion.OnNext(target);
            }
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
    }

    // Update is called once per frame
    void Update()
    {
        CheckTargets();
    }
}
