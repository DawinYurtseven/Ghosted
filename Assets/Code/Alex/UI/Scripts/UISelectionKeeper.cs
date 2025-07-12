using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISelectionKeeper : MonoBehaviour
{
    private GameObject lastSelected;

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            lastSelected = EventSystem.current.currentSelectedGameObject;
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && lastSelected != null)
        {
            // Restore selection when the window regains focus
            EventSystem.current.SetSelectedGameObject(lastSelected);
        }
    }
}