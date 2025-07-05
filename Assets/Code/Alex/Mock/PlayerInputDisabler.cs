using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputDisabler: MonoBehaviour
{
    /// <summary>
    /// This script is turned off for now, because the player input is disabled in many dialogues, but right now we change inputMap for that
    /// </summary>
    private PlayerInput playerInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public void DisableInput()
    {
        //playerInput.enabled = false;
    }

    public void EnableInput()
    {
       // playerInput.enabled = true;
    }
}