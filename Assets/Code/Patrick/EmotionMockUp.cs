using System;
using System.Collections.Generic;
using UnityEngine;

public struct emotionJumpParameters
{
    public float jumpStrenght;
    public float fallStrength;
    public float gravity;

    public emotionJumpParameters(float jumpStrenght,float fallStrength, float gravity)
    {
        this.jumpStrenght = jumpStrenght;
        this.fallStrength = fallStrength;
        this.gravity = gravity;
    }
}

public class EmotionMockUp : MonoBehaviour
{
    public CharacterControllerMockup controler;
    private Dictionary<String,emotionJumpParameters> emotionParameters = new Dictionary<String, emotionJumpParameters>();
    
    private void Start()
    {
        emotionJumpParameters defaultparams = new emotionJumpParameters(18,70,10);
        emotionJumpParameters joyParams = new emotionJumpParameters(21, 50, 10);
        emotionParameters.Add("Default", defaultparams);
        emotionParameters.Add("Joy", joyParams);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            Debug.Log("Loading Default Config");
            applyConfig(controler, emotionParameters["Default"]);
        }
        else if(Input.GetKeyUp(KeyCode.E))
        {
            Debug.Log("Loading Joy config");
            applyConfig(controler, emotionParameters["Joy"]);
        }
    }

    private void applyConfig(CharacterControllerMockup controller, emotionJumpParameters paramsToApply)
    {
        controller.gravity = paramsToApply.gravity;
        controller.jumpStrength = paramsToApply.jumpStrenght;
        controller.fallStrength = paramsToApply.fallStrength;
    }
    
}
