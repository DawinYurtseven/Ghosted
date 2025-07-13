using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueWIndowLayoutData : ScriptableObject
{
    [Range(0, 100)] public float widthPercent;
    [Range(0, 100)] public float heightPercent;
    [Range(0, 100)] public float leftPercent;
    [Range(0, 100)] public float topPercent;
}
