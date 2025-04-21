using UnityEngine;

public class Deadzone : MonoBehaviour
{
    public GameObject playerObj;
    public bool loop = false;
    public Transform resetPoint;
    public float threshold = -10;
    public float resetAmount = 20;
    // Update is called once per frame
    void Update()
    {
        if (playerObj.transform.position.y < threshold)
        {
            if(loop)
            {
                playerObj.transform.position += new Vector3(0, resetAmount, 0);
            }
            else
            {
                playerObj.transform.position = resetPoint.position;
            }
        }
    }
}
