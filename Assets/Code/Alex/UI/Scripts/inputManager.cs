using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class inputManager : MonoBehaviour
{
    public static inputManager Instance { get; private set; }

    public delegate void InputChangedHandler(string newControlScheme);
    public event InputChangedHandler InputChanged;
    public PlayerInput playerInput;
    
    public string currentScheme = "MK";

    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {

        if (Instance == this)
            Instance = null;
    }

    public void OnControlsChanged()
    {
        currentScheme = playerInput.currentControlScheme;
        InputChanged?.Invoke(currentScheme);
    }
}
