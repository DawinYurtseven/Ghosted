using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class MaterialChanger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Material joyMaterial, fearMaterial;
    private Renderer _renderer;
    
    void Awake()
    {
        
        _renderer = gameObject.GetComponent<MeshRenderer>();
        if (!_renderer) return;
        _renderer.material = joyMaterial;
        if (!_renderer) return;
        EmotionSingletonMock.Instance.EmotionSubject
            .Subscribe(emotion =>
            {
                switch (emotion)
                {
                    case Emotion.Joy: 
                        _renderer.material = joyMaterial;
                        break;
                    case Emotion.Fear:
                        _renderer.material = fearMaterial;
                        break;
                    default:
                        break;
                }
            });
    }
}
