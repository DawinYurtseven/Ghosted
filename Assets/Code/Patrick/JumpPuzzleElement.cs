using UnityEngine;

public class JumpPuzzleElement : MonoBehaviour
{
    public int objectID = 0;              // 0,1,2
    private PuzzleMock manager;
    void Awake() => manager = FindObjectOfType<PuzzleMock>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.registerStep(objectID);
        }
    }
}
