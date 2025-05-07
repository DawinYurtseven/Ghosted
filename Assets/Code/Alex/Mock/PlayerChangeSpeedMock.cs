using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChangeSpeedMock : MonoBehaviour
{
    // Start is called before the first frame update
    
    
    
    void OnEnable()
    {
        StateManagerMock.OnStateChanged += ChangePlayerCharacteristics;
    }
    
    void OnDisable()
    {
        StateManagerMock.OnStateChanged -= ChangePlayerCharacteristics;
    }


    private CharacterControllerMockup _controller;

    private void Awake()
    {
        _controller = this.GetComponent<CharacterControllerMockup>();
    }

    // Update is called once per frame
    void ChangePlayerCharacteristics(State newState)
    {
        if (!_controller) return;
        if (newState == State.Joy)
        {
            _controller.jumpStrength = 21;
            _controller.fallStrength = 50;
        }

        else {
            _controller.jumpStrength = 18;
            _controller.fallStrength = 70;
        }
    }
}
