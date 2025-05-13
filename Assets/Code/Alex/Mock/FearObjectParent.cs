using UnityEngine;

public class FearObjectParent : Lockable
{
    public GameObject openState, closedState, shadow;

   

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

        currentState = newState;
        shadow.SetActive(currentState == State.Fear);
    }

    public override void Lock() {
        _locked = currentState == State.Fear;

        if (specialEffect != null)
        {
            specialEffect.SetActive(true);
        }
    }

    public override void Unlock()
    {
        closedState.SetActive(currentState == State.Joy);
        openState.SetActive(currentState != State.Joy);
        _locked = false;
        if (specialEffect != null)
        {
            specialEffect.SetActive(false);
        }
    }

}
