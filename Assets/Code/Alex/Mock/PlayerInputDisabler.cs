using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputDisabler: MonoBehaviour
{
    private PlayerInput playerInput;

    [SerializeField] private string defaultActionMap = "Character Control";
    public static PlayerInputDisabler Instance { get; private set; }

    private string presavedActionMap = "";
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
        SwitchInputMap(defaultActionMap);
    }
    
    public string GetCurrentActionMap()
    {
        return presavedActionMap != "" ? presavedActionMap:playerInput.currentActionMap.name;
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
        EnableInput();
        StartCoroutine(EnableInputCoroutine(delaySeconds));
    }

    private IEnumerator EnableInputCoroutine(float delay)
    {
        //to switch even when game is paused
        yield return new WaitForSecondsRealtime(delay);
        EnableInput();
    }
    //change in next frame to avoid confusion


    public void SwitchInputMap(string mapName)
    {
        {
            foreach (InputActionMap actionMap in playerInput.actions.actionMaps)
            {
                actionMap.Disable();
            }
            playerInput.SwitchCurrentActionMap(mapName);
            playerInput.actions.FindActionMap(mapName)?.Enable();
        }
    }
    public void SwitchInputMapDelayed(string mapName, float delay = 0.5f)
    {
        Debug.Log("Delay Switch to " + mapName);
        StopCoroutine(SwitchInputMapDelayedCoroutine(presavedActionMap));
        presavedActionMap = mapName;
        foreach (InputActionMap actionMap in playerInput.actions.actionMaps)
        {
            actionMap.Disable();
        }
        StartCoroutine(SwitchInputMapDelayedCoroutine(mapName, delay));
    }
    private IEnumerator SwitchInputMapDelayedCoroutine(string mapName, float delay = 0.5f)
    {
        //to switch even when game is paused
        yield return new WaitForSecondsRealtime(delay);
        playerInput.SwitchCurrentActionMap(mapName);
        playerInput.actions.FindActionMap(mapName)?.Enable();
        presavedActionMap = "";
        //Debug.Log("State after coroutine: " + GetCurrentActionMap());
    }
    
    
}