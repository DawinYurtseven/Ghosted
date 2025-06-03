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
    // Update is called once per frame
    void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var vfx = Instantiate(vfxPrefab).GetComponent<BoundVFXController>();
            vfx.PlayVFXAt(target, emotions);
        }

    }
}
