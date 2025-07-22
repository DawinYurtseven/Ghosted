using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPref_Speed : MonoBehaviour
{
    public float sensitivity = 70.0f;
    public float adjustmentStep = 10.0f;
    private const string SensitivityKey = "sensitivity";

    void Start()
    {
        sensitivity = PlayerPrefs.GetFloat(SensitivityKey, sensitivity);
        Debug.Log("Loaded Sensitivity: " + sensitivity);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            sensitivity += adjustmentStep;
            SaveSensitivity();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            sensitivity = Mathf.Max(0.1f, sensitivity - adjustmentStep); 
            SaveSensitivity();
        }
    }

    void SaveSensitivity()
    {
        PlayerPrefs.SetFloat(SensitivityKey, sensitivity);
        PlayerPrefs.Save();
        Debug.Log("Saved Sensitivity: " + sensitivity);
    }
}
