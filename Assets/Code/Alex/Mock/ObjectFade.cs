using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ObjectFade : MonoBehaviour
{
    public float fadeDuration = 1.5f;

    private Renderer _renderer;
    private Material _material;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            // Use a new material instance to avoid affecting shared material
            _material = _renderer.material;

        }
    }

    public void doFading()
    {
        // Animate alpha from 1 to 0
        _material.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            gameObject.SetActive(false); // Or Destroy(gameObject);
        });
    }
    
    
    
}
