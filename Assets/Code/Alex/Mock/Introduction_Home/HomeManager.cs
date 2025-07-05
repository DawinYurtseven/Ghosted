using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class HomeManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.5f;
    void Start()
    {
        fadeImage.DOFade(0f, fadeDuration);
    }

    // Update is called once per frame
    public void Finish()
    {
        fadeImage.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            SceneManager.LoadScene("Birthday_ThirdPlayable");
        });
    }
}
