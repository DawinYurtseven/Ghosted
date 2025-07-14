using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "NewPopUpData", menuName = "UI/PopUpData")]
public class PopUpData : ScriptableObject
{
    public PopUpType type;
    public Sprite icon;
    public string header;
    [TextArea] public string description;
    
}


public enum PopUpType
{
    Collectable,
    Talisman,
    Altar
}

