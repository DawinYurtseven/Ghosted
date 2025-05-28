using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class LevelManagerMock : MonoBehaviour
{
    
    [Header("Player")]
    [SerializeField] private GameObject player;
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    [Header("Train")]
    [SerializeField] private GameObject train;
    [SerializeField] private CinemachineVirtualCamera trainCamera;
    [SerializeField] private int roadPart = 0;
    [Header("Train stop 1")]
    [SerializeField] private SplineContainer secondSpline;
    [SerializeField] private Transform playerSpawn1;
    [Header("Train stop 2")]
    [SerializeField] private SplineContainer thirdSpline;
    [SerializeField] private Transform playerSpawn2;
    [Header("Enter Train second time")]
    [SerializeField] private FearObjectParent barier;
    
    [Header("Enter Train third time")]
    [SerializeField] private FearObjectParent barier2;
    
    private int trainSceneCount = 0;
    
    [Header("CuckooClock")]
    [SerializeField] private Animator clockAnimator;

    public GameObject[] objectsToActivate;
    private void OnEnable()
    {
        EmotionSingletonMock.Instance.disableAll = true;
        CutSceneTrigger.OnCutScenePlayerTriggered += ExecuteCutScenePlayer;
        CutSceneTrigger.OnCutSceneTrainTriggered += ExecuteCutSceneTrain;
    }

    private void OnDisable()
    {
        CutSceneTrigger.OnCutScenePlayerTriggered -= ExecuteCutScenePlayer;
        CutSceneTrigger.OnCutSceneTrainTriggered -= ExecuteCutSceneTrain;
    }
    
    private void ExecuteCutScenePlayer(CutSceneName cutScene)
    {
        switch (cutScene)
        {
            case CutSceneName.Train:
                TrainCutScene();
                break;
            case CutSceneName.CuckooClock:
                CuckooClockCutScene();
                break;
            default: return;
        }
    }
    
    private void ExecuteCutSceneTrain(CutSceneName cutScene)
    {
        switch (cutScene)
        {
            case CutSceneName.EnterNextLevel:
                SceneManager.LoadScene("MovingMock");
                break;
            case CutSceneName.ChangeTrain:
                TrainChangeScene();
                break;
            default: return;
        }
    }


    void CuckooClockCutScene()
    {
        clockAnimator.SetTrigger("cuckoo");
        foreach (GameObject obj in objectsToActivate )
        {
            obj.SetActive(true);
        }
        
        EmotionSingletonMock.Instance.disableAll = false;
    }

    void TrainCutScene()
    {
        if (trainSceneCount == 0 || trainSceneCount == 1 && barier.GetLocked() || trainSceneCount == 2 && barier2.GetLocked())
        {
            playerCamera.Priority = 0;
            trainCamera.Priority = 10;
            train.GetComponent<SplineAnimate>()?.Play();
            trainSceneCount++;
        }
    }


    void TrainChangeScene()
    {
        if (roadPart == 0)
        {
            train.GetComponent<SplineAnimate>().Container = secondSpline;
            //train.GetComponent<SplineAnimate>().Pause();
            train.GetComponent<SplineAnimate>()?.Restart(false);
            playerCamera.Priority = 10;
            trainCamera.Priority = 0;
            player.transform.position = playerSpawn1.position;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            roadPart++;
        }
        
        else if (roadPart == 1)
        {
            train.GetComponent<SplineAnimate>().Container = thirdSpline;
            //train.GetComponent<SplineAnimate>().Pause();
            train.GetComponent<SplineAnimate>()?.Restart(false);
            playerCamera.Priority = 10;
            trainCamera.Priority = 0;
            player.transform.position = playerSpawn2.position;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
} 
