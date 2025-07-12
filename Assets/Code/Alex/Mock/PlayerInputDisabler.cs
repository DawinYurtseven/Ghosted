using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputDisabler: MonoBehaviour
{
    /// <summary>
    /// This script is turned off for now, because the player input is disabled in many dialogues, but right now we change inputMap for that
    /// </summary>
    private PlayerInput playerInput;
    public static PlayerInputDisabler Instance { get; private set; }
    void Awake()
    {
        
        playerInput = GetComponent<PlayerInput>();
        // Set up singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void DisableInput()
    {
        Debug.Log("careful, you disabled all input from the player");
        playerInput.enabled = false;
    }

    public void EnableInput()
    {
        Debug.Log("careful, you enabled all input from the player");
       playerInput.enabled = true;
    }
    
    public void EnableInputWithDelay(float delaySeconds)
    {
        StartCoroutine(EnableInputCoroutine(delaySeconds));
    }

    private IEnumerator EnableInputCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        EnableInput();
    }
}