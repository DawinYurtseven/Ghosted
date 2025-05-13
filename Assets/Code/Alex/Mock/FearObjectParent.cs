using UnityEngine;

public class FearObjectParent : Lockable
{
    public GameObject openState, closedState, shadow;

    private State _currentState;

   

    public GameObject specialEffect;

    private void OnEnable()
    {
        StateManagerMock.OnStateChanged += ChangeState;
    }


    private void OnDisable()
    {
        StateManagerMock.OnStateChanged -= ChangeState;
    }


    protected void ChangeState(State newState)
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
        shadow.SetActive(_currentState == State.Fear && !_locked);
    }

    public override void Lock() {
            _locked = _currentState == State.Fear;
            if (specialEffect != null)
            {
                specialEffect.SetActive(true);
            }
    }

    public override void Unlock()
    {
        closedState.SetActive(_currentState == State.Joy);
        openState.SetActive(_currentState != State.Joy);
        _locked = false;
        if (specialEffect != null)
        {
            specialEffect.SetActive(false);
        }
    }

}
