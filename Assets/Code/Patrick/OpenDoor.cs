using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject indicator;
    [SerializeField] private bool isClosed = true;
    [SerializeField] private bool allowToggle = false;
    
    public void OnInteract()
    {
        setDoor(!isClosed);

        if (allowToggle)
            isClosed = !isClosed;
    }

    private void setDoor(bool status)
    {
        door.SetActive(status);
        indicator.SetActive(status);
    }
}
