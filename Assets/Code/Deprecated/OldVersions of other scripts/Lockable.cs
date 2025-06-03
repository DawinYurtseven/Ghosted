using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// DEPRECATED, USE Dawin -> Emotionabstract and talismantargetmock INSTEAD!!!!
/// </summary>
public abstract class Lockable : MonoBehaviour
{
    protected State currentState;
    protected bool _locked = false;
    // Called to lock the object.
    public abstract void Lock();

   
    // Called to unlock the object.
    // </summary>
    public abstract void Unlock();

    public bool GetLocked()
    {
        return _locked;
    }
}
