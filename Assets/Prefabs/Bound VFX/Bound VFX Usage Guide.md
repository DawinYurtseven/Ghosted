# Bound VFX Usage Guide

## Where to Find the Prefab

After pulling the latest changes from the `shader` branch, you can find the prefab here:

```
Assets > Prefabs > Bound VFX
```

---

## How to Use It

The prefab comes with the `BoundVFXController` script already attached. This script provides two main functions:

```csharp
public void PlayVFXAt(Transform parent, Emotion emotion)
```  
This starts and attaches the VFX to the given transform. The effect uses the specified emotion to control its visual appearance (colour, gradients, texture, etc.).

**Parameters**:

- `Transform parent`: The transform where the VFX should appear and attach to
- An `Emotion emotion` enum value (already set up in the project)

```csharp
public void StopVFX()
```  
Fades out the VFX and removes it from the scene when finished.

**The visual style will automatically match the selected emotion.**

---
## Example Usage

This is a code snippet from `Assets > Code > Ben > Scripts > VFX > "ExampleVFXUsage.cs"`. 

The example scene can be found in `Assets > Prefabs > "Bound VFX Example"`. 

```csharp
public class ExampleVFXUsage : MonoBehaviour
{
    // Reference to the VFX prefab (found in Assets > Prefabs > Bound VFX)
    public GameObject vfxPrefab;

    // The target transform where the VFX should appear and attach
    public Transform target;

    // The emotion to be used when playing the VFX (defines visual style)
    public Emotion emotions;

    // Holds the current active VFX instance
    private BoundVFXController vfxInstance;

    private void Update()
    {
        // Play the VFX when SPACE is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // If a VFX is already playing, destroy the previous one
            if (vfxInstance != null)
                Destroy(vfxInstance.gameObject);

            // Instantiate a new VFX prefab
            var go = Instantiate(vfxPrefab);
            go.SetActive(true); // Ensure it's active in case the prefab was disabled

            // Get the BoundVFXController component and start the effect
            vfxInstance = go.GetComponent<BoundVFXController>();
            vfxInstance.PlayVFXAt(target, emotions);
        }

        // Stop the VFX when R is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Only stop if a VFX is currently active
            if (vfxInstance != null)
                vfxInstance.StopVFX();
        }
    }
}
``` 

---

## Customising the Look

You can tweak how each emotion looks by editing the prefab in the Inspector. The following properties are available for each emotion:

- **Base Colours**
- **Pulse Foreground Gradients**
- **Pulse Background Gradients**
- **Character Textures**

Adjust these to define how each emotion appears visually in the effect.
