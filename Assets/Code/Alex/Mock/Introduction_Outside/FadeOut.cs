using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//TODO Redo
public class FadeOut : MonoBehaviour
{
    //Should be another object
    public Image fadeImage;
    public bool startOpaque =  true;
    public float fadeDuration_final = 1f;
    // Start is called before the first frame update
    void Start()
    {
        fadeImage.color = new Color(0, 0, 0, startOpaque ? 1 : 0);
        if (startOpaque) Fade();
        else
        {
            fadeImage.gameObject.SetActive(false);
        }
    }
    

    public void Fade (bool reversed = false, Action onCompleted = null)
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(0, 0, 0, reversed ? 0 : 1);
        fadeImage.DOFade(reversed ? 1: 0, fadeDuration_final).OnComplete(() => 
            { onCompleted?.Invoke(); });
    }

}
