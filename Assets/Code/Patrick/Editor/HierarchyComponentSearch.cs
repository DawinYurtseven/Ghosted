using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[InitializeOnLoad]
public class HierarchyComponentSearch : MonoBehaviour
{
    private static string searchQuery = "";
    private static bool searchComponents = true;

    static HierarchyComponentSearch()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        EditorApplication.update += OnUpdate;
    }

    private static void OnUpdate()
    {
        // Read current hierarchy search string (from Unity’s built-in search)
        string hierarchySearch = GetHierarchySearchString();

        if (hierarchySearch != searchQuery)
        {
            searchQuery = hierarchySearch;
            EditorApplication.RepaintHierarchyWindow();
        }
    }

    private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        if (string.IsNullOrEmpty(searchQuery) || !searchComponents)
            return;

        GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (go == null) return;

        // If Unity already matched the object name, skip extra check
        if (go.name.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
            return;

        // Check if any component name matches search
        bool hasMatch = go.GetComponents<Component>()
            .Any(c => c != null && c.GetType().Name.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0);

        if (!hasMatch)
        {
            // Hide the row by drawing over it
            EditorGUI.DrawRect(selectionRect, new Color(0, 0, 0, 0.1f));
        }
    }

    private static string GetHierarchySearchString()
    {
        // Unity doesn’t expose hierarchy search directly, so we query the SceneHierarchyWindow’s search field
        var sceneHierarchyType = typeof(Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
        var windows = Resources.FindObjectsOfTypeAll(sceneHierarchyType);
        if (windows.Length > 0)
        {
            var searchFieldInfo = sceneHierarchyType.GetProperty("searchFilter", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            if (searchFieldInfo != null)
            {
                return searchFieldInfo.GetValue(windows[0], null) as string;
            }
        }
        return "";
    }

    [MenuItem("Tools/Hierarchy Search/Toggle Component Search")]
    private static void ToggleComponentSearch()
    {
        searchComponents = !searchComponents;
        Debug.Log("Hierarchy Component Search: " + (searchComponents ? "Enabled" : "Disabled"));
        EditorApplication.RepaintHierarchyWindow();
    }
}
