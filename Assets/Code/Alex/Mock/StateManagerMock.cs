using System;
using UnityEngine;

public class StateManagerMock : MonoBehaviour
{

    /* States:
    1 - Joy
    2 - Fear
    3 - Loneliness
    */
    
    public GameObject[] stateObjects;

    public static event Action<State> OnStateChanged;

    public State currentState = State.Joy;
    
    

  

    private void Start()
    {
        if (stateObjects.Length < Enum.GetNames(typeof(State)).Length) {
            Debug.Log ("Not enough states in the scene!");
            this.enabled = false;
        }
        for (int i = 0; i < stateObjects.Length; i++) {
            if (i == (int)currentState) {
                stateObjects[i]?.SetActive(true);
            }
            else {
                stateObjects[i]?.SetActive(false);
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0)) {
            changeState(State.Joy);
        }

        if (Input.GetKeyDown(KeyCode.Keypad1)) {
            changeState(State.Fear);
        }
    }


    private void changeState (State newState ) {
        stateObjects[(int) currentState]?.SetActive(false);
        stateObjects[(int) newState]?.SetActive(true);

        currentState = newState;
        OnStateChanged?.Invoke(newState);
    }


    
}

public enum State {
        Joy,
        Fear
    }
