using TMPro;
using UnityEngine;

[ExecuteAlways]
public class TextBoard : MonoBehaviour
{
    [TextArea(3, 10)] public string TextToDisplay;

    public TMP_Text textMain;
    public TMP_Text textOther;

    private void Start()
    {
        ApplyText();
    }

    private void OnValidate()
    {
        ApplyText();
    }

    private void ApplyText()
    {
        if (textMain != null)
            textMain.text = TextToDisplay;

        if (textOther != null)
            textOther.text = TextToDisplay;
    }
}