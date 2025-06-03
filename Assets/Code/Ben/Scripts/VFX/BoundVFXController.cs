using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BoundVFXController : MonoBehaviour
{
    private VisualEffect vfx;
    [Header("Base Colours")][ Space(10)]
    [ColorUsage(true, true)] public Color joyColour;
    [ColorUsage(true, true)] public Color fearColour;
    [ColorUsage(true, true)] public Color lonelinessColour;
    [ColorUsage(true, true)] public Color loveColour;

    [Header("Foreground Gradients")][ Space(10)]
    public Gradient joyFgGradient;
    public Gradient fearFgGradient;
    public Gradient lonelinessFgGradient;
    public Gradient loveFgGradient;

    [Header("Background Gradients")][ Space(10)]
    public Gradient joyBgGradient;
    public Gradient fearBgGradient;
    public Gradient lonelinessBgGradient;
    public Gradient loveBgGradient;

    [Header("Emotion Character")] [Space(10)]
    public Texture2D joyTex2D;
    public Texture2D fearTex2D;
    public Texture2D lonelinessTex2D;
    public Texture2D loveTex2D;

    [Header("Fade In & Out Settings")] [Space(10)]
    [Range(0, 1)] public float clipAmount = 0.4f;
    public float clipSpeed = 1f;
    float clip = 1;

    private void OnEnable()
    {
        vfx = GetComponent<VisualEffect>();
        clip = 1;
    }

    public void PlayVFXAt(Transform parent, Emotion emotion)
    {
        transform.position = parent.position;
        transform.rotation = parent.rotation;

        vfx.SetVector4("BaseColour", GetEmotionColour(emotion));
        vfx.SetGradient("PulseFGGradient", GetEmotionFgGradient(emotion));
        vfx.SetGradient("PulseBGGradient", GetEmotionBgGradient(emotion));
        vfx.SetTexture("CharacterTexture", joyTex2D);
        vfx.SetVector3("VFXMeshScale", parent.transform.lossyScale);

        transform.SetParent(parent);
        vfx.SendEvent("OnPlay");
        clip = 1;
    }

    public void StopVFX()
    {
        throw new NotImplementedException();
    }

    private Color GetEmotionColour(Emotion emotion)
    {
        switch (emotion)
        {
            case Emotion.Joy:
                return joyColour;
            case Emotion.Fear:
                return fearColour;
            case Emotion.Lonely:
                return lonelinessColour;
            case Emotion.Love:
                return loveColour;
            default:
                return joyColour;
        }
    }

    private Gradient GetEmotionFgGradient(Emotion emotion)
    {
        switch (emotion)
        {
            case Emotion.Joy:
                return joyFgGradient;
            case Emotion.Fear:
                return fearFgGradient;
            case Emotion.Lonely:
                return lonelinessFgGradient;
            case Emotion.Love:
                return loveFgGradient;
            default:
                return joyFgGradient;
        }
    }

    private Gradient GetEmotionBgGradient(Emotion emotion)
    {
        switch (emotion)
        {
            case Emotion.Joy:
                return joyBgGradient;
            case Emotion.Fear:
                return fearBgGradient;
            case Emotion.Lonely:
                return lonelinessBgGradient;
            case Emotion.Love:
                return loveBgGradient;
            default:
                return joyBgGradient;
        }
    }

    private Texture2D GetEmotionTex2D(Emotion emotion)
    {
        switch (emotion)
        {
            case Emotion.Joy:
                return joyTex2D;
            case Emotion.Fear:
                return fearTex2D;
            case Emotion.Lonely:
                return lonelinessTex2D;
            case Emotion.Love:
                return loveTex2D;
            default:
                return joyTex2D;
        }
    }
    private void Update()
    {
        clip = Mathf.MoveTowards(clip, clipAmount, Time.deltaTime * clipSpeed);
        vfx.SetFloat("Clip", clip);
    }

}
