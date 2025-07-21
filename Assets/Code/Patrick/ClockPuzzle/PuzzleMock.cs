using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleMock : MonoBehaviour
{
    public int requiredSteps = 3;
    [SerializeField] private int[] correctOrder = {3,1,2};
    [SerializeField] private Queue<int> cache = new Queue<int>();
    public UnityEvent puzzleSolved;
    private int currentStep = -1;   // offset for the first registered step
    public bool isRepeatable = false;
    private bool wasPuzzleSolved = false;
    
    public bool isSolved()
    {
        return checkSolution();
    }

    public bool WasSolved() => wasPuzzleSolved;
    
    // returns whether the current step is a solution, eg. if feedback needs to be provided
    public bool isSolution() => requiredSteps == currentStep;

    public int getCurrentStep() => currentStep;
    
    public void registerStep(int index)
    {
        if(wasPuzzleSolved && !isRepeatable)
            return;
        
        Debug.Log("Registered current puzzle index: " + index);
        
        addToCache(index);
        if (checkSolution())
        {
            Debug.Log("Solved Puzzle!");
            puzzleSolved?.Invoke();
        }

        currentStep = (currentStep + 1) % requiredSteps;
    }

    private bool checkSolution()
    {
        if (cache.Count != correctOrder.Length)
        {
            Debug.Log("Puzzle not solved, wrong number of steps: " + cache.Count + " vs " + correctOrder.Length);
            return false;
        }
        
        
        int i = 0;
        foreach (var val in cache)
        {
            if (val != correctOrder[i]) return false;
            i++;
        }

        wasPuzzleSolved = true;
        //Debug.Log("Solved Puzzle!");
        return true;
    }
    
    private void addToCache(int newStep)
    {
        // If the new step is the lastly added step in the cache, don't add it again
        if(cache.Count > 0 && cache.Last() == newStep)
        {
            Debug.Log("Step " + newStep + " already in cache (" + cache + "), not adding again.");
            return;
        }
        
        cache.Enqueue(newStep);
        adjustCache();
    }
    
    private void adjustCache()
    {
        if (cache.Count > requiredSteps)
        {
            while (cache.Count > requiredSteps)
            {
                cache.Dequeue();
            }
        }
    }
    
}
