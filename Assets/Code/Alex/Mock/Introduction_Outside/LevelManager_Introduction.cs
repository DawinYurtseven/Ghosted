using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelManager_Introduction : MonoBehaviour
{
    [Header("Introduction Sequence")]
    public CanvasGroup mainCanvasGroup;
    public CanvasGroup[] groups;
    public float fadeDuration = 0.5f;
    public float displayDuration = 5f;
    
    [SerializeField] private GameObject player;
    PlayerInputDisabler playerInputDisabler;
    void Start()
    {
        PlaySequence();
        playerInputDisabler = player.GetComponent<PlayerInputDisabler>();
        if (playerInputDisabler)
        {
            playerInputDisabler.DisableInput();
        }
    }

    void PlaySequence()
    {
        Sequence sequence = DOTween.Sequence();

        foreach (CanvasGroup group in groups)
        {
            sequence.AppendCallback(() => group.gameObject.SetActive(true));
            sequence.Append(group.DOFade(1f, fadeDuration));
            sequence.AppendInterval(displayDuration);
            sequence.Append(group.DOFade(0f, fadeDuration));
            sequence.AppendCallback(() => group.gameObject.SetActive(false));
        }

        sequence.OnComplete(OnSequenceComplete);
    }

    void OnSequenceComplete()
    {
        Debug.Log("Text sequence finished.");
        if (playerInputDisabler)
        {
            playerInputDisabler.EnableInput();
        }

        mainCanvasGroup.DOFade(0f, fadeDuration).OnComplete(() => UIHintShow.Instance.ShowHint("Use WASD/Left Joystick to move", 8f));
    }

    // Update is called once per frame
    
    [Header("finishLevel")]
    public Image fadeImage; 
    public float fadeDuration_final = 1f;
    
    
    public void finishLevel()
    {
        // Set image fully transparent black
        fadeImage.color = new Color(0, 0, 0, 0);
        if (playerInputDisabler)
        {
            playerInputDisabler.DisableInput();
        }
        fadeImage.DOFade(1f, fadeDuration_final).OnComplete(() =>
        {
            SceneManager.LoadScene("HomeLevel");
        });
    }
}
