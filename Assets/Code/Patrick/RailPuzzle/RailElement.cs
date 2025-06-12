using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshRenderer))]
public class RailElement : MonoBehaviour
{
    [SerializeField] private string elementID; // e.g. "A1", "A2", etc.
    public RailController controller;
    public string prevWeiche;                   //technically not needed anymore
    public Material activeMaterial;
    public Material inactiveMaterial;
    
    private MeshRenderer meshRenderer;
    [SerializeField] private bool needEnergy = true;
    [SerializeField] private bool isActive = false;

    public UnityEvent onConnection;
    public UnityEvent onDissconnect;
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        
        if(controller)
            UpdateMaterial("");
        
        if (!needEnergy)
            meshRenderer.material = activeMaterial;
        else meshRenderer.material = inactiveMaterial;
    }

    private void OnEnable()
    {
        RailWeiche.onSwitch?.AddListener(UpdateMaterial);
    }

    private void OnDisable()
    {
        RailWeiche.onSwitch?.RemoveListener(UpdateMaterial);
    }

    // Call this to refresh material based on current switch path
    public void UpdateMaterial(string weichenID)
    {
        Debug.Log("Gleis mat updating at " + elementID +", for " + weichenID);
        
        if(!needEnergy)
            return;
        
        if (elementID.Equals(weichenID))
            isActive = true;
        else
            isActive = false;
        
        meshRenderer.material = isActive ? activeMaterial : inactiveMaterial;
        
        if(isActive) onConnection?.Invoke();
        else onDissconnect?.Invoke();
        
        // // Determine if this elementID matches the next output of the previous switch
        // RailWeiche prevSwitch = controller.alleWeichen.Find(s => s.switchID.Equals(prevWeiche));
        // if (prevSwitch == null)
        // {
        //     Debug.Log("no weiche found with id " + prevWeiche);
        //     meshRenderer.material = inactiveMaterial;
        //     return;
        // }

        // Debug.Log("Updating because Switch: " + prevSwitch.GetNextSwitchID() + " was altered, elementID: " + elementID);
        // bool isActive = prevSwitch.GetNextSwitchID().Equals(elementID);
        // meshRenderer.material = isActive ? activeMaterial : inactiveMaterial;
    }

    public bool isConnected()
    {
        if (!needEnergy) return true;
        return meshRenderer.material == activeMaterial;
    }
}
