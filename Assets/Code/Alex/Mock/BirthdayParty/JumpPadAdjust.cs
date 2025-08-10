using UnityEngine;

public class JumpPadAdjust : MonoBehaviour
{
    [SerializeField] private Joy JumpPad;
    // Start is called before the first frame update
    void Start()
    {
        JumpPad = GetComponent<Joy>();
    }


    [SerializeField] private float newStrength;
    public void AdjustStrength()
    {
        JumpPad.force = newStrength;
    }
}
