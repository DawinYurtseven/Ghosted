using UnityEngine;

public class JumpStopHand : JumpPuzzle
{
    [SerializeField] private StopClockManager manager;
    
    [Header("Clock Hand Settings")]
    //[SerializeField] private ClockAnim clockAnim;
    [SerializeField] private ClockHand _hand;
    private bool isHandRunning = true;

    private void Awake()
    {
        if(manager == null)
        {
            manager = FindObjectOfType<StopClockManager>();
        }
    }

    void OnTriggerEnter()
    {
        toggleHand();
        manager.getInput(_hand, isHandRunning);
    }

    private void toggleHand()
    {
        if (isHandRunning)
        {
            isHandRunning = false;
            Debug.Log("Stopped hand: " + _hand);
        }
        else
        {
            isHandRunning = true;
            Debug.Log("Started hand: " + _hand);
        }
    }
}
