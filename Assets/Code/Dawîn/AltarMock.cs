using UnityEngine;

public class AltarMock : MonoBehaviour
{

    public UIFacePlayer uiHint;
    
    public void ChangeEmotion(Emotion emotion)
    {
        EmotionSingletonMock.Instance.ChangeEmotion(emotion);
    }

    public void turnOnHintAltar()
    {
        if (uiHint) uiHint.Show();
    }
    
    public void turnOffHintAltar()
    {
        if (uiHint) uiHint.Hide();
    }
}
