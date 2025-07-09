using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Ghosted.Dialogue;
using DG.Tweening;
using TMPro;
using UnityEngine.TextCore.Text;

[System.Serializable]
public class DialogueWindowSettings
{
    public FontAsset fontAsset;
    public float charDelay;
}
[ExecuteAlways]
public class DialogueWindowController : MonoBehaviour
{
    [SerializeField] private DialogueWindowSettings joySettings;
    [SerializeField] private DialogueWindowSettings fearSettings;
    public UIDocument uiDocument;
    private VisualElement root;

    [Range(0, 100)]
    public float widthPercent = 0f;

    [Range(0, 100)]
    public float heightPercent = 0f;

    [Range(0, 100)]
    public float leftOffsetPercent = 0f;
    [Range(0, 100)]
    public float topOffsetPercent = 0f;

    private VisualElement dialogueWindow;
    private Label messageLabel;
    private Label speakerLabel;
    private bool isDoingTransition = false;

    //[SerializeField] private DialogueWIndowLayoutData orbLayout;

    //---// Visuals depending on Emotions //---//
    [SerializeField] private Emotion curEmotion;
    private float charDelay;

    PlayerConversant playerConversant;

    void OnEnable()
    {
        if (uiDocument == null) return;

        root = uiDocument.rootVisualElement;
        dialogueWindow = root.Q<VisualElement>("DialogueWindow");

        messageLabel = dialogueWindow.Q<Label>("Text");
        speakerLabel = dialogueWindow.Q<Label>("Conversant");

        ApplyOffset();
    }

    void Start()
    {
        if (!Application.isPlaying) return;

        playerConversant = GameObject.FindGameObjectsWithTag("Player")[0]
            .GetComponent<PlayerConversant>(); //Can go wrong

        if (EmotionSingletonMock.Instance == null)
            Debug.LogWarning("I couldn't attacl the function to Emotion Singleton Mock");
        else
            EmotionSingletonMock.Instance.emotionChanged.AddListener(AdaptToNewEmotion);
        AdaptToNewEmotion(curEmotion); // Mock

        playerConversant.OnStartDialogue.AddListener(StartDialogue);
        playerConversant.OnEndDialogue.AddListener(EndDialogue);
        playerConversant.OnDialogueNode.AddListener(DisplayNodeInfo);

        gameObject.SetActive(false);
    }

    void OnValidate()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            ApplyOffset(); // Live-Aktualisierung im Edit Mode
        }
