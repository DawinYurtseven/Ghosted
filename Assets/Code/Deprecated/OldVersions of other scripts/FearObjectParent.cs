using UnityEngine;
using System.Linq;
using UnityEngine;
using UniRx;
//just to activate the shadow for tunnels

public class FearObjectParent : MonoBehaviour
{
    public GameObject shadow;
    private System.IDisposable _sub;

    
    
    void Start()
    {
        if (EmotionSingletonMock.Instance != null)
        {
            _sub = EmotionSingletonMock.Instance.EmotionSubject
                .Subscribe(emotion => { ChangeMaterial(emotion); });
            
            ChangeMaterial(EmotionSingletonMock.Instance.getCurrentEmotion());
        }
        
    }


    public void ChangeMaterial(Emotion emotion)
    {
        if (emotion == Emotion.Fear)
        {
            shadow.SetActive(true);
        }
        else
        {
            shadow.SetActive(false);
        }
    }
    
    private void OnDisable()
    {
        _sub?.Dispose();
        _sub = null;
    }

}
