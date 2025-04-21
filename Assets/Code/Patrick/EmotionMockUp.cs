using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    [Header("Character Controller Paramerter")]
    public CharacterControllerMockup controler;
    private Dictionary<String,emotionJumpParameters> emotionParameters = new Dictionary<String, emotionJumpParameters>();
    
    [Header("Emotion Change Mockup")]
    [SerializeField] public UnityEvent Joy;
    [SerializeField] public UnityEvent Fear;
    [SerializeField] public UnityEvent Lonely;

    [Header("Emotions")] 
    public List<GameObject> lonelyObjects;
    
    private void Start()
    {
        emotionJumpParameters defaultparams = new emotionJumpParameters(18,70,10);
        emotionJumpParameters joyParams = new emotionJumpParameters(21, 50, 10);
        emotionParameters.Add("Default", defaultparams);
        emotionParameters.Add("Joy", joyParams);
    }

    private void Awake()
    {
        Joy.AddListener(OnJoy);
        Lonely.AddListener(OnLonely);
        Fear.AddListener(OnFear);
    }

    private void OnDestroy()
    {
        Joy.RemoveListener(OnJoy);
        Lonely.RemoveListener(OnLonely);
        Fear.RemoveListener(OnFear);
    }

    private void Update()
    {
        //Input Change
        if (Input.GetKeyUp(KeyCode.C))
        {
            restoreDefaultConfig();
        }
        else if(Input.GetKeyUp(KeyCode.E))
        {
            OnJoy();
        }
        
        //Emotion Change
        if (Input.GetKeyDown(KeyCode.Alpha1)) Joy.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha2)) Fear.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha3)) Lonely.Invoke();
    }

    private void applyConfig(CharacterControllerMockup controller, emotionJumpParameters paramsToApply)
    {
        controller.gravity = paramsToApply.gravity;
        controller.jumpStrength = paramsToApply.jumpStrenght;
        controller.fallStrength = paramsToApply.fallStrength;
    }

    private void OnJoy()
    {
        enableColliders();
        Debug.Log("Loading Joy config");
        applyConfig(controler, emotionParameters["Joy"]);
    }

    private void OnLonely()
    {
        restoreDefaultConfig();
        //Debug.Log("Not implemented, need objects to deactivate collider from!");
        disableColliders();
    }

    private void OnFear()
    {
        enableColliders();
        restoreDefaultConfig();
    }

    private void restoreDefaultConfig()
    {
        Debug.Log("Loading Default Config");
        applyConfig(controler, emotionParameters["Default"]);
    }

    private void disableColliders()
    {
        foreach (GameObject o in lonelyObjects)
        {
            var component = o.GetComponent<Collider>();
            if (component != null) component.enabled = false;
        }
    }

    private void enableColliders()
    {
        foreach (GameObject o in lonelyObjects)
        {
            var component = o.GetComponent<Collider>();
            if (component != null) component.enabled = true;
        }
    }
}
