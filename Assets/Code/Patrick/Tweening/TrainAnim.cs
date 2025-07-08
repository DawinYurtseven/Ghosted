using UnityEngine;
using UnityEngine.Splines;

public class TrainAnim : MonoBehaviour
{
    public GameObject train;
    public SplineAnimate gleis2follow;
    public float trainSpeed = 1f;
    public float trainDelay = 0.5f; // Delay before the train starts moving
    public bool isTrainRunning = false;
    
    // TODO: implement using the spline system
    public void StartTrain(bool deleteOnFinish = false)
    {
        if (train == null || gleis2follow == null)
        {
            Debug.LogWarning("Train or SplineAnimate not set!");
            return;
        }

        if (!isTrainRunning)
        {
            isTrainRunning = true;
            train.SetActive(true);
            
            //TODO
            //gleis2follow.Play(train, trainSpeed, trainDelay);
            
            Debug.Log("Train started moving along the spline.");
        }
        else
        {
            Debug.Log("Train is already running.");
        }
    }
    
}