#endif
    }

    private void ApplyOffset()
    {
        if (dialogueWindow == null) return;

        dialogueWindow.style.position = Position.Absolute;

        dialogueWindow.style.width = new Length(widthPercent, LengthUnit.Percent);
        dialogueWindow.style.height = new Length(heightPercent, LengthUnit.Percent);
        dialogueWindow.style.left = new Length(leftOffsetPercent, LengthUnit.Percent);
        dialogueWindow.style.top = new Length(topOffsetPercent, LengthUnit.Percent);
    }

    public void ApplyLayout(DialogueWIndowLayoutData layout)
    {
        //float oldWidthPercent = widthPercent;
        //float oldHeightPercent = heightPercent;

        widthPercent = layout.widthPercent;
        heightPercent = layout.heightPercent;
        leftOffsetPercent = layout.leftPercent;
        topOffsetPercent = layout.topPercent;

        ApplyOffset();

        //isDoingTransition = true;

        //PlayDialogueTransition(new Vector2(leftOffsetPercent, topOffsetPercent), oldWidthPercent, oldHeightPercent);
    }

    /*public void PlayDialogueTransition(Vector2 targetPosition, float oldWidthPercent, float oldHeightPercent)
    {
        Sequence seq = DOTween.Sequence();

        // Fade out text
        seq.Append(DOTween.To(() => messageLabel.style.opacity.value, x => messageLabel.style.opacity = x, 0.5f, 0.3f));

        // Shrink and move
        seq.AppendCallback(() =>
        {
            // Convert to relative movement (assuming starting position is anchored)
            Vector2 fromPos = new Vector2(dialogueWindow.resolvedStyle.left, dialogueWindow.resolvedStyle.top);
            Vector2 offset = targetPosition - fromPos;
            //offset = new Vector2((offset.x / 100) * root.resolvedStyle.width, (offset.y / 100) * root.resolvedStyle.height);

            DOTween.To(() => 1f, x =>
            {
                dialogueWindow.style.width = new Length(oldWidthPercent * x + orbLayout.widthPercent * (1 - x), LengthUnit.Percent);
                dialogueWindow.style.height = new Length(oldHeightPercent * x + orbLayout.widthPercent * (1 - x), LengthUnit.Percent);
            }, 0f, 0.4f).SetEase(Ease.InBack).Play();

            DOTween.To(() => 0f, x =>
            {
                dialogueWindow.style.translate = new Translate(offset.x * x, offset.y * x);
            }, 1f, 0.4f).SetEase(Ease.InOutSine).Play();
        });

        // Expand and show new text
        seq.AppendInterval(0.45f); // Slight delay
        seq.AppendCallback(() =>
        {
            DOTween.To(() => 0f, x =>
            {
                dialogueWindow.style.width = new Length(widthPercent * x + orbLayout.widthPercent * (1 - x), LengthUnit.Percent);
                dialogueWindow.style.height = new Length(heightPercent * x + orbLayout.widthPercent * (1 - x), LengthUnit.Percent);
            }, 1f, 0.4f).SetEase(Ease.OutBack).Play();
        });

        seq.Append(DOTween.To(() => messageLabel.style.opacity.value, x => messageLabel.style.opacity = x, 1f, 0.3f));
        seq.AppendCallback(() =>
        {
            isDoingTransition = false;
            Debug.Log("Ended Transition");
        });
    }*/
    private void AdaptToNewEmotion(Emotion emotion)
    {
        switch (emotion)
        {
            case Emotion.Joy:
                ApplyDialogueWindowSettings(joySettings);
                break;
            case Emotion.Fear:
                ApplyDialogueWindowSettings(fearSettings);
                break;
            default:
                Debug.LogError("I dont have this enotion installed in my body. And in dialogue window as well");
                break;
        }
    }
    private void ApplyDialogueWindowSettings(DialogueWindowSettings settings)
    {
        Debug.Log("I am switching to emotion: " + curEmotion);
        messageLabel.style.unityFontDefinition = new StyleFontDefinition(settings.fontAsset);
        speakerLabel.style.unityFontDefinition = new StyleFontDefinition(settings.fontAsset);
        charDelay = settings.charDelay;
    }
    public void StartDialogue(Dialogue dialogue)
    {
        gameObject.SetActive(true);
    }

    public void EndDialogue(Dialogue dialogue)
    {
        gameObject.SetActive(false);
    }

    private void OnExitDialogueClick()
    {
        playerConversant.EndDialogue();
    }

    void DisplayNodeInfo(DialogueEditorNode node)
    {
        UpdateUI(node);
    }

    void UpdateUI(DialogueEditorNode node)
    {
        var curNode = node;

        if (isDoingTransition) // Mock, for now will be executed from ApplyLayout manually
            return;

        if (curNode == null)
        {
            Debug.LogError("I have null curNode in UI");
        }
        else if (curNode as DialogueNode != null)
        {
            StopAllCoroutines();
            DialogueNode dialogueNode = (DialogueNode)curNode;
            speakerLabel.text = dialogueNode.speaker;

            StartCoroutine(TypeText(dialogueNode.text));
        }
        else if (curNode as ReplyNode != null)
        {
            throw new System.NotImplementedException();
        }
    }

    private IEnumerator TypeText(string fullText)
    {
        messageLabel.text = "";

        for (int i = 0; i < fullText.Length; i++)
        {
            messageLabel.text += fullText[i];
            yield return new WaitForSeconds(charDelay);
        }
    }
}
