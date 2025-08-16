using UnityEngine;
using UnityEngine.InputSystem;
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

    void ActivateMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        FMODUnity.RuntimeManager.GetBus("bus:/").setPaused(true);
        pauseUI.SetActive(true);
        isPaused = true;
    }

    public void DeactivateMenu()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        Time.timeScale = 1;
        FMODUnity.RuntimeManager.GetBus("bus:/").setPaused(false);
        pauseUI.SetActive(false);
        optionsUI.SetActive(false);
        isPaused = false;
    }

    public void ResumeGame() => TogglePause();

    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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