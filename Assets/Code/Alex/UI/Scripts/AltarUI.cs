using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AltarUI : MonoBehaviour
{

    [SerializeField] private GameObject emotionChange;
    [SerializeField] private UIAnimator uiAnimator;
    [SerializeField] public UnityEvent onActivateUI;
    [SerializeField] public UnityEvent onDeactivateUI;
    [SerializeField] public CinemachineVirtualCamera altarCamera;
    [SerializeField] private BrushStroke brushStroke;
    [SerializeField] private GameObject defaultButton;

    

    void Start()
    {
        brushStroke.ResetStroke();
        emotionChange.SetActive(false);
    }
    
    public void ActivateUI()
    {
        emotionChange.SetActive(true);
        onActivateUI?.Invoke();
        CameraManager.Instance.turnOnOtherCamera(altarCamera);
        PlayerInputDisabler.Instance.DisableInput();
        uiAnimator.Show(() =>
        {
            brushStroke.AnimateBrush(() => {
                 EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(defaultButton);});
        });
    }

    public void DeactivateUI()
    {
        uiAnimator.Hide(() => {
            onDeactivateUI?.Invoke();
            CameraManager.Instance.turnoOffOtherCamera(altarCamera);
            PlayerInputDisabler.Instance.EnableInputWithDelay(2f);
            emotionChange.SetActive(false);
        });
        
    }
}
