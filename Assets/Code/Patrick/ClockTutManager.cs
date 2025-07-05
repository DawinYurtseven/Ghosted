using UniRx;
using UnityEngine;
using DG.Tweening;

public class ClockTutManager : MonoBehaviour
{
    [Header("Clock Animation")]
    public ClockAnim clockAnim;
    public Material clockDeactivatedMaterial;
    private Material originalMaterial;

    [Header("Train Sequence")] 
    [SerializeField] private GameObject LockFear;
    [SerializeField] private GameObject LockJoy;
    [SerializeField] private Transform lockTarget;
    public AudioClip lockStartSound;
    public AudioClip lockSound;
    [SerializeField] private GameObject[] wallparts;
    
    // Future:
    //public Material highlightMaterial;
    
    void Awake()
    {
        if (clockAnim == null)
        {
            Debug.LogError("ClockAnim reference is not set in ClockTutManager.");
        }
        
        // Store the original material for later use
        if (clockAnim != null && clockAnim.hourHand != null)
        {
            originalMaterial = clockAnim.hourHand.GetComponent<Renderer>().material;
        }
    }
    
    void OnEnable()
    {
        if (clockAnim == null)
        {
            Debug.LogError("ClockAnim reference is not set in ClockTutManager.");
            return;
        }
        
        EmotionSingletonMock.Instance.EmotionSubject
            .Subscribe(setHandRotation);
    }

    private void OnDisable()
    {
        EmotionSingletonMock.Instance?.EmotionSubject
            .Subscribe(setHandRotation);
    }

    private void setHandRotation(Emotion emotion)
    {
        if (emotion == Emotion.Joy)
        {
            clockAnim.startHand(ClockHand.Hour);
            clockAnim.hourHand.GetComponent<Renderer>().material = originalMaterial;
        }
        else
        {
            clockAnim.setHand(ClockHand.Hour, 3);
            clockAnim.stopHand(ClockHand.Hour);
            clockAnim.hourHand.GetComponent<Renderer>().material = clockDeactivatedMaterial;
        }
    }

    private void animateLock(GameObject Lock)
    {
        if(Lock == null || lockTarget == null)
        {
            Debug.LogError("Lock or lockTarget is not set in ClockTutManager.");
            return;
        }
        
        Sequence seq = SpawnAnim.moveTo(Lock.transform, lockTarget, 0.5f, Ease.OutExpo);   // "break exponentially"
        seq.OnPlay(() =>
        {
            Lock.SetActive(true);
            //TODO:
            //AudioManager.Instance.PlaySound(lockStartSound);
        });
        seq.OnComplete(() =>
        {
            // add vibration
            Lock.transform.DOShakePosition(0.3f, 0.2f, 20, 90, false, true);
        });
    }
    
    public void StartTrainSequence()
    {
        // Animate the locks
        animateLock(LockFear);
        animateLock(LockJoy);
        
        //TODO: Play sound for the locks
        //lockSound.Instance.PlaySound(lockSound);
        
        // Set the clock hands to the initial state
        clockAnim.setTime(3, 0, 0); // Example time, adjust as needed
    }

    public void OnTrainCrash()
    {
        Debug.Log("Simulating train crash...");
        foreach (GameObject part in wallparts)
        {
            SpawnAnim.simulatePhysics(part.transform, Vector3.zero, 2f);
        }
    }
}
