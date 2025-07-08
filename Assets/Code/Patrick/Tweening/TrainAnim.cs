using UnityEngine;
using UnityEngine.Splines;

public class TrainAnim : MonoBehaviour
{
    public GameObject train;
    public SplineAnimate train2Anim;
    public float trainDelay = 0.0f; // Delay before the train starts moving
    public bool isTrainRunning = false;
    
    // TODO: implement using the spline system
    public void StartTrain(bool deleteOnFinish = false)
    {
        if (train == null || train2Anim == null)
        {
            Debug.LogWarning("Train or SplineAnimate not set!");
            return;
        }

        if (!isTrainRunning)
        {
            isTrainRunning = true;
            train.SetActive(true);
            
            //TODO
            train2Anim.Play();
            train2Anim.Completed += () =>
            {
                isTrainRunning = false;
                if (deleteOnFinish)
                {
                    Destroy(train);
                    Debug.Log("Train finished moving and was destroyed.");
                }
                else
                {
                    Debug.Log("Train finished moving.");
                }
            };
            Debug.Log("Train started moving along the spline.");
        }
        else
        {
            Debug.Log("Train is already running.");
        }
    }
    
}
