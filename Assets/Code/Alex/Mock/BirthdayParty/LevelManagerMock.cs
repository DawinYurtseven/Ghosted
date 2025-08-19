using Cinemachine;
using Ghosted.Dialogue;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private Fear barier;
    
    [Header("Enter Train third time")]
    [SerializeField] private Fear barier2;
    
    [Header("Transition to move mock")]
    [SerializeField] private FadeOut fadeOut;
    
    private int trainSceneCount = 0;
    

    [SerializeField] private ghostOrb ghost;
    public GameObject[] objectsToActivate;

   
    private ThisIsAProperDialogueSystem dialogue;
    [SerializeField] private ThisIsAProperDialogueSystem trainDialogue;
    [SerializeField] private FMODUnity.StudioEventEmitter _emitter;

    void Start()
    {
        dialogue = this.GetComponent<ThisIsAProperDialogueSystem>();
    }
    private void OnEnable()
    {
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
                TrainCutScene(false);
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
                //train.GetComponent<SplineAnimate>()?.Restart(false);
                fadeOut.Fade(true, () => {
                    SceneManager.LoadScene("MoveMockPart1");
                });
                break;
            case CutSceneName.ChangeTrain:
                TrainChangeScene();
                break;
            default: return;
        }
    }

    private bool altarUsed;
    public void DialogueAfterAltar()
    {
        if (altarUsed) return;
        altarUsed = true;
        player.GetComponent<CharacterControllerMockup>().SetDialogue(dialogue);
        dialogue.StartDialogue();
    }
    void CuckooClockCutScene()
    {
        foreach (GameObject obj in objectsToActivate )
        {
            obj.SetActive(true);
        }
    }

    bool TrainCutScene(bool calledFromTrain)
    {
        if (trainSceneCount == 0 || trainSceneCount == 1 && barier.lockedInFear || trainSceneCount == 2 && barier2.lockedInFear)
        {
            playerCamera.Priority = 0;
            trainCamera.Priority = 10;
            _emitter.Play();
            train.GetComponent<SplineAnimate>()?.Play();
            trainSceneCount++;
           // ghost.FollowObject(train.transform);
           return true;
        }

        if (!calledFromTrain && (trainSceneCount == 1 || trainSceneCount == 2))
        {
            trainDialogue.StartDialogue();
        }

        return false;
    }


    void TrainChangeScene()
    {
        
        if (roadPart == 0)
        {
            train.GetComponent<SplineAnimate>().Container = secondSpline;
            ghost.MoveToNextWaypoint();
            train.GetComponent<SplineAnimate>()?.Restart(false);
            if (!TrainCutScene(true))
            {
                //train.GetComponent<SplineAnimate>().Pause();
                
                playerCamera.Priority = 10;
                trainCamera.Priority = 0;
                player.transform.position = playerSpawn1.position;
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }

            roadPart++;
        }
        
        else if (roadPart == 1)
        {
            train.GetComponent<SplineAnimate>().Container = thirdSpline;
            ghost.MoveToNextWaypoint();
            train.GetComponent<SplineAnimate>()?.Restart(false);
            if (!TrainCutScene(true))
            {
                //train.GetComponent<SplineAnimate>().Pause();
                 playerCamera.Priority = 10;
                 trainCamera.Priority = 0;
                 player.transform.position = playerSpawn2.position;
                 player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                 UIHintShow.Instance.ShowHintUntilAction("Recall");
            }

            roadPart++;
            
        }
    }
} 
