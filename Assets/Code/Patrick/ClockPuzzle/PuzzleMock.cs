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
    
    public UnityEvent<int> solutionCorrectUntil = new UnityEvent<int>();
    
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
        currentStep = (currentStep + 1) % requiredSteps;
        if (checkSolution())
        {
            Debug.Log("Solved Puzzle!");
            puzzleSolved?.Invoke();
        }
        
        if (isCacheCorrectUntil(index))
        {
            Debug.Log("Cache is correct until " + index+"/"+ currentStep + ": " + getCacheString());
            solutionCorrectUntil?.Invoke(index);
        }
        else
        {
            Debug.Log("Resetting all sub-solution indicators");
            solutionCorrectUntil?.Invoke(0);
        }
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
    
    // a method to check if an int is at the correct position in the cache
    private bool isAtCorrectPosition(int index)
    {
        if (index < 0 || index >= correctOrder.Length)
        {
            Debug.LogError("Index out of bounds: " + index);
            return false;
        }
        
        if (cache.Count <= index)
        {
            Debug.Log("Cache does not have enough elements to check position " + index);
            return false;
        }
        
        int[] cacheArray = cache.ToArray();
        return cacheArray[index] == correctOrder[index];
    }
    
    private bool isCacheCorrectUntil(int index)
    {
        if (index < 0 || index >= correctOrder.Length || cache.Count < index)
        {
            Debug.Log("Index out of bounds: " + index + " Cache: " + cache.Count);
            return false;
        }

        if (index == 1) return true;
        
        bool fromBack = checkCacheFromBack(index);
        bool fromFront = checkCacheFromFront(index);
        if(cache.Count < correctOrder.Length )
        { 
            return fromFront;
        }
        return fromBack;
        
    }
    
    // TODO: merge the methods checkCacheFromFront and checkCacheFromBack
    private bool checkCacheFromFront(int index)
    {
        int[] cacheArray = cache.ToArray();
        
        // Check from front (correct order)
        for (int i = 0; i < index; i++)
        {
            if (cacheArray[i] != correctOrder[i])
            {
                Debug.Log("Checking from front: \n " +
                          "Cache not correct until Index " + index + ": " + cacheArray[i] + " != " + correctOrder[i]);
                return false;
            }
        }

        return true;
    }
    
    private bool checkCacheFromBack(int index)
    {
        int[] cacheArray = cache.ToArray();
        
        int j = 0;
        Debug.Log("Starting check from back for index: " + index + " with cache length: " + cacheArray.Length);
        Debug.Log("Checking i: " + (cacheArray.Length - index) + " | i > cacheArray.Length: " + ((cacheArray.Length - index) > cacheArray.Length));
        
        for (int i = cacheArray.Length - index; i < cacheArray.Length ; i++)
        {
            Debug.Log("Checking cache at: " + i+" for solution index: " + j);
            if (cacheArray[i] != correctOrder[j])
            {
                Debug.Log("Checking from back:\n " +
                          "Cache not correct until Index " + index + ": " + cacheArray[i] + " != " + correctOrder[i]);
                return false;
            }
            j++;
        }

        return true;
    }
    
    private string getCacheString()
    {
        return string.Join(", ", cache.Select(i => i.ToString()).ToArray());
    }
}
