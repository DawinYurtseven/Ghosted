using UnityEngine;

public class AltarMock : MonoBehaviour
{

    public UIInteractionHint uiHint;
    [SerializeField] private AltarUI uiAltar;
    
    
    public void ChangeEmotion(Emotion emotion)
    {
        EmotionSingletonMock.Instance.ChangeEmotion(emotion);
    }

    public void ChangeEmotionJoy()
    {
        EmotionSingletonMock.Instance.ChangeEmotion(Emotion.Joy);
    }
    
    public void ChangeEmotionFear()
    {
        EmotionSingletonMock.Instance.ChangeEmotion(Emotion.Fear);
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX/EmotionFear");
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
