using UnityEngine;
using DG.Tweening;

public class SpawnAnim : MonoBehaviour
{
    [SerializeField] private GameObject obj;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float animDuration = 0.5f;
    private CanvasGroup cg;
    
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
        Debug.Log("Got: " + state);
        if (state)
        {
            /// Show + animate in
            cg.alpha = 0;

            t.DOScale(Vector3.one, animDuration).SetEase(Ease.OutBack);
            cg.DOFade(1, animDuration);
            
            Debug.Log("Showing obj");
        }
        else
        {
            // Animate out
            t.DOScale(Vector3.zero, animDuration).SetEase(Ease.InBack);
            cg.DOFade(0, animDuration).OnComplete(() =>
            {
                obj.SetActive(false);
            });

            Debug.Log("Deactivating obj");
        }
    }
    
    public static Sequence moveTo(Transform t, Transform target, float duration = 0.5f, Ease ease = Ease.OutQuad)
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

        // t.DOKill();
        // t.DOMove(target.position, duration).SetEase(ease);
        // t.DORotateQuaternion(target.rotation, duration).SetEase(ease);
        
        Sequence seq = DOTween.Sequence();
        seq.Append(t.DOMove(target.position, duration).SetEase(ease));
        seq.Join(t.DORotateQuaternion(target.rotation, duration).SetEase(ease));
        
        return seq;
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
    
    public static void simulatePhysics(Transform t, Vector3 force, float duration = 0.5f, bool once = false)
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
            rb.isKinematic = false; // Ensure it's not kinematic to apply forces
            return;
        }

        rb.DOKill();
        rb.AddForce(force, ForceMode.Impulse);
        
        // Optionally, you can add a delay before stopping the physics simulation
        DOVirtual.DelayedCall(duration, () => rb.velocity = Vector3.zero);
        
        if (once)
        {
            //remove the Rigidbody after the force is applied
            DOVirtual.DelayedCall(duration, () => Object.Destroy(rb));
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
}

