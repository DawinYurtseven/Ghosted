using UnityEngine;

// class to handle element level of rail puzzle
public class RailWeiche : MonoBehaviour
{
    public string switchID;
    public MeshRenderer straightSegment;
    public MeshRenderer divergingSegment;
    public Material activeMaterial;
    public Material inactiveMaterial;

    private bool isStraight = true;
    
    // Toggle the switch state
    public void Toggle()
    {
        isStraight = !isStraight;
        UpdateMaterials();
    }

    // Set direct state
    public void SetStraight(bool straight)
    {
        isStraight = straight;
        UpdateMaterials();
    }

    // Returns next switch ID based on current setting
    public string GetNextSwitchID()
    {
        // Example: switchID = "A", outputs named "A1" (straight) and "A2" (diverge)
        return isStraight ? switchID + ".straight" : switchID + ".diverge";
    }

    private void UpdateMaterials()
    {
        straightSegment.material = isStraight ? activeMaterial : inactiveMaterial;
        divergingSegment.material = isStraight ? inactiveMaterial : activeMaterial;
    }
}
