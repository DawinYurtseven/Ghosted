using UnityEngine;

public class EmoTP : MonoBehaviour
{
    // subscribe to the EmotionSingletonMock onEmotionChanged event
    private EmotionSingletonMock emotionSingleton;
    [SerializeField] private float tpRaycastThresh = 0.5f; // Height adjustment to avoid clipping
    // a list of layers with all the surfaces that can be teleported to
    [SerializeField] private LayerMask surfaceLayerMask;
    [SerializeField] private float teleportHeightOffset = 1.7f; // Offset to avoid clipping when teleporting
    
    private void Awake()
    {
        emotionSingleton = FindObjectOfType<EmotionSingletonMock>();
        if (emotionSingleton == null)
        {
            Debug.LogError("No EmotionSingletonMock found in the scene!");
        }
    }
    
    private void OnEnable()
    {
        if (emotionSingleton != null)
        {
            emotionSingleton.emotionChanged.AddListener(TeleportToNextSurface);
        }
    }
    
    private void OnDisable()
    {
        if (emotionSingleton != null)
        {
            emotionSingleton.emotionChanged.RemoveListener(TeleportToNextSurface);
        }
    }
    
    // a method that teleports the player to the next surface, depending on the current emotion
    public void TeleportToNextSurface(Emotion emotion)
    {
        Vector3 direction;

        // Determine the direction based on the current emotion
        switch (emotion)
        {
            case Emotion.Joy:
                direction = Vector3.down; // Teleport upwards for Joy
                Debug.Log("Casting ray downwards for Joy.");
                break;
            case Emotion.Fear:
                direction = Vector3.up; // Teleport downwards for Fear
                Debug.Log("Casting ray upwards for Fear.");
                break;
            case Emotion.Lonely:
            case Emotion.Love:
                
            default:
                direction = Vector3.up; // Default to upwards for other emotions
                break;
        }

        // Perform the raycast to find the next surface
        PerformRaycast(direction);
    }
    
    private void PerformRaycast(Vector3 direction)
    {
        RaycastHit hit;
        Vector3 startPosition = transform.position + Vector3.up * tpRaycastThresh; // Start slightly above the player
        // Ignore the player's collider and "ignore" layer during the raycast
        if (Physics.Raycast(startPosition, direction, out hit, 50f))
        {
            // Check if the hit point is a valid surface
            if (hit.collider != null && (hit.collider.gameObject.layer & surfaceLayerMask) != 0)
            {
                Debug.Log($"Teleporting to {hit.point} due to emotion change.");
                
                // Teleport the player to the hit point
                transform.position = hit.point + Vector3.up * teleportHeightOffset; // Adjust height to avoid clipping
            }
            else
            {
                Debug.LogWarning("Raycast hit an invalid surface, not teleporting.");
                Debug.Log($"Hit object: {hit.collider?.gameObject.name ?? "null"}");
            }
        }
        else
        {
            Debug.LogWarning("Raycast did not hit any valid surface.");
            // teleport player upwards if no surface was hit
            Vector3 newPosition = startPosition + direction * 5f;
            Debug.Log($"No surface hit, teleporting to {newPosition}.");
            transform.position = newPosition; // Teleport upwards if no surface was hit
        }
    }
    
    // private void OnDrawGizmosSelected()
    // {
    //     if (emotionSingleton != null)
    //     {
    //         Vector3 startPosition = transform.position + Vector3.up * tpRaycastThresh;
    //         Color rayColor = Color.green; // Default color for Joy
    //         Vector3 direction = Vector3.down; // Default direction for Joy
    //         
    //         switch (emotionSingleton.getCurrentEmotion())
    //         {
    //             case Emotion.Joy:
    //                 rayColor = Color.green; // Joy
    //                 direction = Vector3.down; // Joy raycast direction
    //                 break;
    //             case Emotion.Fear:
    //                 rayColor = Color.red; // Fear
    //                 direction = Vector3.up; // Fear raycast direction
    //                 break;
    //             case Emotion.Lonely:
    //             case Emotion.Love:
    //                 rayColor = Color.blue; // Other emotions
    //                 break;
    //         }
    //         
    //         Gizmos.color = rayColor;
    //         Gizmos.DrawLine(startPosition, startPosition + direction * 20f); // Draw the raycast line
    //     }
    // }
    //
    // private void OnDrawGizmos()
    // {
    //     // Draw the raycast direction in the editor for debugging, one color for Joy and another for Fear
    //     if (emotionSingleton != null)
    //     {
    //         Vector3 startPosition = transform.position + Vector3.up * tpRaycastThresh;
    //         Color rayColor = Color.green; // Default color for Joy
    //         Vector3 direction = Vector3.down; // Default direction for Joy
    //         
    //         switch (emotionSingleton.getCurrentEmotion())
    //         {
    //             case Emotion.Joy:
    //                 rayColor = Color.green; // Joy
    //                 direction = Vector3.down; // Joy raycast direction
    //                 break;
    //             case Emotion.Fear:
    //                 rayColor = Color.red; // Fear
    //                 direction = Vector3.up; // Fear raycast direction
    //                 break;
    //             case Emotion.Lonely:
    //             case Emotion.Love:
    //                 rayColor = Color.blue; // Other emotions
    //                 break;
    //         }
    //         
    //         Gizmos.color = rayColor;
    //         Gizmos.DrawLine(startPosition, startPosition + direction * 20f); // Draw the raycast line
    //     }
    //     else
    //     {
    //         Gizmos.color = Color.yellow;
    //         Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 20f); // Default raycast line if no EmotionSingletonMock found
    //     }
    // }
}
