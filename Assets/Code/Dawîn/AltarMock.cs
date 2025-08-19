using UnityEngine;

public class AltarMock : MonoBehaviour
{

    public UIInteractionHint uiHint;
    [SerializeField] private AltarUI uiAltar;
    
    [SerializeField] private FMODUnity.StudioEventEmitter _emitter;
    
    
    public void ChangeEmotion(Emotion emotion)
    {
        EmotionSingletonMock.Instance.ChangeEmotion(emotion);
    }

    public void ChangeEmotionJoy()
    {
        if (EmotionSingletonMock.Instance.getCurrentEmotion() == Emotion.Fear)
        {
            _emitter.Play(); 
        }
        EmotionSingletonMock.Instance.ChangeEmotion(Emotion.Joy);
    }
    
    public void ChangeEmotionFear()
    {
        if (EmotionSingletonMock.Instance.getCurrentEmotion() == Emotion.Joy)
        {
            _emitter.Play(); 
        }
        EmotionSingletonMock.Instance.ChangeEmotion(Emotion.Fear);
    }

    void Awake()
    {
        uiAltar = GetComponent<AltarUI>();
        
    }

    public void InteractAltar()
    {
        Debug.Log("Interacted with altar");
        uiHint.Hide();
        
        uiAltar.ActivateUI();
    }

    public void turnOnHintAltar()
    {
        if (uiHint) uiHint.Show();
    }
    
    public void turnOffHintAltar()
    {
        if (uiHint) uiHint.Hide();
    }
    
    
    void OnTriggerEnter(Collider other)
    {
        CharacterControllerMockup player = other.GetComponent<CharacterControllerMockup>();
        if (player != null)
        {
            player.SetAltar(this);
        }
    }
    void OnTriggerExit(Collider other)
    {
        CharacterControllerMockup player = other.GetComponent<CharacterControllerMockup>();
        if (player != null)
        {
            player.LeaveAltar();
        }
    }
}
