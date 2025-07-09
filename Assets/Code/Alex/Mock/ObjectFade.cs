using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ObjectFade : MonoBehaviour
{
    public float fadeDuration = 1.5f;

    private Renderer _renderer;
    private Material[] _materials;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            // Use a new material instance to avoid affecting shared material
            _materials = _renderer.materials;
        }
    }

    public void doFading()
    {
        if (_materials == null) return;

        // Animate alpha from 1 to 0
        // _material.DOFade(0f, fadeDuration).OnComplete(() =>
        // {
        //     gameObject.SetActive(false); // Or Destroy(gameObject);
        // });

        int completed = 0;

        foreach (var mat in _materials)
        {
            DOTween.To(() => mat.GetFloat("_Alpha"), x => mat.SetFloat("_Alpha", x), 0f, fadeDuration)
                .OnComplete(() =>
                {
                    completed++;
                    if (completed >= _materials.Length)
                    {
                        gameObject.SetActive(false);
                    }
                });
        }
    }
}