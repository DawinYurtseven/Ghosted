using UnityEngine;
using UnityEngine.Events;

public class RailSignalControler : MonoBehaviour
{
    [SerializeField] private GameObject[] lampObjs;     // alle lampen
    [SerializeField] private MeshRenderer indicator;    // der renderer von hebel an der seite
    [SerializeField] private bool lightGreen;           //steps ok
    [SerializeField] private bool indicatorGo;
    [SerializeField] private bool green;                //everything is solved
    [SerializeField] private HebelAnim _hebelAnim;      // um den Hebel an der Seite zu switchen
    
    public Material stop;
    public Material go;

    public UnityEvent onAllGreen;
    
    public void switchLights(bool state)
    {
        lightGreen = state;
        
        foreach (GameObject lampObj in lampObjs)
        {
            MeshRenderer r = lampObj.GetComponent<MeshRenderer>();
            r.material =  lightGreen? go : stop;
        }   
        
        checkIsGreen();
    }

    public void switchIndicator(bool state)
    {
        indicatorGo = state;
        indicator.material = state ? go : stop;
        _hebelAnim?.Toggle(state);
        checkIsGreen();
    }

    public void OnSolved()
    {
        switchIndicator(true);
        switchLights(true);

        green = lightGreen = indicatorGo =  true;
    }

    public bool isGreen() => lightGreen && indicatorGo;

    private void checkIsGreen()
    {
        if (isGreen())
        {
            onAllGreen?.Invoke();
        }
    }
}
