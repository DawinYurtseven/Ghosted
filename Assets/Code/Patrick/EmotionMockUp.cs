using System;
using System.Collections.Generic;
using UnityEngine;

public struct emotionJumpParameters
{
    private float jumpStrenght;
    private float fallStrength;
    private float gravity;

    public emotionJumpParameters(float jumpStrenght,float fallStrength, float gravity)
    {
        this.jumpStrenght = jumpStrenght;
        this.fallStrength = fallStrength;
        this.gravity = gravity;
    }
}

public class EmotionMockUp : MonoBehaviour
{
    public GameObject playerObj;
    public CharacterControllerMockup controler;
    private Dictionary<String,emotionJumpParameters> emotionParameters = new Dictionary<String, emotionJumpParameters>();
    
    private void Start()
    {
        emotionJumpParameters defaultparams = new emotionJumpParameters(18,70,10);
        emotionJumpParameters joyParams = new emotionJumpParameters(25, 50, 10);
        emotionParameters.Add("Default", defaultparams);
        emotionParameters.Add("Joy", joyParams);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            applyConfig(controler, emotionParameters["Default"]);
        }
        else if(Input.GetKeyUp(KeyCode.E))
        {
            applyConfig(controler, emotionParameters["Joy"]);
        }
    }

    private void applyConfig(CharacterControllerMockup controller, emotionJumpParameters paramsToApply)
    {
           //TODO:
    }
    
}
