using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputDisabler: MonoBehaviour
{
    private PlayerInput playerInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public void DisableInput()
    {
        playerInput.enabled = false;
    }

    public void EnableInput()
    {
        playerInput.enabled = true;
    }
}