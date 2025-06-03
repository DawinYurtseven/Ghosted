# Bound VFX Usage Guide

### Where to find the prefab?

After you pull the changes from `shader` branch, you can find it in `Assets > Prefabs > Bound VFX`.

### How to use it?

The prefab comes with a `BoundVFXController` script already attached. You can use its main function like this:

```csharp
public void PlayAt(Transform parent, Emotion emotion)
``` 

Just call this function to spawn the VFX and attach it to whatever object you want (pass in the transform). The `emotion` is an enum that one of ya'll already set up — use it to pick the emotion you want, and the colour will automatically match.

You can tweak the look in the Inspector. The script lets you set things like:

- `Base Colours`

- `Pulse Foreground Gradient`

- `Pulse Background Gradient`

- `Character textures`

So if you want to change how the VFX looks for different emotions, just adjust those in the prefab.


