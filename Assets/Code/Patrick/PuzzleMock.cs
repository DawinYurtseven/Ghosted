using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleMock : MonoBehaviour
{
    public int requiredSteps = 3;
    [SerializeField] private int[] correctOrder = {3,1,2};
    private Queue<int> cache = new Queue<int>();
    public UnityEvent puzzleSolved;
    
    public void registerStep(int index)
    {
        Debug.Log("Registered current puzzle index: " + index);
        
        addToCache(index);
        if (checkSolution())
        {
            Debug.Log("Solved Puzzle!");
            puzzleSolved?.Invoke();
        }
    }

    private bool checkSolution()
    {
        if (cache.Count != correctOrder.Length) return false;
        int i = 0;
        foreach (var val in cache)
        {
            if (val != correctOrder[i]) return false;
            i++;
        }
        return true;
    }

    
    private void addToCache(int newStep)
    {
        cache.Enqueue(newStep);
        adjustCache();
    }
    
    private void adjustCache()
    {
        if (cache.Count > requiredSteps)
        {
            while (cache.Count>requiredSteps)
            {
                cache.Dequeue();
            }
        }
    }
}
