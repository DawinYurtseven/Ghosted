using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BufferLevelScript : MonoBehaviour
{
    public int exp;

    public int Level {
        get {
            return exp / 750;
        }
    }

    public void DoSomething() {
        Debug.Log("I did something");
    }
}
