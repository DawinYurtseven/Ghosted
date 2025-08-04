using System.Collections;
using UnityEngine;

public abstract class EmotionAbstract : TalismanTargetMock
{
    protected Emotion cur_emotion;

    [SerializeField] protected GameObject specialEffect;
    [SerializeField] protected GameObject joyVFX;
    [SerializeField] protected GameObject fearVFX;
    public AudioSource bindingSFX;
    public AudioSource boundSFX;
    
    protected IEnumerator PlayAudioSequentially()
    {
        //Debug.Log("AudioSource");
        if (bindingSFX != null)
        {
            bindingSFX.Play();
            yield return new WaitForSeconds(bindingSFX.clip.length);
        }

        if (boundSFX != null)
        {
            boundSFX.Play();
        }
    }
}
