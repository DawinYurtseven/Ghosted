using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ExampleVFXUsage : MonoBehaviour
{
    public GameObject vfxPrefab;
    public Transform target;
    public Emotion emotions;

    private BoundVFXController vfxInstance;

    private void Update()
    {
        // Start VFX when SPACE is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (vfxInstance != null)
                Destroy(vfxInstance.gameObject); // Clean up previous

            var go = Instantiate(vfxPrefab);
            go.SetActive(true); // In case it's inactive in prefab

            vfxInstance = go.GetComponent<BoundVFXController>();
            vfxInstance.PlayVFXAt(target, emotions);
        }

        // End VFX when R is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (vfxInstance != null)
                vfxInstance.StopVFX();
        }
    }

}
