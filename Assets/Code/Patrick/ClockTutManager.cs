using UniRx;
using UnityEngine;

public class ClockTutManager : MonoBehaviour
{
    public ClockAnim clockAnim;
    public Material clockDeactivatedMaterial;
    //public Material highlightMaterial;
    private Material originalMaterial;
    
    void Awake()
    {
        if (clockAnim == null)
        {
            Debug.LogError("ClockAnim reference is not set in ClockTutManager.");
        }
        
        // Store the original material for later use
        if (clockAnim != null && clockAnim.hourHand != null)
        {
            originalMaterial = clockAnim.hourHand.GetComponent<Renderer>().material;
        }
    }
    
    void OnEnable()
    {
        if (clockAnim == null)
        {
            Debug.LogError("ClockAnim reference is not set in ClockTutManager.");
            return;
        }
        
        EmotionSingletonMock.Instance.EmotionSubject
            .Subscribe(emotion =>
            {
                if (emotion == Emotion.Joy)
                {
                    clockAnim.startHand(ClockHand.Hour);
                    clockAnim.hourHand.GetComponent<Renderer>().material = originalMaterial;
                }
                else
                {
                    clockAnim.setHand(ClockHand.Hour, 3);
                    clockAnim.stopHand(ClockHand.Hour);
                    clockAnim.hourHand.GetComponent<Renderer>().material = clockDeactivatedMaterial;
                }
            });
    }
    
    
}
