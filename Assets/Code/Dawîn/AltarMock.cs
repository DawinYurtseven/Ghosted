using UnityEngine;

public class AltarMock : MonoBehaviour
{

    public GameObject uiHint;
    public void ChangeEmotion(Emotion emotion)
    {
        EmotionSingletonMock.Instance.ChangeEmotion(emotion);
    }

    public void turnOnHintAltar()
    {
        uiHint?.SetActive(true);
    }
    
    public void turnOffHintAltar()
    {
        uiHint?.SetActive(false);
    }
}
