using UnityEngine;

public class ClockSubsolutionMarker : MonoBehaviour
{
    public Material correctMaterial;
    public GameObject indicatorObj;
    [SerializeField] private int id = 0;
    [SerializeField] private PuzzleMock puzzleController;
    
    private Material originalMaterial;

    public void Start()
    {
        if(indicatorObj == null)
            indicatorObj = this.gameObject;
        if (!correctMaterial)
            Debug.LogError("Correct Material is not set on " + gameObject.name + " for solution indicator!");
        
        originalMaterial = GetComponent<Renderer>().material;
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
        if (index > 0 && id == index)
        {
            originalMaterial = GetComponent<Renderer>().material;
            GetComponent<Renderer>().material = correctMaterial;
            //indicatorObj.SetActive(true);
        }
        else
        {
            // Reset to original material if current solution is not correct
            GetComponent<Renderer>().material = originalMaterial;
            //indicatorObj.SetActive(false);
        }
    }

}
