using System.Collections;
using Cinemachine;
using UnityEngine;
using DG.Tweening;

public class SpawnAnim : MonoBehaviour
{
    [Header("Spawn Animation Settings")]
    [Tooltip("The GameObject to animate. If not set, the script's GameObject will be used.")]
    [SerializeField] private GameObject obj;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float animDuration = 1f;
    private CanvasGroup cg;
    
    [Header("Spawn Many")]
    [SerializeField] private GameObject[] spawnObjs;
    private float delay = 9f;
    
    
    // Set all required Attributes
    private void Awake()
    {
        if (!obj)
            obj = this.gameObject;
        if (!spawnPoint)
        {
            if(obj)
            {
                spawnPoint = obj.transform;
            }
            else
                spawnPoint = this.gameObject.transform;
        }
         
        
        if (obj)
        {
            // Add CanvasGroup for fade if not already present
            cg = obj.GetComponent<CanvasGroup>();
            if (!cg) cg = obj.AddComponent<CanvasGroup>();
        }
    }
    
    // handles the animation
    public void Spawn(bool state)
    {
        if (!obj)
        {
            Debug.LogWarning("No gameobject to spawn!");
            return;
        }
        
        Transform t = obj.transform;
        obj.SetActive(true);
        //Debug.Log("Got: " + state);
        if (state)
        {
            // Show + animate in
            if (!cg)
            {
                Debug.Log("No CanvasGroup found, adding one");
                cg = obj.AddComponent<CanvasGroup>();
            }
            
            cg.alpha = 0;
            
            t.localScale = Vector3.zero; // Start from zero scale
            t.DOScale(Vector3.one, animDuration).SetEase(Ease.OutBack);
            cg.DOFade(1, animDuration);
            
            //Debug.Log("Showing obj " + obj.name);
        }
        else
        {
            animateOut(this.gameObject, animDuration);
        }
    }
    
    // Spawn many objects, if all objects have a SpawnAnim, they will use that spawn point
    public void SpawnMany(bool state)
    {
        if (spawnObjs == null || spawnObjs.Length <= 0)
        {
            Debug.LogWarning("No game objects to spawn!");
            return;
        }
        
        foreach (GameObject obj in spawnObjs)
        {
            SpawnAnim anim = obj.GetComponent<SpawnAnim>();
            if (anim == null)
            {
                anim = obj.AddComponent<SpawnAnim>();
                anim.spawnPoint = this.spawnPoint; // Use the same spawn point
            }
            anim.Spawn(state);
        }
    }

    public void SpawnManyAfter(bool state)
    {
        DOVirtual.DelayedCall(delay, () => SpawnMany(state));
    }
    
    //------------------- Static Stuff -------------------
    
    public static Sequence animateOut(GameObject obj, float animDuration = 0.6f)
    {
        if (obj == null)
        {
            Debug.LogWarning("No transform to animate out!");
            return null;
        }

        var cg = obj.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = obj.AddComponent<CanvasGroup>();

        Sequence seq = DOTween.Sequence();
        seq.Append(obj.transform.DOScale(Vector3.zero, animDuration).SetEase(Ease.InBack));
        seq.Join(cg.DOFade(0, animDuration)).OnComplete(() => obj.SetActive(false));

        Debug.Log("Deactivating obj");
        return seq;
    }
    
    public static void Despawn(GameObject obj, float delay = 0.5f)
    {
        if (obj == null)
        {
            Debug.LogWarning("No game object to despawn!");
            return;
        }

        Sequence s = animateOut(obj);
        obj.SetActive(false);
        s.SetDelay(delay);
        s.OnComplete(() => Destroy(obj)); // Optionally destroy the object after animation
        //Debug.Log("Deleted obj");
    }
    
