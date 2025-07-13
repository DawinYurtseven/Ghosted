using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class KirillCharacterInteractionInput : MonoBehaviour
{
    //should be Deprecated
    
    
    
    CharacterInteraction controls;
    
    void Awake()
    {
        controls = new CharacterInteraction();
    }
    
    void OnEnable()
    {
        controls.Enable();
    }
    
    public bool GetInteracted()
    {
        return controls.Dialogue.Interact.triggered;
    }
    
    public bool GetPressedNext()
    {
        return controls.Dialogue.DialogueNext.triggered;
    }
    
    public void SubscribePressedNext(Action<InputAction.CallbackContext> func)
    {
        Debug.Log("I subscribe");
        controls.Dialogue.DialogueNext.performed += func;
    }
    
    public void UnsubscribePressedNext(Action<InputAction.CallbackContext> func)
    {
        controls.Dialogue.DialogueNext.performed -= func;
    }
    
    public void SubscribeInteract(Action<InputAction.CallbackContext> func)
    {
        controls.Dialogue.Interact.performed += func;
    }
    
    public void UnsubscribeInteract(Action<InputAction.CallbackContext> func)
    {
        controls.Dialogue.Interact.performed -= func;
    }
}
