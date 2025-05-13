using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Lockable : MonoBehaviour
{

    // Called to lock the object.
    public abstract void Lock();

   
    // Called to unlock the object.
    // </summary>
    public abstract void Unlock();
}
