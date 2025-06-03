using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarriereScript : FearObjectParent
{
    /// <summary>
    /// DEPRECATED
    /// </summary>
    public override  void Lock() {
        _locked = currentState == State.Fear;
        if (specialEffect != null)
        {
            specialEffect.SetActive(true);
        }
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
    }
}
