using Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

public class TrainCutsceneController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private GameObject player;
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    
    [Header("Train")]
    [SerializeField] private GameObject train;
    [SerializeField] private CinemachineVirtualCamera trainCamera;
    // [SerializeField] private int roadPart = 0;
    
    [Header("Train stop")]
    [SerializeField] private SplineContainer spline;
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private CutSceneName _cutSceneName;
    
    private int trainSceneCount = 0;        //keep track (pun intended)
    private void OnEnable()
    {
        CutSceneTrigger.OnCutScenePlayerTriggered += TrainCutscene;
        CutSceneTrigger.OnCutSceneTrainTriggered += exitTrain;
    }

    private void OnDisable()
    {
        CutSceneTrigger.OnCutScenePlayerTriggered -= TrainCutscene;
        CutSceneTrigger.OnCutSceneTrainTriggered -= exitTrain;
    }

    private void TrainCutscene(CutSceneName name)
    {
        Debug.Log("Triggered " + name);
        
        if (name == CutSceneName.Train2Station)
        {
            playerCamera.Priority = 0;
            trainCamera.Priority = 10;
            train.GetComponent<SplineAnimate>()?.Play();
            trainSceneCount++;
            
            //Set player spawn
            player.transform.position = playerSpawn.position;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    private void exitTrain(CutSceneName name)
    {
        Debug.Log("Triggered " + name);
        
        if (name == CutSceneName.Exit2Station)
        {
            train.GetComponent<SplineAnimate>().Container = spline;
            train.GetComponent<SplineAnimate>().Pause();
            train.GetComponent<SplineAnimate>()?.Restart(false);
            playerCamera.Priority = 10;
            trainCamera.Priority = 0;
            player.transform.position = playerSpawn.position;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
