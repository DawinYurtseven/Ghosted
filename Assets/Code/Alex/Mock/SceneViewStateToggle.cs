using System;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class SceneViewStateToggle : EditorWindow
{
    [MenuItem("Project Windows/Scene View State Toggle")]
    public static void ShowExample()
    {
        SceneViewStateToggle wnd = GetWindow<SceneViewStateToggle>();
        wnd.titleContent = new GUIContent("Scene View State Toggle");
    }
    
    static GameObject stateJoy;
    static GameObject stateFear;
    static bool toggleState = false;

    public void CreateGUI()
    {
        Debug.Log("CreateGUI called");
        VisualElement root = rootVisualElement;
        
        Button joyButton = new Button();
        joyButton.name = "Joy";
        joyButton.text = "Joy";
        joyButton.clicked += () => SetEmotion(Emotion.Joy); 
        
        root.Add(joyButton);
        
        Button fearButton = new Button();
        fearButton.name = "Fear";
        fearButton.text = "Fear";
        fearButton.clicked += () =>  SetEmotion(Emotion.Fear);
        root.Add(fearButton);
    }

    void SetEmotion(Emotion emotion)
    {
        EmotionSingletonMock.Instance.ChangeEmotion(emotion);
    }

    private void OnEnable()
    {
        FindObjectsInScene();

    }

    private static bool FindObjectsInScene()
    {
        // Find both active and inactive objects with the specified name
        GameObject[] allObjects = FindObjectsOfType<GameObject>(true); // True to include inactive objects

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "Joy")
            {
                stateJoy = obj;
            }
            else if (obj.name == "Fear")
            {
                stateFear = obj;
            }

            if (stateFear && stateJoy) break;
        }

        if (stateJoy == null)
            Debug.LogWarning("No GameObject named 'Joy' found in the scene.");

        if (stateFear == null)
            Debug.LogWarning("No GameObject named 'Fear' found in the scene.");
        return stateJoy != null && stateFear != null;
    }
}