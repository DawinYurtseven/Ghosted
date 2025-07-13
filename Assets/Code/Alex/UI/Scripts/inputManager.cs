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
        Debug.Log("Input Manager changed controls");
        string scheme = playerInput.currentControlScheme;
        InputChanged?.Invoke(scheme);
    }
}
