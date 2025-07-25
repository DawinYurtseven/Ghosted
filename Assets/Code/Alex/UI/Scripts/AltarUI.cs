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
    [SerializeField] private BrushStrokeCanvas brushStroke;
    [SerializeField] private GameObject defaultButton;

    
    private int originalMask;
    private int altarMask;

    void Start()
    {
        originalMask = Camera.main.cullingMask;
        int playerLayer = LayerMask.NameToLayer("Player");
        altarMask = ~(1 << playerLayer);  
        emotionChange.SetActive(false);
    }
    
    public void ActivateUI()
    {
        Camera.main.cullingMask &= altarMask;
        emotionChange.SetActive(true);
        onActivateUI?.Invoke();
        CameraManager.Instance.turnOnOtherCamera(altarCamera);
        PlayerInputDisabler.Instance.SwitchInputMapDelayed("AltarUI");
        uiAnimator.Show(() =>
        {
            brushStroke.Animate(() => {
                 EventSystem.current.SetSelectedGameObject(null);
                 EventSystem.current.SetSelectedGameObject(defaultButton);});
        });
    }

    public void DeactivateUI()
    {
        uiAnimator.Hide(() => {
            PlayerInputDisabler.Instance.SwitchInputMapDelayed("Character Control");
            CameraManager.Instance.turnoOffOtherCamera(altarCamera);
            
            emotionChange.SetActive(false);
            Camera.main.cullingMask = originalMask;
            brushStroke.Reset();
            onDeactivateUI?.Invoke();
        });
        
    }
}
