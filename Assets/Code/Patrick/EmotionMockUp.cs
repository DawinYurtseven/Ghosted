using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public struct emotionJumpParameters
{
    public float jumpStrenght;
    public float fallStrength;
    public float speed;
    public float acceleration;
    public float decceleration;

    public emotionJumpParameters(float jumpStrenght,float fallStrength, float newSpeed, float newAcc, float newDecc)
    {
        this.jumpStrenght = jumpStrenght;
        this.fallStrength = fallStrength;
        this.speed = 8.5f;
        this.acceleration = 20;
        this.decceleration = 1;
        
        if (newSpeed > 0f)
            this.speed = newSpeed;
        if (newAcc > 0f)
            this.acceleration = newAcc;
        if (newDecc > 0f)
            this.decceleration = newDecc;
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
    [SerializeField] public UnityEvent LonelyTP;
    [SerializeField] public UnityEvent Bind;
    [SerializeField] public UnityEvent Shoot;
    [SerializeField] public StateManagerMock stateMock;
    
    [Header("Room Materials")] 
    [SerializeField] private Material joyMat;
    [SerializeField] private Material lonelyMat;
    [SerializeField] private List<GameObject> roomElements;
    
    [Header("Emotions")] 
    public List<GameObject> lonelyObjects;
    public List<Transform> teleportPos;
    public Material transparentMat;
    public Material ogMat;
    [SerializeField] private bool isBound = false;
    [SerializeField] private GameObject boundObj;
    public GameObject dummyObj;
    public bool useTP = false;
    [SerializeField] private bool allowChange = true;
    [SerializeField] private bool allowBind = true;
    public GameObject shooterActiveMsg;
    public GameObject shooterInactiveMsg;
    private bool isShooting = false;
    [SerializeField] private List<GameObject> triggerToDeactivate;
    private void Start()
    {
        emotionJumpParameters defaultparams = new emotionJumpParameters(18,70, 8.5f, -1,-1);
        emotionJumpParameters joyParams = new emotionJumpParameters(21, 50, 8.5f, -1,-1);
        emotionJumpParameters fearParams = new emotionJumpParameters(18, 70, 15,100,0.5f);
        
        emotionParameters.Add("Default", defaultparams);
        emotionParameters.Add("Joy", joyParams);
        emotionParameters.Add("Fear", fearParams);
    }

    #region Subscribe

    private void Awake()
    {
        Joy.AddListener(OnJoy);
        Lonely.AddListener(OnLonely);
        Fear.AddListener(OnFear);
        Shoot.AddListener(toggleShoot);
    }

    private void OnDestroy()
    {
        Joy.RemoveListener(OnJoy);
        Lonely.RemoveListener(OnLonely);
        Fear.RemoveListener(OnFear);
        Shoot.RemoveListener(toggleShoot);
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
        if (allowChange)
        {
            if (Input.GetKeyDown(KeyCode.Keypad0)) Joy.Invoke();
            if (Input.GetKeyDown(KeyCode.Alpha2)) Lonely.Invoke();
            if (Input.GetKeyDown(KeyCode.Alpha3)) LonelyTP.Invoke();
            
            if (Input.GetKeyDown(KeyCode.Keypad1)) Fear.Invoke();
        }
        
        if(Input.GetKeyDown(KeyCode.Q)) Bind.Invoke();
        if (Input.GetKeyDown(KeyCode.F))
        {
            Shoot.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.B)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void applyConfig(CharacterControllerMockup controller, emotionJumpParameters paramsToApply)
    {
        controller.jumpStrength = paramsToApply.jumpStrenght;
        controller.fallStrength = paramsToApply.fallStrength;
        controller.maxSpeed = paramsToApply.speed;
        controller.acceleration = paramsToApply.acceleration;
        controller.deaccelerationTime = paramsToApply.decceleration;
    }

    public void OnJoy()
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
        
        if(useTP)
            teleport(teleportPos[0]);
        
        addMaterialToRoom(joyMat);
        
        Debug.Log("Loading Joy config");
        applyConfig(controler, emotionParameters["Joy"]);
        //Ducktape
        stateMock.changeState(State.Joy);
    }

    public void OnLonely()
    {
        Debug.Log("Changing to Lonely");
        _roomState = RoomState.Lonely;
        
        addMaterialToRoom(lonelyMat);
        
        restoreDefaultConfig();
        //Debug.Log("Not implemented, need objects to deactivate collider from!");
        disableColliders();
    }

    public void OnFear()
    {
        Debug.Log("Changing to Fear");
        _roomState = RoomState.Fear;
        
        // Ducktape
        addMaterialToRoom(lonelyMat);
        stateMock.changeState(State.Fear);
        
        enableColliders();
        applyConfig(controler, emotionParameters["Fear"]);
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

    private void addMaterialTo(GameObject o, Material m)
    {
        if (!o)
        {
            Debug.LogWarning("Oh Oh, obj to add material " + m + " to is null!");
            return;
        }
        
        Renderer renderer = o.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = m;
        }
    }

    private void addMaterialToRoom(Material m)
    {
        foreach (GameObject roomElement in roomElements)
        {
            addMaterialTo(roomElement, m);
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

    public void setAllowChange(bool newVal) { allowChange = newVal;}
    public void setAllowBind(bool newVal) { allowBind = newVal;}

    public void toggleAllowChange() { allowChange = !allowChange;}
    public void toggleAllowBind() { allowBind = !allowBind;}

    public void setUseTP(bool newVal)
    {
        useTP = newVal;
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
        if(!allowBind)
            return;
        
        boundObj = gameObject;
        isBound = !isBound;
        
        if (isBound)
        {
            Debug.Log("Obj bound");
            disableCollider(gameObject);
            disableCollider(dummyObj);
            addTransparency(gameObject);
            addTransparency(dummyObj);
        }
        else
        {
            Debug.Log("Obj unbound");
            
            enableCollider(gameObject);
            enableCollider(dummyObj);
            removeTransparency(gameObject);
            removeTransparency(dummyObj);
        }
    }

    private void toggleShoot()
    {
        Debug.Log("Toggle shoot mode to: " + (allowChange && allowBind));

        if(isShooting) setShooting(false);
        else setShooting(true);
        
        setEmotionTriggers(!isShooting);
        
        if (allowChange && allowBind)
        {
            shooterActiveMsg.SetActive(true);
            shooterInactiveMsg.SetActive(false);
        }
        else
        {
            shooterActiveMsg.SetActive(false);
            shooterInactiveMsg.SetActive(true);
        }
    }

    private void setShooting(bool newVal)
    {
        isShooting = newVal;
        setAllowBind(newVal);
        setAllowChange(newVal);
    }

    private void setEmotionTriggers(bool newVal)
    {
        foreach (GameObject o in triggerToDeactivate)
        {
            o.SetActive(newVal);
        }
    }
    
}
