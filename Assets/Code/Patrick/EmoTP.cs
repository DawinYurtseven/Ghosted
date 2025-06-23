using UnityEngine;

public class EmoTP : MonoBehaviour
{
    // subscribe to the EmotionSingletonMock onEmotionChanged event
    private EmotionSingletonMock emotionSingleton;
    [SerializeField] private float tpRaycastThresh = 0.5f; // Height adjustment to avoid clipping
    // a list of layers with all the surfaces that can be teleported to
    [SerializeField] private LayerMask surfaceLayerMask;
    [SerializeField] private float teleportHeightOffset = 1f; // Offset to avoid clipping when teleporting
    
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
                break;
            case Emotion.Fear:
                direction = Vector3.up; // Teleport downwards for Fear
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

        if (Physics.Raycast(startPosition, direction, out hit, Mathf.Infinity))
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
    }
}
