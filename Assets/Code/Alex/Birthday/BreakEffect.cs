using UnityEngine;

public class BreakEffect : MonoBehaviour
{
    public ParticleSystem breakEffect;

    public void PlayBreakEffect()
    {
        if (breakEffect == null)
        {
            Debug.LogWarning("No particle system assigned!");
            return;
        }

        if (!breakEffect.isPlaying)
        {
            breakEffect.Play();
        }
    }
}