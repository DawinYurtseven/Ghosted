using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelManager_Introduction : MonoBehaviour
{
    //Definitely need to rewrite that, because it is just shitty written, but I dont have time anymore
    
    public ghostOrb ghost;
    
    private AudioSource audioSource;
    [SerializeField] private GameObject player;

    [SerializeField] private FadeOut fade;

    // Update is called once per frame
    [Header("Cutscenes Canvas")]
    [SerializeField] private CutSceneCanvas[] cutScenes;
    
    private CutSceneCanvas currentCutScene = null;


    void Start()
    {
        currentCutScene = cutScenes[0];
        cutScenes[0].Initialize(() =>
        {
            UIHintShow.Instance.ShowHintUntilAction("Move");
        });
    }
    
    public void onAccepted(InputAction.CallbackContext context)
    {
        if (context.performed && currentCutScene != null)
        {
            currentCutScene.onAccepted();
        }
    }
    

    public void animateGhost()
    
    //TODO for @Dawin - laughing in FMOD
    {
        ghost.MoveToPosition(player.transform.position + new Vector3 (0,1,0.5f), false);
        ghost.Laugh();
    }
    public void finishLevel()
    {
        fade.Fade(true, () =>
        {
            SceneManager.LoadScene("HomeLevel");
        });
        
    }
}
