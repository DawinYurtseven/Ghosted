using System.Collections.Generic;
using UnityEngine;

public class RailController : MonoBehaviour
{
    /*
     * SwitchController.cs
     * Manages multiple RailSwitch instances and dispatches toggle commands.
     */
    public List<RailWeiche> alleWeichen = new List<RailWeiche>();
    
    // set and check all needed Objects (the junction)
    public void Awake()
    {   
        // if list is empty, get all Weichen from the scene
        if (alleWeichen.Count <= 0)
        {
            foreach (RailWeiche weiche in FindObjectsOfType<RailWeiche>())
            {
                alleWeichen.Add(weiche);
            }

            Debug.Log("Found " + alleWeichen.Count + " junctions in the scene.");
        }
        
        if(alleWeichen.Count == 0) Debug.LogWarning("No rail junctions in the controller!");
    }

    public void OnEnable()
    {
        RailSwitch.switchWeiche?.AddListener(ToggleSwitch);
    }

    public void OnDisable()
    {
        RailSwitch.switchWeiche?.RemoveListener(ToggleSwitch);
    }

    // Toggle a switch by ID
    public void ToggleSwitch(string id)
    {
        var sw = alleWeichen.Find(s => s.switchID.Equals(id));
        if (sw != null)
            sw.Toggle();
        else
            Debug.LogWarning($"Switch {id} not found");
    }

    // Set a switch to straight or diverging
    public void SetSwitch(string id, bool straight)
    {
        var sw = alleWeichen.Find(s => s.switchID == id);
        if (sw != null)
            sw.SetStraight(straight);
        else
            Debug.LogWarning($"Switch {id} not found");
    }
}
