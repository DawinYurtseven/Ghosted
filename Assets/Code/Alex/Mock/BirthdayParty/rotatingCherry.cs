using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class rotatingCherry : MonoBehaviour
{
    public float duration = 3f;

    private void Start()
    {

        // Continuous rotation using DOTween
        transform.DORotate(new Vector3(0, 360f, 0), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1);
    }
}
