using UnityEngine;
using UnityEngine.Events;

public class StopClockManager : MonoBehaviour
{
    [Header("Clock Settings")]
    [SerializeField] private GameObject clockObj;
    public ClockAnim clockAnim;
    
    [Header("Clock Hands and Solutions")]
    public UnityEvent<bool> PuzzleSolved = new UnityEvent<bool>();
    [SerializeField] private int solutionHours = -1;
    [SerializeField] private int solutionMinutes = -1;
    [SerializeField] private int solutionSeconds = -1;
    
    [SerializeField] private float threshold = 30f; // threshold in degrees to check if the clock hand is correct
    private ClockHand currentHand;
    public ClockHand lastJumpedHand => currentHand;

    public bool isSolved { get; private set; } = false;
    
    public void getInput(ClockHand hand, bool isRunning)
    {
        Debug.Log("Got input for clock hand: " + hand + ", isRunning: " + isRunning);
        
        if(!isRunning)  // Clock was stopped
        {
            registerStep(hand);
        }
        else
        {
            // toggle Clock back into running state
            clockAnim.startHand(hand);
        }
    }
    
    public void registerStep(ClockHand hand)
    {
        // just stop the hand
        currentHand = hand;
        clockAnim.stopHand(currentHand);
        
        // if clock stopped, check solution
        if (!clockAnim.isClockRunning)
        {
            if (checkSolution())
            {
                Debug.Log("Clock hands are correct");
                clockAnim.AnimateSolution(true, solutionHours, solutionMinutes, solutionSeconds);
                PuzzleSolved?.Invoke(true);
                isSolved = true;
            }
            else
            {
                int [] currentTime = clockAnim.getCurrentTime();
                clockAnim.AnimateSolution(false, currentTime[0], currentTime[1], currentTime[2]);
                PuzzleSolved?.Invoke(false);
            }
        }
    }
    
    protected bool checkSolution()
    {
        Debug.Log("Checking solution for clock hands");
        
        // for all clock hands check if the time is correct
        if(solutionHours > 0 && !checkHand(ClockHand.Hour))
        {
            Debug.Log("Hour hand is not correct");
            return false;
        }
        
        if(solutionMinutes > 0 && !checkHand(ClockHand.Minute))
        {
            Debug.Log("Minute hand is not correct");
            return false;
        }
        
        if(solutionSeconds > 0 && !checkHand(ClockHand.Second))
        {
            Debug.Log("Second hand is not correct");
            return false;
        }
        
        Debug.Log("All clock hands are correct");
        return true;
    }
    
    private bool checkHand(ClockHand hand)
    {
        switch (hand)
        {
            case ClockHand.Hour:
                Debug.Log("Checking hour hand: " + clockAnim.hourHand.localEulerAngles.y + 
                          ", solution: " + solutionHours * 30f + 
                          ", threshold: " + threshold);
                return Mathf.Approximately(clockAnim.hourHand.localEulerAngles.y, solutionHours * 30f) || 
                       Mathf.Abs(clockAnim.hourHand.localEulerAngles.y - solutionHours * 30f) < threshold;
            case ClockHand.Minute:
                        bool min = Mathf.Abs(clockAnim.minuteHand.localEulerAngles.y - solutionMinutes * 6f) < threshold;
                        Debug.Log("Minute is inside threshold: " + min + 
                                  ", current angle: " + clockAnim.minuteHand.localEulerAngles.y + 
                                  ", solution angle: " + solutionMinutes * 6f +
                                  ", threshold angle: " + threshold);
                return Mathf.Approximately(clockAnim.minuteHand.localEulerAngles.y, solutionMinutes * 6f) || 
                       Mathf.Abs(clockAnim.minuteHand.localEulerAngles.y - solutionMinutes * 6f) < threshold;
            case ClockHand.Second:
                return Mathf.Approximately(clockAnim.secondHand.localEulerAngles.y, solutionSeconds * 6f) || 
                       Mathf.Abs(clockAnim.secondHand.localEulerAngles.y - solutionSeconds * 6f) < threshold;
            default:
                Debug.Log("Something went deeply wrong with the clock hand check");
                return false;
        }
    }
}
