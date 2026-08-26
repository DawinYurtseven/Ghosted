using Cinemachine;
using Ghosted.Dialogue;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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
    
    [Header("Train stop 3")]
    [SerializeField] private SplineContainer fourthSpline;
    [SerializeField] private Transform playerSpawn3;
    [Header("Enter Train second time")]
    [SerializeField] private Fear barier;
    
    [Header("Enter Train third time")]
    [SerializeField] private Fear barier2;
    [SerializeField] private GameObject wall;
    
    [Header("Enter Train fourth time")]
    [SerializeField] private Fear barier3;
    
    [Header("Transition to move mock")]
    [SerializeField] private FadeOut fadeOut;
    
    private int trainSceneCount = 0;
    
    private int layerPlayer;
    

    [SerializeField] private ghostOrb ghost;
    public GameObject[] objectsToActivate;

   
    private ThisIsAProperDialogueSystem dialogue;
    [SerializeField] private ThisIsAProperDialogueSystem trainDialogue;
    [SerializeField] private FMODUnity.StudioEventEmitter _emitter;

    void Start()
    {
        dialogue = this.GetComponent<ThisIsAProperDialogueSystem>();
        layerPlayer = LayerMask.NameToLayer("Player");
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
                    //SceneManager.LoadScene("Thank_You");
                    SceneManager.LoadScene("Thank_You");
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
        if (trainSceneCount == 0 || trainSceneCount == 1 && barier.lockedInFear || trainSceneCount == 2 && barier2.lockedInFear  || trainSceneCount == 3 && barier3.lockedInFear)
        {
            fadeOut.Fade(0.7f, true, () =>
            {
                if (Camera.main != null) Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));
                trainCamera.Priority = 10;
                playerCamera.Priority = 0;
                if (!_emitter.IsPlaying())
                {
                    _emitter.Play();
                }
                train.GetComponent<SplineAnimate>()?.Play();
                if (trainSceneCount == 2)
                {
                    wall.SetActive(false);
                }
                trainSceneCount++;
                fadeOut.Fade(0.7f, 0.2f);
            });
            return true;
        }

        return false;
    }


    void TrainChangeScene()
    {
        switch (roadPart)
        {
            case 0: splineChange(playerSpawn1, secondSpline); break;
            case 1: splineChange(playerSpawn2, thirdSpline); 
                UIHintShow.Instance.ShowHintUntilAction("Recall");
                break;
            case 2: splineChange(playerSpawn3, fourthSpline);
                break;
                
        }
    }

        private void splineChange(Transform playerSpawn, SplineContainer nextSpline)
        {
            fadeOut.Fade(0.5f, true, () =>
            {
                Debug.Log("finished fade");
                train.GetComponent<SplineAnimate>().Container = nextSpline;
                ghost.MoveToNextWaypoint();
                train.GetComponent<SplineAnimate>()?.Restart(false);
                if (!TrainCutScene(true))
                {

                    //train.GetComponent<SplineAnimate>().Pause();
                    player.transform.position = playerSpawn.position;
                    playerCamera.Priority = 10;
                    trainCamera.Priority = 0;
                    if (Camera.main != null) Camera.main.cullingMask |= (1 << LayerMask.NameToLayer("Player"));
                    player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    fadeOut.Fade(0.7f, 0.2f);
                }

                roadPart++;
            });
        }
} 
