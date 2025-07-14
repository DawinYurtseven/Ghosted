using Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

public class TrainCutsceneController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private GameObject player;
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    
    [Header("Train Start")]
    [SerializeField] private GameObject trainStart;
    [SerializeField] private CinemachineVirtualCamera trainCameraStart;
    
    [Header("Main Train")]
    [SerializeField] private GameObject train;
    [SerializeField] private CinemachineVirtualCamera trainCamera;
    // [SerializeField] private int roadPart = 0;
    
    [Header("Train stop")]
    [SerializeField] private SplineContainer spline;
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private CutSceneName _cutSceneName;
    
    [Header("Start Train Stop")]
    [SerializeField] private SplineContainer splineStart;
    [SerializeField] private Transform playerSpawnStart;
    [SerializeField] private CutSceneName _cutSceneNameStart;
    
    private int trainSceneCount = 0;        //keep track (pun intended)
    private int trainStartSceneCount = 0; //for the start train
    
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
            startTrain(spline, trainCamera);
            trainSceneCount++;
            
            //Set player spawn
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            player.transform.position = playerSpawn.position;
        }
        
        if(name == CutSceneName.Start2StartStation)
        {
            startTrain(splineStart, trainCameraStart);
            trainStartSceneCount++;
            
            //Set player spawn
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            player.transform.position = playerSpawnStart.position;
        }
    }

    private void exitTrain(CutSceneName name)
    {
        Debug.Log("Triggered " + name);
        
        if (name == CutSceneName.Exit2Station)
        {
            stopTrain(train.GetComponent<SplineAnimate>());
            //train.GetComponent<SplineAnimate>()?.Restart(false);
            Debug.Log("Stopped train at target station");
            spawnPlayer(playerSpawn.position, trainCamera);
        }
        if(name == CutSceneName.Exit2StartStation)
        {
            stopTrain(train.GetComponent<SplineAnimate>());
            Debug.Log("Stopped train at start station");
            spawnPlayer(playerSpawnStart.position, trainCameraStart);
        }
    }

    private void startTrain(SplineContainer trainAnim, CinemachineVirtualCamera trainCamera)
    {
        if(trainAnim == null)
        {
            Debug.LogWarning("Train animation is null, cannot start.");
            return;
        }
        
        playerCamera.Priority = 0;
        trainCamera.Priority = 10;
        trainAnim.GetComponent<SplineAnimate>()?.Play();
    }
    
    private void stopTrain(SplineAnimate trainAnim)
    {
        if(trainAnim == null)
        {
            Debug.LogWarning("Train animation is null, cannot stop.");
            return;
        }
        
        trainAnim.Container = spline;
        trainAnim.Pause();
    }

    private void spawnPlayer(Vector3 position, CinemachineVirtualCamera trainCamera = null)
    {
        playerCamera.Priority = 10;
        if (trainCamera != null) trainCamera.Priority = 0;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.transform.position = position;
    }
}
