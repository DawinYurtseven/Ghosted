using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class BufferMenuItems
{
    [MenuItem("Tools/SubMenu/Option")]
    private static void NewNestedOption()
    {
    }

    // Add a new menu item that is accessed by right-clicking on an asset in the project view

    [MenuItem("Assets/Load Additive Scene")]
    private static void LoadAdditiveScene()
    {
        var selected = Selection.activeObject;
        EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(selected));
        //EditorApplication.OpenSceneAdditive(AssetDatabase.GetAssetPath(selected));
    }

    // Adding a new menu item under Assets/Create

    [MenuItem("Assets/Create/Add Configuration")]
    private static void AddConfig()
    {
        // Create and add a new ScriptableObject for storing configuration
    }

    // Add a new menu item that is accessed by right-clicking inside the RigidBody component

    [MenuItem("CONTEXT/Rigidbody/New Option")]
    private static void NewOpenForRigidBody()
    {
    }

    [MenuItem("Assets/ProcessTexture")]
    private static void DoSomethingWithTexture()
    {
    }

    // Note that we pass the same path, and also pass "true" to the second argument.
    [MenuItem("Assets/ProcessTexture", true)]
    private static bool NewMenuOptionValidation()
    {
        // This returns true when the selected object is a Texture2D (the menu item will be disabled otherwise).
        return Selection.activeObject.GetType() == typeof(Texture2D);
    }

    [MenuItem("NewMenu/Option1", false, 1)]
    private static void NewMenuOption()
    {
    }

    [MenuItem("NewMenu/Option2", false, 40)]
    private static void NewMenuOption2()
    {
    }

    [MenuItem("NewMenu/Option3", false, 51)]
    private static void NewMenuOption3()
    {
    }

    
}
