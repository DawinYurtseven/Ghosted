using UnityEngine;

public class WeatherController : MonoBehaviour
{
    [Header("Default")] 
    [SerializeField] private GameObject dirLight;
    [SerializeField] private Skybox _skybox;
    [SerializeField] private GameObject defaultPP;
    
    [Header("Rain")]
    [SerializeField] private GameObject rainGameObject;
    [SerializeField] private AudioSource rainBGM;
    [SerializeField] private GameObject rainLight;
    [SerializeField] private Skybox rainSkybox;
    [SerializeField] private GameObject rainPP; 
    private bool isRaining = false;
    private bool isInside = false;

    [Header("Emotions")] 
    [SerializeField] private Material joyBox;
    [SerializeField] private Material fearBox;
    
    // Adjust the rain particles whether player is inside or not
    public bool IsInside
    {
        get => isInside;
        set
        {
            isInside = value;
            OnInside(isInside);
        }
    }
    
    private EmotionSingletonMock emotionSing;

    #region Event Subscription

    private void Awake()
    {
        emotionSing = FindObjectOfType<EmotionSingletonMock>();
        if(!emotionSing)
            Debug.LogWarning("Found no emotion singleton! Cant change weather dynamically!");
        
        changeWeather(emotionSing.getCurrentEmotion());
    }

    private void OnEnable()
    {
        emotionSing.emotionChanged.AddListener(changeWeather);
        emotionSing.emotionChanged.AddListener(changeSkybox);
    }

    private void OnDisable()
    {
        emotionSing.emotionChanged.RemoveListener(changeWeather);
        emotionSing.emotionChanged.RemoveListener(changeSkybox);
    }
    
    #endregion Event Subscription
    
    public void setRain(bool raining)
    {
        isRaining = raining;
        
        if (raining)
        {
            rainGameObject.SetActive(true);
            rainBGM.Play();
            rainBGM.loop = true;
            rainLight?.SetActive(true);
            rainPP?.SetActive(true);
            
            dirLight?.SetActive(false);
        }
        else
        {
            rainGameObject.SetActive(false);
            rainBGM.Stop();
            dirLight?.SetActive(true);
            rainLight?.SetActive(false);
            rainPP.SetActive(false);
        }
    }

    private void OnInside(bool inside)
    {  
        //Rain particles are only active when the player is outside
        rainGameObject.SetActive(isRaining && !isInside);
    }
    
    public void changeWeather(Emotion emotion)
    {
        Debug.Log("Changing Weather for: " + emotion);
        
        switch (emotion)
        {
            case Emotion.Joy:
                setRain(false);
                break;
            case Emotion.Fear:
                setRain(true);
                break;
            
            default:
                setRain(false);
                break;
        }
    }
    
    public void changeSkybox(Emotion emotion)
    {
        Debug.Log("Changing Skybox for: " + emotion);
        
        switch (emotion)
        {
            case Emotion.Joy:
                if(joyBox)
                    _changeSkybox(joyBox);
                _changeSkybox(_skybox.material);
                break;
            case Emotion.Fear:
                if(fearBox)
                    _changeSkybox(fearBox);
                _changeSkybox(rainSkybox.material);
                break;
            
            default:
                _changeSkybox(_skybox.material);
                break;
        }
    }
    
    private void _changeSkybox(Material skybox)
    {
        if (skybox != null)
        {
            _skybox.material = skybox;
        }
        else
        {
            Debug.LogWarning("Skybox is null, cannot change skybox.");
        }
    }
}
