using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add this script to any selectable objects
public class SelectionOverlayController : MonoBehaviour
{
    private int originalLayer;

    void Awake()
    {
        // Save original layer so you can restore it
        originalLayer = gameObject.layer;
    }

    public void SetSelected(bool isSelected)
    {
        gameObject.layer = isSelected ? LayerMask.NameToLayer("Selectable") : originalLayer;
    }
}
