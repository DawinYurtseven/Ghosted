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

public enum RoomState
{
    Normal,Joy, Fear, Lonely
}

public class EmotionMockUp : MonoBehaviour
{
    private RoomState _roomState = RoomState.Normal;
    
    [Header("Character Controller Paramerter")]
    public GameObject playerObj;
    public CharacterControllerMockup controler;
    private Dictionary<String,emotionJumpParameters> emotionParameters = new Dictionary<String, emotionJumpParameters>();
    
    [Header("Emotion Change Mockup")]
    [SerializeField] public UnityEvent Joy;
    [SerializeField] public UnityEvent Fear;
    [SerializeField] public UnityEvent Lonely;
    [SerializeField] public UnityEvent Bind;

    [Header("Emotions")] 
    public List<GameObject> lonelyObjects;
    public List<Transform> teleportPos;
    public Material transparentMat;
    public Material ogMat;
    [SerializeField]private bool isBound = false;
    [SerializeField]private GameObject boundObj;
    private void Start()
    {
        emotionJumpParameters defaultparams = new emotionJumpParameters(18,70,10);
        emotionJumpParameters joyParams = new emotionJumpParameters(21, 50, 10);
        emotionParameters.Add("Default", defaultparams);
        emotionParameters.Add("Joy", joyParams);
    }

    #region Subscribe

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

    #endregion
    
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
        if(Input.GetKeyDown(KeyCode.Q)) Bind.Invoke();
    }

    private void applyConfig(CharacterControllerMockup controller, emotionJumpParameters paramsToApply)
    {
        controller.gravity = paramsToApply.gravity;
        controller.jumpStrength = paramsToApply.jumpStrenght;
        controller.fallStrength = paramsToApply.fallStrength;
    }

    private void OnJoy()
    {
        Debug.Log("Changing to Joy");
        _roomState = RoomState.Joy;
        
        enableColliders();
        
        if (isBound && boundObj)
        {
            disableCollider(boundObj);
            addTransparency(boundObj);
        }
        else if(boundObj)
        {
            enableCollider(boundObj);
            removeTransparency(boundObj);
        }
        
        Debug.Log("Loading Joy config");
        applyConfig(controler, emotionParameters["Joy"]);
    }

    private void OnLonely()
    {
        Debug.Log("Changing to Lonely");
        _roomState = RoomState.Lonely;
        
        restoreDefaultConfig();
        //Debug.Log("Not implemented, need objects to deactivate collider from!");
        disableColliders();
    }

    private void OnFear()
    {
        Debug.Log("Changing to Fear");
        _roomState = RoomState.Fear;
        
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
            disableCollider(o);
            addTransparency(o);
        }
    }

    public void disableCollider(GameObject o)
    {
        var component = o.GetComponent<Collider>();
        if (component != null) component.enabled = false;
    }

    public void enableCollider(GameObject o)
    {
        var component = o.GetComponent<Collider>();
        if (component != null) component.enabled = true;
    }
    
    private void enableColliders()
    {
        foreach (GameObject o in lonelyObjects)
        {
            enableCollider(o);
            removeTransparency(o);
        }
    }

    private void addTransparency(GameObject o)
    {
        Renderer renderer = o.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = transparentMat;
        }
    }

    private void removeTransparency(GameObject o)
    {
        Renderer renderer = o.GetComponent<Renderer>();
        if (renderer != null && ogMat != null)
        {
            renderer.material = ogMat;
        }
    }
    
    public void teleport(Transform newPos)
    {
        playerObj.transform.position = newPos.position;
    }

    public void changeRoomstate(RoomState newState)
    {
        _roomState = newState;
    }
    
    public void BindObj(GameObject obj)
    {
        switch (_roomState)
        {
            case RoomState.Lonely:
                Debug.Log("Binding obj in Lonely");
                if (lonelyObjects.Contains(obj))
                {
                    disableCollider(obj);
                    Debug.Log("Disabled " + obj);
                }
                    
                break;
            
            default:
                Debug.Log("Bound obj" + obj);
                break;
        }
    }

    public void toggleBind(GameObject gameObject)
    {
        boundObj = gameObject;
        isBound = !isBound;
        
        if (isBound)
        {
            Debug.Log("Obj bound");
            disableCollider(gameObject);
            addTransparency(gameObject);
        }
        else
        {
            Debug.Log("Obj unbound");
            enableCollider(gameObject);
            removeTransparency(gameObject);
        }
        
    }
}
