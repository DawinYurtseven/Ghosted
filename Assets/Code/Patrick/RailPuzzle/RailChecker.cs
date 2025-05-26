using System.Collections.Generic;
using UnityEngine;

public class RailChecker : MonoBehaviour
{
    /*
     * TrainPathChecker.cs
     * Starting from an entry switch, follows the chain of outputs to determine the final path.
     */
    public RailController controller;
    public List<string> solution = new List<string>();
    
    public void Start()
    {
        if (!controller) FindObjectOfType<RailController>();
        if(!controller) Debug.LogWarning("No Controller assigned to the railChecker " + this + "!");

        if (solution.Count <= 0) Debug.LogWarning("No solution given for the rail puzzle!");
    }

    // Given a starting switch ID, returns the ordered list of outputs until no next exists
    public List<string> GetCurrentPath(string startID)
    {
        var path = new List<string>();
        var currentID = startID;
        var visited = new HashSet<string>();

        while (!string.IsNullOrEmpty(currentID) && !visited.Contains(currentID))
        {
            visited.Add(currentID);
            path.Add(currentID);
            RailWeiche weiche = controller.alleWeichen.Find(s => s.switchID == currentID);
            if (weiche == null) break;
            currentID = weiche.GetNextSwitchID();
        }

        return path;
    }

    public bool CheckPath(string startID)
    {
        List<string> path = GetCurrentPath(startID);

        if (solution.Count != path.Count)
            return false;
        
        for (int i = 0; i < solution.Count; i++)
        {
            if (!solution[i].Equals(path[i])) return false;
        }

        return true;
    }
    
    public void PrintPath(string startID)
    {
        var path = GetCurrentPath(startID);
        Debug.Log("Train path: " + string.Join(" -> ", path));
    }

}
