using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class RailElement : MonoBehaviour
{
    public string elementID; // e.g. "A1", "A2", etc.
    public RailController controller;
    public string prevWeiche;
    public Material activeMaterial;
    public Material inactiveMaterial;
    
    private MeshRenderer meshRenderer;
    [SerializeField] private bool needEnergy = true;
    
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        
        if(controller)
            UpdateMaterial();
        
        if (!needEnergy)
            meshRenderer.material = activeMaterial;
        else meshRenderer.material = inactiveMaterial;
    }

    // Call this to refresh material based on current switch path
    public void UpdateMaterial()
    {
        // Determine if this elementID matches the next output of the previous switch
        RailWeiche prevSwitch = controller.alleWeichen.Find(s => s.switchID.Equals(prevWeiche));
        if (prevSwitch == null)
        {
            Debug.Log("no weiche found with id " + prevWeiche);
            meshRenderer.material = inactiveMaterial;
            return;
        }

        Debug.Log("Switch ID: " + prevSwitch.GetNextSwitchID() + ", elementID: " + elementID);
        bool isActive = prevSwitch.GetNextSwitchID() == elementID;
        meshRenderer.material = isActive ? activeMaterial : inactiveMaterial;
    }
}
