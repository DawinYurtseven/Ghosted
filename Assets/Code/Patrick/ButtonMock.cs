using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ButtonMock : MonoBehaviour
{
    [SerializeField] private bool pressed = false;
    [SerializeField] private GameObject btnOut;
    [SerializeField] private GameObject btnIn;  

    public UnityEvent<bool> onPressed; // = new UnityEvent<bool>(); // Event that is triggered with the state of the button
    public bool togglebar = false;
    public void toggle()
    {
        if (togglebar)
        {
            // toggle the button
            pressed = !pressed;
            toggleState();
            Debug.Log("Toggled button to " + pressed);
        }
        else
        {
            // only play short press animation
            PlayPunch();
        }

        onPressed.Invoke(pressed);
        Debug.Log("Pressed Button!");
    }

    private void toggleState()
    {
        btnOut?.SetActive(!pressed);
        btnIn?.SetActive(pressed);
    }
    
    private void PlayPunch()
    {
        btnOut.transform.DOPunchPosition(-transform.forward * 0.1f, 0.2f, 10, 1f);
    }
    
}
