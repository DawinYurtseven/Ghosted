using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogues/Layout Data")]
public class DialogueMenuLayoutData : ScriptableObject
{
    [Range(0, 100)] public float widthPercent;
    [Range(0, 100)] public float heightPercent;
    [Range(0, 100)] public float leftPercent;
    [Range(0, 100)] public float topPercent;
}
