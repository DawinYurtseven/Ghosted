using System.Collections;
using UnityEngine;

public abstract class EmotionAbstract : TalismanTargetMock
{
    protected Emotion cur_emotion;

    [SerializeField] protected GameObject specialEffect;
    [SerializeField] protected GameObject joyVFX;
    [SerializeField] protected GameObject fearVFX;
    public FMODUnity.StudioEventEmitter bindingSFX;
    public FMODUnity.StudioEventEmitter boundSFX;
    
    protected IEnumerator PlayAudioSequentially()
    {
        //Debug.Log("AudioSource");
        if (bindingSFX != null)
        {
            bindingSFX.Play();
            
            while (bindingSFX.IsPlaying())
            {
                yield return null;
            }
        }

        if (boundSFX != null)
        {
            boundSFX.Play();
        }
    }
}
