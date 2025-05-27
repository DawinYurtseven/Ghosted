using UnityEngine;

public class RailUser : MonoBehaviour
{
    public RailElement connector;
    
    public void onEnergy()
    {
        Debug.Log("Connected " + this.name + " to circuit!");
    }
}
