using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.Rendering.Universal;

public class MaterialChanger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Material joyMaterial, fearMaterial;
    private Renderer _renderer;

    void Start()
    {
        _renderer = gameObject.GetComponent<MeshRenderer>();
        if (!_renderer) return;

        _renderer.material = joyMaterial;

        DecalProjector decalProjector = GetComponent<DecalProjector>();

        EmotionSingletonMock.Instance.EmotionSubject
            .Subscribe(emotion =>
            {
                switch (emotion)
                {
                    case Emotion.Joy:
                        _renderer.material = joyMaterial;
                        if (decalProjector != null)
                        {
                            Debug.Log("IM NOT NULL");
                            decalProjector.material.SetColor("_Tint", joyMaterial.GetColor("_Tint"));
                        }
                        break;
                    case Emotion.Fear:
                        _renderer.material = fearMaterial;
                        if (decalProjector != null)
                        {
                            Debug.Log("IM NOT NULL");
                            decalProjector.material.SetColor("_Tint", fearMaterial.GetColor("_Tint"));
                        }
                        break;
                    default:
                        break;
                }
            });
    }
}