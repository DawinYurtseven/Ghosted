using UnityEngine;
using UnityEngine.Events;

// class to handle element level of rail puzzle
public class RailWeiche : MonoBehaviour
{
    public string switchID;
    public MeshRenderer straightSegment;
    public MeshRenderer divergingSegment;
    public Material activeMaterial;
    public Material inactiveMaterial;

    [SerializeField] private bool isStraight = true;

    [SerializeField] private GameObject indicatorStraight;
    [SerializeField] private GameObject indicatorCurve;

    [SerializeField] private string straightIDsuffix = ".straight";
    [SerializeField] private string divergeIDsuffix = ".diverge";

    public static UnityEvent<string> onSwitch = new UnityEvent<string>();

    [SerializeField] private HebelAnim animIndicator;
    
    // Toggle the switch state
    public void Toggle()
    {
        isStraight = !isStraight;
        Debug.Log("Set " + switchID + " to isStraight: " + isStraight);
        
        UpdateID();
        UpdateMaterials();
        
        if(animIndicator)
            animIndicator.Toggle(isStraight);
        else
        {
            indicatorCurve.SetActive(!isStraight);
            indicatorStraight.SetActive(isStraight);
        }
        
        onSwitch?.Invoke(switchID);
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
        string res = switchID;

        if (isStraight)
        {
            res += straightIDsuffix;
        }
        else
        {
            res += divergeIDsuffix;
        }

        Debug.Log("Returning id with isStraight: " + isStraight);
        return res;
    }

    public void UpdateID()
    {
        switchID = switchID.Split('.')[0];
        
        if (isStraight)
        {
            switchID += straightIDsuffix;
        }
        else
        {
            switchID += divergeIDsuffix;
        }
    }
    
    public bool isWeicheStraight()
    {
        return isStraight;
    }

    private void UpdateMaterials()
    {
        straightSegment.material = isStraight ? activeMaterial : inactiveMaterial;
        divergingSegment.material = isStraight ? inactiveMaterial : activeMaterial;
    }
}
