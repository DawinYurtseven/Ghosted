using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    [Tooltip("Event triggered when something enters the trigger zone.")]
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit; 

    public void OnTriggerExit(Collider other)
    {
        onTriggerExit?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke();
    }
}
