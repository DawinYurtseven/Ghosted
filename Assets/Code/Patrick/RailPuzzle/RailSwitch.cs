using UnityEngine;
using UnityEngine.Events;

public class RailSwitch : MonoBehaviour
{
    public UnityEvent switchWeiche;
    [SerializeField] private bool up = true;
    [SerializeField] private RailWeiche Weiche;
    [SerializeField] private GameObject hebelUp;
    [SerializeField] private GameObject hebelDown;
    private void OnEnable()
    {
        switchWeiche.AddListener(OnSwitch);
    }

    private void OnDisable()
    {
        switchWeiche.RemoveListener(OnSwitch);
    }

    public void OnSwitch()
    {
        toggle();
    }

    public void envokeEvent()
    {
        switchWeiche?.Invoke();
    }
    
    private void toggle()
    {
        up = !up;
        
        hebelUp.SetActive(up);
        hebelDown.SetActive(!up);
    }
}
