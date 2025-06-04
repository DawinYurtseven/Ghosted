using UnityEngine;

public class WeatherController : MonoBehaviour
{
    [Header("Default")] 
    [SerializeField] private GameObject dirLight;
    [SerializeField] private Skybox _skybox;
    
    [Header("Rain")]
    [SerializeField] private GameObject rainGameObject;
    [SerializeField] private AudioSource rainBGM;
    [SerializeField] private GameObject rainLight;
    [SerializeField] private Skybox rainSkybox;
    
    // Add post processing
    
    public void setRain(bool raining)
    {
        if (raining)
        {
            rainGameObject.SetActive(true);
            rainBGM.Play();
            rainBGM.loop = true;
            rainLight?.SetActive(true);
        }
        else
        {
            rainGameObject.SetActive(false);
            rainBGM.Stop();
            dirLight?.SetActive(true);
            rainLight?.SetActive(false);
        }
    }
    
}
