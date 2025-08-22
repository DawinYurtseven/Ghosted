using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private Button backButton; // Back button from Options menu
    [SerializeField] private PlayerInput playerInput;

    private InputAction pauseAction;
    private bool isPaused;
    
    private Button lastButton;
    
    public CanvasGroup canvasGroup;

    [Header("Animation")]
    public RectTransform panelRoot;
    public float slideDuration = 0.5f;
    private Vector2 offScreenPos;
    private Vector2 onScreenPos;

    private bool isShowing = false;

    public static PauseMenu Instance { get; private set; }
    
    public void CurrentSelectedButton(Button button)
    {
        if (button != null)
        {
            lastButton = button;
        }
    }
    
    void Awake()
    {
        // Set up singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        onScreenPos = panelRoot.anchoredPosition;
        offScreenPos = onScreenPos + new Vector2(500f, 0f); // adjust for screen width
        panelRoot.anchoredPosition = offScreenPos;
        canvasGroup.alpha = 0;
    }

    public void OnPausePressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            TogglePause();
        }
    }
    

    private string previousActionMap = "";

    public void TogglePause()
    {
        if (isPaused)
        {
            DeactivateMenu();
            Debug.Log(previousActionMap);
            if (previousActionMap != "AltarUI")
            {
                PlayerInputDisabler.Instance.SwitchInputMap(previousActionMap);
            }
            if (lastButton != null)
            {
                lastButton.Select();
            }
        }
        else
        {
            if (Time.timeScale == 0)
            {
                return;
            }
            else
            {
                ActivateMenu();
                previousActionMap = PlayerInputDisabler.Instance.GetCurrentActionMap();
                if (previousActionMap != "AltarUI")
                {
                    PlayerInputDisabler.Instance.SwitchInputMap("AltarUI");
                }
            }
        }
    }
    
    private string savedActionMap = "";


    void ActivateMenu()
    {
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        FMODUnity.RuntimeManager.GetBus("bus:/").setPaused(true);
        
        isPaused = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        pauseUI.SetActive(true);
        panelRoot.DOAnchorPos(onScreenPos, slideDuration).SetUpdate(true);
        canvasGroup.DOFade(1f, 0.3f).SetUpdate(true);
    }

    public void DeactivateMenu()
    {
        panelRoot.DOAnchorPos(offScreenPos, slideDuration).SetUpdate(true);
        canvasGroup.DOFade(0f, 0.2f).SetUpdate(true).OnComplete(() =>
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            Time.timeScale = 1;
            FMODUnity.RuntimeManager.GetBus("bus:/").setPaused(false);
            pauseUI.SetActive(false);
            optionsUI.SetActive(false);
            isPaused = false;
        });
    }

    public void ResumeGame() => TogglePause();

    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        Time.timeScale = 1;
        FMODUnity.RuntimeManager.GetBus("bus:/").setPaused(false);
        pauseUI.SetActive(false);
        optionsUI.SetActive(false);
        isPaused = false;
        SceneManager.LoadScene("StartScreen");

    }

    public void OpenOptionsMenu()
    {
        pauseUI.SetActive(false);
        optionsUI.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        optionsUI.SetActive(false);
        pauseUI.SetActive(true);
    }
}