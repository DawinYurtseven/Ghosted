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
    [SerializeField] private Vector3 trainCrashForce = new Vector3(1, 0.5f, 0);
    [SerializeField] private GameObject[] bridgeParts;
    private bool isTrainCrashed = false;
    
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
            originalMaterial = clockAnim.hourHand.GetChild(0).GetComponent<Renderer>().material;
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
        if (emotion == Emotion.Fear)
        {
            clockAnim.startHand(ClockHand.Hour);
            clockAnim.hourHand.GetChild(0).GetComponent<Renderer>().material = originalMaterial;
        }
        else
        {
            clockAnim.setHand(ClockHand.Hour, 3);
            clockAnim.stopHand(ClockHand.Hour);
            clockAnim.hourHand.GetChild(0).GetComponent<Renderer>().material = clockDeactivatedMaterial;
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
        OnTrainCrash();   
        // Set the clock hands to the initial state
        clockAnim.setTime(3, 0, 0); // Example time, adjust as needed
    }

    public void OnTrainCrash()
    {
        if(isTrainCrashed)
            return;
        
        //ChangeBridgePartsLayer(LayerMask.NameToLayer("NoCollision"));
        
        isTrainCrashed = true;
        Debug.Log("Simulating train crash...");
        foreach (GameObject part in wallparts)
        {
            SpawnAnim.simulatePhysics(part.transform, trainCrashForce, 3f);
        }
        
        //wait for duration and change the layer of all bridge parts
        //Invoke(nameof(ChangeBridgePartsLayer), 2f);
    }
    
    private void ChangeBridgePartsLayer(int layer = -1)
    {
        if (bridgeParts == null || bridgeParts.Length == 0)
        {
            Debug.LogWarning("No bridge parts set in ClockTutManager.");
            return;
        }
        
        if(layer< 0)
        {
            layer = LayerMask.NameToLayer("CameraCollision");
        }
        
        foreach (GameObject part in bridgeParts)
        {
            part.layer = layer;
        }
    }
    
    // draw for the first wall part the force vector in simulatePhysics for reference in the editor
    // private void OnDrawGizmos()
    // {
    //     if (wallparts != null && wallparts.Length > 0)
    //     {
    //         Gizmos.color = Color.red;
    //         Gizmos.DrawLine(wallparts[0].transform.position, wallparts[0].transform.position + trainCrashForce);
    //     }
    //     else
    //     {
    //         Debug.Log("No wall parts set in ClockTutManager.");
    //     }
    // }
}
