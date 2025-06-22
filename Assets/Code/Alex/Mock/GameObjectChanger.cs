using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
public class GameObjectChanger : MonoBehaviour
{
    [SerializeField] private GameObject joyOverlay, fearOverlay;
    // Start is called before the first frame update
    void Start()
    {
        EmotionSingletonMock.Instance.EmotionSubject
            .Subscribe(emotion =>
            {
                switch (emotion)
                {
                    case Emotion.Joy: 
                        joyOverlay.SetActive(true);
                        fearOverlay.SetActive(false);
                        break;
                    case Emotion.Fear:
                        joyOverlay.SetActive(false);
                        fearOverlay.SetActive(true);
                        break;
                    default:
                        break;
                }
            });   
    }
}
