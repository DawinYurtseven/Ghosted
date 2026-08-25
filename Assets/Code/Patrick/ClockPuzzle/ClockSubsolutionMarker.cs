using UnityEngine;

public class ClockSubsolutionMarker : MonoBehaviour
{
    public Material correctMaterial;
    public GameObject indicatorObj;
    [SerializeField] private int id = 0;
    [SerializeField] private PuzzleMock puzzleController;
    [SerializeField] private MaterialChanger changer;
    private Material originalMaterial;

    public void Start()
    {
        if(indicatorObj == null)
            indicatorObj = this.gameObject;
        if (!correctMaterial)
            Debug.LogError("Correct Material is not set on " + gameObject.name + " for solution indicator!");
        changer = indicatorObj.GetComponent<MaterialChanger>();

        originalMaterial = indicatorObj.GetComponent<Renderer>().material;
    }

    public void OnEnable()
    {
        puzzleController.solutionCorrectUntil.AddListener(OnSolutionCorrectUntil);
    }
    
    public void OnDisable()
    {
        puzzleController.solutionCorrectUntil.RemoveListener(OnSolutionCorrectUntil);
    }
    
    private void OnSolutionCorrectUntil(int index)
    {
        //Debug.Log("Got solution correct until: " + index + " for id: " + id);
        if (index > 0 && id <= index)
        {
            indicatorObj.GetComponent<Renderer>().material = correctMaterial;
            if (changer) changer.isChangerDisabled = true;
        }
        else
        {
            // Reset to original material if current solution is not correct
            if (changer != null)
            {
                changer.isChangerDisabled = false;
                changer.ChangeMaterial(EmotionSingletonMock.Instance.getCurrentEmotion());
            }
            else indicatorObj.GetComponent<Renderer>().material = originalMaterial;
        }
    }

}
