using UnityEngine;
using UnityEngine.UI;

public class SelectOnEnable : MonoBehaviour
{
    private Selectable selectable;
    private void Awake()
    {
        selectable = GetComponent<Selectable>();
    }

    private void OnEnable()
    {
        if(selectable == null){
            Debug.LogWarning("No Selectable attached to GameObject. Are you sure you want to use SelectOnEnable?", gameObject);
            return;
        }
        selectable.Select();
    }
}