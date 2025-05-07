using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearObjectParent : MonoBehaviour
{
    public GameObject openState, closedState, shadow;

    private State _currentState;

    private bool _locked = false;

    private void OnEnable()
    {
        StateManagerMock.OnStateChanged += ChangeState;
    }


    private void OnDisable()
    {
        StateManagerMock.OnStateChanged -= ChangeState;
    }


    private void ChangeState(State newState)
    {   if (!_locked) {
            if (newState == State.Fear) {
                closedState.SetActive(false);
                openState.SetActive(true);   
            }
            else {
                closedState.SetActive(true);
                openState.SetActive(false);
            }
        }
        _currentState = newState;
        shadow.SetActive(_currentState == State.Fear);
    }

    public void Lock() {
        if (_locked) {
            closedState.SetActive(_currentState == State.Joy);
            openState.SetActive(_currentState != State.Joy);
            _locked = false;
        }

        else {
            _locked = _currentState == State.Fear;
        }
    }
}
