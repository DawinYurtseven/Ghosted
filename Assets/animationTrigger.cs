using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationTrigger : MonoBehaviour
{
    [SerializeField] private CharacterControllerMockup mock;
    
    public void Talisman()
    {
        print("thrown");
        mock.ThrowTalismanAnim();
    }
}
