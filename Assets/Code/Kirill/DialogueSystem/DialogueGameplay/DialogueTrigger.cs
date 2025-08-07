using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    public string action;
    [SerializeField] private UnityEvent onTrigger;

    public void Trigger()
    {
        onTrigger.Invoke();
    }
}