using UnityEngine;

public class OpenClock : MonoBehaviour
{
    [Header("Clock Parts")]
    [SerializeField] private GameObject clockClosed;
    [SerializeField] private GameObject clockOpen;  // Reference to the clock UI GameObject
    [SerializeField] private GameObject handMin;
    [SerializeField] private GameObject handSec;
    // [SerializeField] private GameObject handHour; // Optional, if you want to animate the hour hand as well
    
    [Header("Animation Settings")]
    [SerializeField] private SpawnAnim spawnAnim;   // Reference to the SpawnAnim script for animation
    [SerializeField] private Transform minGoal;
    [SerializeField] private Transform secGoal;
    // [SerializeField] private Transform hourGoal; // Optional, if you want to animate the hour hand as well
    public void onPuzzleSolved()
    {
        Debug.Log("Puzzle solved, opening clock");
        
        // Disable the closed clock if it exists
        if (clockClosed != null)
        {
            clockClosed.SetActive(false);
        }
        
        // Activate the open clock
        if (clockOpen != null)
        {
            clockOpen.SetActive(true);
        }
        
        // Trigger the spawn animation
        if (handMin != null && handSec != null)
        {
            SpawnAnim.moveTo(handMin.transform, minGoal, 1f);
            SpawnAnim.moveTo(handSec.transform, secGoal, 1f);
        }
    }
}
