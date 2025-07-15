public static class HintBuilder
{
    public static string Control(string spriteName)
    {
        return $"<sprite name=\"{spriteName}\">";
    }

    public static string BuildHint(string template, InputIconMappings iconMap, string scheme)
    {
        foreach (var mapping in iconMap.mappings)
        {
            string token = $"[{mapping.actionName}]";
            string sprite = iconMap.GetSpriteName(mapping.actionName, scheme);
            template = template.Replace(token, Control(sprite));
        }
        return template;
    }
}