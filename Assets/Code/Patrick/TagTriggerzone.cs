using System;
using UnityEngine;

public class TagTriggerzone : TriggerZone
{
    public String tagName = "Ghost";

    protected override void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(tagName))
        {
            Debug.Log("Tag entered: " + tagName);
            onTriggerEnter?.Invoke();
        }
    }
    
    public virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagName))
        {
            onTriggerExit?.Invoke();
        }
    }

    public virtual void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(tagName))
        {
            Debug.Log("Deprecated!");
            // if (Input.GetKeyUp(KeyCode.E))
            // {
            //     OnInteract?.Invoke();
            // }
        }
    }
    
}
