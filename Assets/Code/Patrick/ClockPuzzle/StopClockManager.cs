using UnityEngine;
using UnityEngine.Events;

public class StopClockManager : MonoBehaviour
{
    [Header("Clock Settings")]
    [SerializeField] private GameObject clockObj;
    [SerializeField] private ClockAnim clockAnim;
    
    [Header("Clock Hands and Solutions")]
    public UnityEvent onPuzzleSolved;
    [SerializeField] private int solutionHours = -1;
    [SerializeField] private int solutionMinutes = -1;
    [SerializeField] private int solutionSeconds = -1;
    
    [SerializeField] private float threshold = 5f; // threshold in degrees to check if the clock hand is correct
    
    private ClockHand currentHand;
    private bool isClockRunning = true;

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
        if(!isClockRunning)
            return;
        
        currentHand = hand;
        
        // check if the solution is correct
        if (checkSolution())
        {
            Debug.Log("Clock hands are correct");
            clockAnim.AnimateSolution(true, solutionHours, solutionMinutes, solutionSeconds);
            onPuzzleSolved?.Invoke();
        }
        else
        {
            Debug.Log("Clock hands are not correct yet");
            // get the current time of the clock
            int [] currentTime = clockAnim.getCurrentTime();
            clockAnim.AnimateSolution(false, currentTime[0], currentTime[1], currentTime[2]);
            clockAnim.setTime(currentTime[0], currentTime[1], currentTime[2]);
            clockAnim.startHand(hand);
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
                return clockAnim.hourHand.localEulerAngles.z == solutionHours * 30f || 
                       Mathf.Abs(clockAnim.hourHand.localEulerAngles.z - solutionHours * 30f) < threshold;
            case ClockHand.Minute:
                return clockAnim.minuteHand.localEulerAngles.z == solutionMinutes * 6f || 
                       Mathf.Abs(clockAnim.minuteHand.localEulerAngles.z - solutionMinutes * 6f) < threshold;
            case ClockHand.Second:
                return clockAnim.secondHand.localEulerAngles.z == solutionSeconds * 6f || 
                       Mathf.Abs(clockAnim.secondHand.localEulerAngles.z - solutionSeconds * 6f) < threshold;
            default:
                Debug.Log("Something went deeply wrong with the clock hand check");
                return false;
        }
    }
}
