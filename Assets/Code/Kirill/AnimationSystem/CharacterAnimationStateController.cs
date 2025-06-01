using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationStateController : MonoBehaviour
{
    [SerializeField] Animator animator;

    //---// Sound Effects //---//
    [SerializeField] AudioSource characterAudioSource;
    [SerializeField] AudioClip throwingTalismanAudio;

    //---// Functions To Interact with from Controller //---//
    public void SetWalking(bool isWalking)
    {
        animator.SetBool("isWalking", isWalking);
    }

    public void SetRunning(bool isRunning)
    {
        animator.SetBool("isRunning", isRunning);
    }

    public void Jump()
    {
        animator.SetTrigger("jump");
    }

    public void Land()
    {
        animator.SetTrigger("land");
    }

    public void ThrowTalisman()
    {
        animator.SetTrigger("throwTalisman");
    }

    public void CatchTalisman()
    {
        animator.SetTrigger("catchTalisman");
    }
    //---// Functions To Interact with from Controller //---//

    //---// Functions called from Animator //---//

    // Throw Talisman only after certain frame (Animation Event)
    public void PerformThrowingTalisman()
    {
        characterAudioSource.clip = throwingTalismanAudio;
        characterAudioSource.Play();
    }

    public void PerformCatchingTalisman()
    {
        characterAudioSource.clip = throwingTalismanAudio;
        characterAudioSource.Play();
    }
}
