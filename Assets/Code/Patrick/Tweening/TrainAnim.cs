using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

public class TrainAnim : MonoBehaviour
{
    [Header("Train Animation Settings")]
    public GameObject train;
    public SplineAnimate train2Anim;
    public float trainDelay = 0.0f; // Delay before the train starts moving
    private bool IsTrainRunning = false;

    public SplineAnimate trainBackwards;
    
    [Header("Train Target Settings, independent of SplineAnimate")]
    public Transform trainTarget; // only for moving the train to a target position, with SpawnAnim.moveTo()
    private float moveDuration = 5f;
    //[SerializeField] private SpawnAnim _spawnAnim;
    
    private float wiggleDuration = 1f;
    private float wiggleStrength = 1f;
    
    public void StartTrain(bool deleteOnFinish = false)
    {
        if (train == null || train2Anim == null)
        {
            Debug.LogWarning("Train or SplineAnimate not set!");
            return;
        }

        if (!this.IsTrainRunning)
        {
            this.IsTrainRunning = true;
            train.SetActive(true);
            
            Debug.Log("I am: " + gameObject.name+ " with delay: " + trainDelay);
            
            train2Anim.Play();
            train2Anim.Completed += () =>
            {
                this.IsTrainRunning = false;
                if (deleteOnFinish)
                {
                    Destroy(train);
                    Destroy(train2Anim);
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
            Debug.Log("Train " + gameObject.name + "is already running.");
        }
    }

    // Specialized mehtods for one thing only
    // a method that first wiggles and then starts the train
    public void StartWithWiggle(bool deleteOnFinish = false)
    {
        if (train == null)
        {
            Debug.LogWarning("Train not set!");
            return;
        }
        
        if (!this.IsTrainRunning)
        {
            // wait for the trainDelay before starting the train
            DOVirtual.DelayedCall(trainDelay, animateTrainBreak);   
        }
        else
        {
            Debug.Log("Train " + gameObject.name + "is already running:" + this.IsTrainRunning);
        }
    }

    public void ReverseTrain()
    {
        if (trainBackwards != null && !this.IsTrainRunning)
        {
            this.IsTrainRunning = true;
            trainBackwards.Play();
            trainBackwards.Completed += () =>
            {
                this.IsTrainRunning = false;
                Debug.Log("Train reversed along the spline.");
            };
        }
        else
        {
            Debug.LogWarning("TrainBackwards SplineAnimate not set!");
        }
    }
    
    private void animateTrainBreak()
    {
        train.SetActive(true);
        Sequence s = SpawnAnim.Wiggle(train.transform, wiggleDuration, wiggleStrength);
        
        s.onComplete += () =>
        {
            StartTrain();
        };
        
        // sets the isTrainRunning to true itself
        train2Anim.Completed += () =>
        {
            s = SpawnAnim.Wiggle(train.transform, wiggleDuration, wiggleStrength);
        };
    }
    
    public void moveTrain()
    {
        SpawnAnim.moveTo(train.transform, trainTarget, duration:moveDuration);
    }
}
