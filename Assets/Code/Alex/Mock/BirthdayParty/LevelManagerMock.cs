using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

public class LevelManagerMock : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    [SerializeField] private CinemachineVirtualCamera trainCamera;
    [SerializeField] private GameObject player;
    
    [SerializeField] private GameObject train;

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
                TrainCutScne();
                break;
            case CutSceneName.TakeDocuments:
                break;
            case CutSceneName.EnterNextLevel:
                break;
            default: return;
        }
    }

    void TrainCutScne()
    {
        playerCamera.Priority = 0;
        trainCamera.Priority = 20;
        train.GetComponent<SplineAnimate>()?.Play();
    }
} 
