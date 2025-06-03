using UnityEngine;



/// <summary>
/// DEPRECATED, USE Dawin -> Fear INSTEAD!!!!
/// </summary>

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
                shadow.SetActive(true);
            }
            else {
                closedState.SetActive(true);
                openState.SetActive(false);
                shadow.SetActive(false);
            }
        }

        if (_locked && openState.activeSelf)
        {
            if (newState == State.Joy)
            {
                shadow.SetActive(false);
            }
            else
            {
                shadow.SetActive(true);
            }
            
        }

        if (_locked && closedState.activeSelf)
        {
            shadow.SetActive(false);
        }
        
        currentState = newState;
        
    }
    
    public override void Lock() {
        _locked = true;
        if (specialEffect != null)
        {
            specialEffect.SetActive(true);
        }
    }

    public override void Unlock()
    {
        closedState.SetActive(currentState == State.Joy);
        openState.SetActive(currentState != State.Joy);
        shadow.SetActive(currentState == State.Fear);
        _locked = false;
        if (specialEffect != null)
        {
            specialEffect.SetActive(false);
        }
    }

}
