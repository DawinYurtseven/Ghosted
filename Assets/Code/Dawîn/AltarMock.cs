using UnityEngine;

public class AltarMock : MonoBehaviour
{
    public void ChangeEmotion(Emotion emotion)
    {
        EmotionSingletonMock.Instance.ChangeEmotion(emotion);
    }
}
