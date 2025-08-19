using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InactiveScreenLoader : MonoBehaviour
{
    [SerializeField] private float inactivityTime = 60f;
    [SerializeField] private string inactiveSceneName = "InactiveScreen";

    private float timer;
    private bool sceneLoaded;

    void Update()
    {
        // Check for input every frame, if there is input, reset the timer, otherwise increment the timer
        if (checkForInput())
        {
            timer = 0f;
            if (sceneLoaded)
            {
                // Unload the inactive scene if it is loaded
                SceneManager.UnloadSceneAsync(inactiveSceneName);
                sceneLoaded = false;
            }
        }
        else
        {
            timer += Time.unscaledDeltaTime;
            if (timer >= inactivityTime && !sceneLoaded)
            {
                // Load the inactive scene if the timer exceeds the inactivity time and the scene is not already loaded
                SceneManager.LoadSceneAsync(inactiveSceneName, LoadSceneMode.Additive);
                sceneLoaded = true;
            }
        }
    }
    
    private bool checkForInput()
    {
        bool mouseInput = Mouse.current != null && (Mouse.current.delta.ReadValue() != Vector2.zero ||
            Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame);
        
        bool keyboardInput = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
        
        bool controllerInput = false;
        if (Gamepad.current != null)
        {
            // Prüfe alle Buttons des Gamepads
            controllerInput =
                Gamepad.current.buttonSouth.wasPressedThisFrame ||
                Gamepad.current.buttonNorth.wasPressedThisFrame ||
                Gamepad.current.buttonEast.wasPressedThisFrame ||
                Gamepad.current.buttonWest.wasPressedThisFrame ||
                Gamepad.current.leftShoulder.wasPressedThisFrame ||
                Gamepad.current.rightShoulder.wasPressedThisFrame ||
                Gamepad.current.leftTrigger.wasPressedThisFrame ||
                Gamepad.current.rightTrigger.wasPressedThisFrame ||
                Gamepad.current.startButton.wasPressedThisFrame ||
                Gamepad.current.selectButton.wasPressedThisFrame ||
                Gamepad.current.leftStick.ReadValue() != Vector2.zero ||
                Gamepad.current.rightStick.ReadValue() != Vector2.zero ||
                Gamepad.current.dpad.ReadValue() != Vector2.zero;
        }
        return mouseInput || keyboardInput || controllerInput;
    }
    
}
