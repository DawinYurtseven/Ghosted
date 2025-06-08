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
    }

    private void OnDisable()
    {
        emotionSing.emotionChanged.RemoveListener(changeWeather);
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
    
}
