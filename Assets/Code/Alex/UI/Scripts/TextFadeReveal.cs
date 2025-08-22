using System;
using TMPro;
using DG.Tweening;
using UnityEngine;

public class TextFadeReveal : MonoBehaviour
{
    public TMP_Text tmpText;
    public float characterFadeDuration = 0.3f;
    public float delayBetweenChars = 0.05f;
    public bool isAnimating = false;

    public void Reset()
    {
        tmpText.text = "";
        ResetButLeaveText();
        
    }
    
    public void ResetButLeaveText()
    {
        DOTween.Kill(this);
        StopAllCoroutines();
        isAnimating = false;
    }

    public void showText(string text)
    {
        tmpText.text = text;
    }



    public void Complete()
    {
        DOTween.Kill(this);
        StopAllCoroutines();
        isAnimating = false;
        tmpText.ForceMeshUpdate();

        TMP_TextInfo textInfo = tmpText.textInfo;
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;

            for (int j = 0; j < 4; j++)
            {
                vertexColors[vertexIndex + j].a = 255;
            }
        }
        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
    public void animateText(string text)
    {
        isAnimating = true;
        DOTween.Kill(this);
        StopAllCoroutines();
        //Debug.Log("started coroutine");
        StartCoroutine(RevealTextWithFade(text));
        
    }
    
    public void animateText()
    {
        isAnimating = true;
        DOTween.Kill(this);
        StopAllCoroutines();
        //Debug.Log("started coroutine");
        StartCoroutine(RevealTextWithFade(tmpText.text));
        
    }
    System.Collections.IEnumerator RevealTextWithFade(string fullText)
    {
        
        tmpText.text = fullText;
        tmpText.ForceMeshUpdate();

        TMP_TextInfo textInfo = tmpText.textInfo;
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;

            for (int j = 0; j < 4; j++)
            {
                vertexColors[vertexIndex + j].a = 0;
            }
        }
        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        
        //Using this video as a reference: https://www.youtube.com/watch?v=FXMqUdP3XcE&ab_channel=KembleSoftware
        
        for (int i = 0; i < fullText.Length; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            Color32[] newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            // Fade in using DOTween
            DOTween.To(() => 0f, a =>
            {
                for (int j = 0; j < 4; j++)
                {
                    newVertexColors[vertexIndex + j].a = (byte)(a * 255);
                }
                tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            }, 1f, characterFadeDuration).SetId(this);
            
            //yield return fadeTween.WaitForCompletion();
            yield return new WaitForSeconds(delayBetweenChars);
        }
        
        Debug.Log("stopped Animating " + fullText);
        isAnimating = false;
    }
}