    public static Sequence moveTo(Transform t, Transform target, float duration = 0.5f, Ease ease = Ease.OutQuad, bool withRotation = true)
    {
        if (t == null)
        {
            Debug.LogWarning("No transform to move!");
            return null;
        }
        if(target == null)
        {
            Debug.LogWarning("No target transform to move to!");
            return null;
        }
        
        Sequence seq = DOTween.Sequence();
        seq.Append(t.DOMove(target.position, duration).SetEase(ease));
        if(withRotation)
            seq.Join(t.DORotateQuaternion(target.rotation, duration).SetEase(ease));
        
        return seq;
    }
    
    public static Sequence Wiggle(Transform t, float duration, float strength=10f)
    {
        if(strength <= 0 || duration <= 0)
        {
            Debug.LogWarning("Invalid parameters for wiggle animation!");
            return null;
        }
        
        Sequence wiggle = DOTween.Sequence();

        wiggle.Join(t.DOShakeRotation(duration, strength, vibrato: 10, randomness: 90))
            .Join(t.DOShakeRotation(duration, strength, vibrato: 10, randomness: 90))
            .Join(t.DOShakeRotation(duration, strength, vibrato: 10, randomness: 90));

        return wiggle;
    }
    
    public static void rotateTo(Transform t, Transform target, Vector3 axis, float duration = 0.5f)
    {
        if (t == null)
        {
            Debug.LogWarning("No transform to rotate!");
            return;
        }

        t.DOKill();
    
        if (target == null)
        {
            // Endlosrotation um die angegebene Achse
            t.DORotate(axis.normalized * 360f, duration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
        }
        else
        {
            t.DORotateQuaternion(Quaternion.AngleAxis(axis.magnitude, axis.normalized) * target.rotation, duration)
                .SetEase(Ease.OutQuad);
        }
    }
    
    public static void simulatePhysics(Transform t, Vector3 force, float duration = 0.5f, bool once = true)
    {
        if (t == null)
        {
            Debug.LogWarning("No transform to simulate physics on!");
            return;
        }

        Rigidbody rb = t.GetComponent<Rigidbody>();
        if (rb == null)
        {
            // add a Rigidbody if it doesn't exist
            rb = t.gameObject.AddComponent<Rigidbody>();
            rb.useGravity = true; // Enable gravity
            rb.isKinematic = false; // Ensure it's not kinematic to apply forces
        }

        rb.DOKill();
        rb.AddForce(force, ForceMode.Impulse);
        
        // Optionally, you can add a delay before stopping the physics simulation
        DOVirtual.DelayedCall(duration, () => rb.velocity = Vector3.zero);
        
        if (once)
        {
            //remove the Rigidbody after the force is applied
            DOVirtual.DelayedCall(duration, () => Destroy(rb)); //Destroy(t.gameObject.GetComponent<Rigidbody>())
        }
    }
    
    public static void stopAnimation(Transform t)
    {
        if (t == null)
        {
            Debug.LogWarning("No transform to stop animation on!");
            return;
        }
        
        t.DOKill();
    }
    
    public static void changeLayerTo(Transform t, int layer)
    {
        if (t == null)
        {
            Debug.LogWarning("No transform to change layer!");
            return;
        }
        
        t.gameObject.layer = layer;
        
        // Recursively change the layer for all child objects
        foreach (Transform child in t)
        {
            changeLayerTo(child, layer);
        }
    }

    public static IEnumerator shakeVirtualCamera(CinemachineVirtualCamera cam = null, float duration = 1f,
        float strength = 1f, int vibrato = 10, float randomness = 90f)
    {
        if (cam == null)
        {
            Debug.Log("Cam two shake is null");
            yield break;
        }

        Debug.Log("Shaking that cam");

        var shaker = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        if (shaker == null)
            shaker = cam.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        shaker.m_AmplitudeGain = strength;
        shaker.m_FrequencyGain = vibrato;

        yield return new WaitForSeconds(duration);

        shaker.m_AmplitudeGain = 0f;
        shaker.m_FrequencyGain = 0f;
    }


}

