using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class LevelManagerMock : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    [SerializeField] private CinemachineVirtualCamera trainCamera;
    [SerializeField] private GameObject player;
    
    [SerializeField] private GameObject train;
    
    [Header("Train stop")]
    [SerializeField] private SplineContainer secondSpline;

    [SerializeField] private Transform playerSpawn;
    [Header("Enter Train second time")]
    [SerializeField] private FearObjectParent barier;
    [SerializeField] private GameObject redLight, greenLight;
    
    private int trainSceneCount = 0;
    private void OnEnable()
    {
        CutSceneTrigger.OnCutSceneTriggered += ExecuteCutScene;
    }

    private void OnDisable()
    {
        CutSceneTrigger.OnCutSceneTriggered -= ExecuteCutScene;
    }
    
    private void ExecuteCutScene(CutSceneName cutScene)
    {
        switch (cutScene)
        {
            case CutSceneName.Train:
                TrainCutScene();
                break;
            case CutSceneName.TakeDocuments:
                break;
            case CutSceneName.EnterNextLevel:
                SceneManager.LoadScene("MovingMock");
                break;
            case CutSceneName.ChangeTrain:
                TrainChangeScene();
                break;
            default: return;
        }
    }

    void TrainCutScene()
    {
        if (trainSceneCount == 0 || barier.GetLocked())
        {
            playerCamera.Priority = 0;
            trainCamera.Priority = 10;
            train.GetComponent<SplineAnimate>()?.Play();
            
            trainSceneCount++;
        }
    }


    void TrainChangeScene()
    {
        train.GetComponent<SplineAnimate>().Container = secondSpline;
        //train.GetComponent<SplineAnimate>().Pause();
        train.GetComponent<SplineAnimate>()?.Restart(false);
        playerCamera.Priority = 10;
        trainCamera.Priority = 0;
        player.transform.position = playerSpawn.position;
    }
} 
