using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    [Tooltip("Event triggered when something enters the trigger zone.")]
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    public UnityEvent OnInteract;

    public void OnTriggerExit(Collider other)
    {
        onTriggerExit?.Invoke();
    }

    public void OnTriggerStay(Collider other)
    {
        if(Input.GetKeyUp(KeyCode.E))
            OnInteract?.Invoke();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke();
    }
}
