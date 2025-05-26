using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarriereScript : FearObjectParent
{
    public GameObject greenLight, redLight;
    public override  void Lock() {
        _locked = currentState == State.Fear;
        if (specialEffect != null)
        {
            specialEffect.SetActive(true);
        }
        greenLight.SetActive(_locked);
        redLight.SetActive(!_locked);
    }
    
    public override  void Unlock()
    {
        closedState.SetActive(currentState == State.Joy);
        openState.SetActive(currentState != State.Joy);
        _locked = false;
        if (specialEffect != null)
        {
            specialEffect.SetActive(false);
        }
        
        greenLight.SetActive(false);
        redLight.SetActive(true);
    }
}
