using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    [Tooltip("Event triggered when something enters the trigger zone.")]
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    public UnityEvent OnInteract;

    public virtual void OnTriggerExit(Collider other)
    {
        onTriggerExit?.Invoke();
    }

    // TODO: adjust for actual input system
    public virtual void OnTriggerStay(Collider other)
    {
        if(Input.GetKeyUp(KeyCode.E))
            OnInteract?.Invoke();
    }
    

    protected virtual void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke();
    }
}
