using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TriggerZone : MonoBehaviour
{
    [Tooltip("Event triggered when something enters the trigger zone.")]
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    public UnityEvent OnInteract;
    
    public InputActionReference Interact;

    protected virtual void OnDisable()
    {
        if (Interact != null)
            Interact.action.performed -= interact;
    }
    
    public virtual void OnTriggerExit(Collider other)
    {
        if (Interact != null)
            Interact.action.performed -= interact;
        onTriggerExit?.Invoke();
    }

    // TODO: adjust for actual input system
    public virtual void OnTriggerStay(Collider other)
    {
        if (Interact != null)
        {
            Interact.action.performed -= interact; // Doppelte Registrierung vermeiden
            Interact.action.performed += interact;
        }
        else
        {
            Debug.LogWarning("Interact action is not assigned.");
        }
    }
    
    private void interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnInteract?.Invoke();
        }
    }
    

    protected virtual void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke();
    }
}
