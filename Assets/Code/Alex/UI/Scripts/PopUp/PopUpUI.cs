using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PopUpUI : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform panelRoot;
    public Image iconImage;
    public TMP_Text headerText;
    public TMP_Text descriptionText;
    public CanvasGroup canvasGroup;

    [Header("Animation")]
    public float slideDuration = 0.5f;
    private Vector2 offScreenPos;
    private Vector2 onScreenPos;

    private bool isShowing = false;
    
    public static PopUpUI Instance { get; private set; }
    public List<PopUpData> popupDataList;
    
    private Dictionary<PopUpType, PopUpData> popupDict;

    private string savedActionMap = "";
    private void Awake()
    {
        onScreenPos = panelRoot.anchoredPosition;
        offScreenPos = onScreenPos + new Vector2(500f, 0f); // adjust for screen width
        panelRoot.anchoredPosition = offScreenPos;
        canvasGroup.alpha = 0;
        popupDict = popupDataList.ToDictionary(x => x.type, x => x);
    }

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }




    public void onAccepted(InputAction.CallbackContext context)
    {
        if (context.performed && isShowing)
        {
            HideHint();
        }
    }

    public void ShowHintByType(PopUpType type)
    {
        ShowHint(popupDict[type]);
    }
    
    public void ShowHint(PopUpData data)
    {
        savedActionMap = PlayerInputDisabler.Instance.GetCurrentActionMap();
        PlayerInputDisabler.Instance.SwitchInputMapDelayed("PopUp");
        iconImage.sprite = data.icon;
        headerText.text = data.header;
        descriptionText.text = data.description;

        isShowing = true;
        Time.timeScale = 0f;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        panelRoot.DOAnchorPos(onScreenPos, slideDuration).SetUpdate(true);
        canvasGroup.DOFade(1f, 0.3f).SetUpdate(true);
    }

    public void HideHint()
    {
        isShowing = false;
        panelRoot.DOAnchorPos(offScreenPos, slideDuration).SetUpdate(true);
        canvasGroup.DOFade(0f, 0.2f).SetUpdate(true).OnComplete(() =>
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            Time.timeScale = 1f;
            PlayerInputDisabler.Instance.SwitchInputMapDelayed(savedActionMap);
            savedActionMap = "";
        });
    }


    //To use in the end of the dialogue
    [SerializeField] private float defaultDelay = 2.5f;
    public void StartPopUpWithDelay(PopUpData data)
    {
        StartCoroutine(ShowPopUpWithDelayCoroutine(data));
    }

    private IEnumerator ShowPopUpWithDelayCoroutine(PopUpData data)
    {
        yield return new WaitForSeconds(defaultDelay);
        ShowHint(data);
    }
    
    public void StartPopUpWithDelay(PopUpType type)
    {
        StartCoroutine(ShowPopUpWithDelayCoroutine(popupDict[type]));
    }
}