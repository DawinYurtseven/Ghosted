using UnityEngine;

[CreateAssetMenu(fileName = "InputIconMappings", menuName = "Input/IconMappings")]
public class InputIconMappings : ScriptableObject
{
    [System.Serializable]
    public class IconMapping
    {
        public string actionName;
        public string keyboardSpriteName;
        public string gamepadSpriteName;
    }

    public IconMapping[] mappings;

    public string GetSpriteName(string action, string controlScheme)
    {
        foreach (var mapping in mappings)
        {
            if (mapping.actionName == action)
            {
                return (controlScheme ==  "Gamepad" || controlScheme == "Controller" ) ? mapping.gamepadSpriteName : mapping.keyboardSpriteName;
            }
        }
        
        return "MissingIcon";
    }
    
    
}