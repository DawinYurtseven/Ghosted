using System.Collections;
using System.Collections.Generic;
using Ghosted.Dialogue;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

[System.Serializable]
public class DialogueMenuSettings
{
    public FontAsset fontAsset;
    public float charDelay;
}
[ExecuteAlways]
public class DialogueMenuController : MonoBehaviour
{
    [Range(0, 100)]
    public float widthPercent = 0f;

    [Range(0, 100)]
    public float heightPercent = 0f;

    [Range(0, 100)]
    public float leftOffsetPercent = 0f;
    [Range(0, 100)]
    public float topOffsetPercent = 0f;


    [SerializeField] private UIDocument document;
    private VisualElement dialogueMenu;
    private Label messageLabel;
    private Label speakerLabel;
    private float charDelay;

    public bool useCharDelay;
    [SerializeField] private DialogueMenuSettings joySettings;
    [SerializeField] private DialogueMenuSettings fearSettings;

    [SerializeField] private Emotion curEmotion;


    PlayerConversant playerConversant;
    [SerializeField] private FMODUnity.StudioEventEmitter emitter;
    void OnEnable()
    {
        if (document == null) return;

        var root = document.rootVisualElement;
        Debug.Log(root);
        dialogueMenu = root.Q<VisualElement>("DialogueMenu");
        speakerLabel = root.Q<Label>("Speaker");
        messageLabel = root.Q<Label>("Message");

        if (EmotionSingletonMock.Instance == null)
            Debug.LogWarning("I couldn't attach the function to Emotion Singleton Mock");
        else
            EmotionSingletonMock.Instance.emotionChanged.AddListener(AdaptToNewEmotion);

        var players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0)
        {
            Debug.LogWarning("I couldnt find any playerConversant on a scene");
        }
        else
        {
            playerConversant = players[0].GetComponent<PlayerConversant>(); //Can go wrong   
        }
        AdaptToNewEmotion(curEmotion); // Mock
        if (playerConversant != null)
        {
            playerConversant.OnStartDialogue.AddListener(StartDialogue);
            playerConversant.OnEndDialogue.AddListener(EndDialogue);
            playerConversant.OnDialogueNode.AddListener(DisplayNodeInfo);
        }
    }

    void Start()
    {
        if (!Application.isPlaying)
            return;
        dialogueMenu.style.display = DisplayStyle.None;
    }

    void OnValidate()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            ApplyOffset();
        }
#endif
    }

    private void ApplyOffset()
    {
        if (dialogueMenu == null) return;

        dialogueMenu.style.position = Position.Absolute;

        dialogueMenu.style.width = new Length(widthPercent, LengthUnit.Percent);
        dialogueMenu.style.height = new Length(heightPercent, LengthUnit.Percent);
        dialogueMenu.style.left = new Length(leftOffsetPercent, LengthUnit.Percent);
        dialogueMenu.style.top = new Length(topOffsetPercent, LengthUnit.Percent);
    }
    public void ApplyLayout(DialogueMenuLayoutData layout)
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
    private void ApplyMenuSettings(DialogueMenuSettings settings)
    {
        dialogueMenu.style.unityFontDefinition = new StyleFontDefinition(settings.fontAsset);
        charDelay = settings.charDelay;
    }

    private void AdaptToNewEmotion(Emotion emotion)
    {
        this.curEmotion = emotion;
        switch (emotion)
        {
            case Emotion.Joy:
                ApplyMenuSettings(joySettings);
                break;
            case Emotion.Fear:
                ApplyMenuSettings(fearSettings);
                break;
            default:
                Debug.LogError("I dont have this enotion installed in my body. And in dialogue window as well");
                break;
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialogueMenu.style.display = DisplayStyle.Flex;
    }

    public void EndDialogue(Dialogue dialogue)
    {
        emitter.Stop();
        dialogueMenu.style.display = DisplayStyle.None;
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

        if (curNode == null)
        {
            Debug.LogError("I have null curNode in UI");
        }
        else if (curNode as DialogueNode != null)
        {
            emitter.Stop();
            DialogueNode dialogueNode = (DialogueNode)curNode;

            if (!dialogueNode.voiceClip.IsNull)
            {
                emitter.EventReference = dialogueNode.voiceClip;
                emitter.Play();
            }

            speakerLabel.text = dialogueNode.speaker;
            //StartTyping(dialogueNode.text);
            if (useCharDelay)
            {
                StopAllCoroutines();
                StartCoroutine(TypeText(dialogueNode.text));
            }
            else
            {
                messageLabel.text = dialogueNode.text;
            }
        }
        else if (curNode as ReplyNode != null)
        {
            throw new System.NotImplementedException();
        }
    }

    /*public void StartTyping(string fullText)
    {
        float count = 0;
        messageLabelCurrent.text = "";

        DOTween.To(() => count, x =>
        {
            count = x;
            int intCount = Mathf.FloorToInt(count);
            messageLabelCurrent.text = fullText.Substring(0, Mathf.Clamp(intCount, 0, fullText.Length));
        }, fullText.Length, 3).SetEase(Ease.Linear).SetOptions(true).Play();
    }*/

    private IEnumerator TypeText(string fullText)
    {
        var sb = new System.Text.StringBuilder();
        messageLabel.text = "";

        for (int i = 0; i < fullText.Length; i++)
        {
            sb.Append(fullText[i]);
            messageLabel.text = sb.ToString();
            yield return new WaitForSeconds(charDelay);
        }
    }
}
