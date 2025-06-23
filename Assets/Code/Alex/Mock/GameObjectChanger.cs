using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UnityEngine.Serialization;

public class GameObjectChanger : MonoBehaviour
{
    [SerializeField] private GameObject joyObject;

    [SerializeField] private GameObject fearObject;

    // Start is called before the first frame update
    void Start()
    {
        EmotionSingletonMock.Instance.EmotionSubject
            .Subscribe(emotion =>
            {
                switch (emotion)
                {
                    case Emotion.Joy:
                        
                        if (joyObject)
                        {
                            joyObject.SetActive(true);
                        }

                        if (fearObject)
                        {
                            fearObject.SetActive(false);
                        }
                        
                        break;
                    case Emotion.Fear:
                        if (joyObject)
                        {
                            joyObject.SetActive(false);
                        }

                        if (fearObject)
                        {
                            fearObject.SetActive(true);
                        }
                        break;
                    default:
                        break;
                }
            });   
    }
}
