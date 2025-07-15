using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablesManager : MonoBehaviour
{
    //TODO: we can also use this latet if we want to keep track of coolectables/show it later or so

    private bool collectedFirstColelctable = false;

    public void Collect()
    {
        if (!collectedFirstColelctable)
        {
            PopUpUI.Instance.StartPopUpWithDelay(PopUpType.Collectable);
            collectedFirstColelctable = true;
        }
    }
}
