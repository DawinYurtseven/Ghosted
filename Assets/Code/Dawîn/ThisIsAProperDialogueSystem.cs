using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using Ghosted.Dialogue;
using TMPro;
using UnityEngine;

public class ThisIsAProperDialogueSystem : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private List<DialogueTrigger> triggers;
    [SerializeField] private TextMeshProUGUI _name, _text;
    [SerializeField] private StudioEventEmitter _emitter;
    [SerializeField] private bool ForcedDialogue = false;
    [SerializeField] private GameObject dialogueWindowGameObject;

    private int _index ;
    private DialogueNode[] nodes;

    private void Awake()
    {
        triggers = gameObject.GetComponentsInChildren<DialogueTrigger>().ToList();
        nodes = dialogue.GetAllNodes().ToArray();
    }
    

    public void StartDialogue()
    {
        dialogueWindowGameObject = EmotionSingletonMock.Instance.dialogueWindowGameObject;
        dialogueWindowGameObject.SetActive(true);
        _emitter = EmotionSingletonMock.Instance.dialogueEventEmitter;
        _text = EmotionSingletonMock.Instance.textField;
        _name = EmotionSingletonMock.Instance.nameField;
        
        PlayerInputDisabler.Instance.SwitchInputMapDelayed("Dialogue");
        _index = 0;
        _text.text = nodes[0].text;
        _name.text = nodes[0].speaker;
        _emitter.Stop();
        
        if (!nodes[0].voiceClip.IsNull)
        {
            _emitter.EventReference = nodes[0].voiceClip;
            _emitter.Play();
        }

        TriggerDialogueEnterEvents();
    }

    public bool Next()
    {
        _emitter.Stop();
        TriggerDialogueExitEvents();
        _index++;
        if (_index > dialogue.GetAllNodes().Count-1)
        {
            PlayerInputDisabler.Instance.SwitchInputMapDelayed("Character Control");
            CameraManager.Instance.turnOffAll();
            dialogueWindowGameObject.SetActive(false);
            return false;
        }

        _text.text = nodes[_index].text;
        _name.text = nodes[_index].speaker;
        if (!nodes[_index].voiceClip.IsNull)
        {
            _emitter.EventReference = nodes[_index].voiceClip;
            _emitter.Play();
        }

        TriggerDialogueEnterEvents();
        return true;
    }

    private void TriggerDialogueEnterEvents()
    {
        foreach (string action in nodes[_index].onEnterActions)
        {
            var trigger = triggers.FindAll((x) => x.action == action);
            if (trigger.Count != 0)
            {
                foreach (DialogueTrigger trig in trigger)
                {
                    trig.Trigger();
                }
            }
        }
    }

    private void TriggerDialogueExitEvents()
    {
        foreach (string action in nodes[_index].onExitActions)
        {
            var trigger = triggers.FindAll((x) => x.action == action);
            if (trigger.Count != 0)
            {
                foreach (DialogueTrigger trig in trigger)
                {
                    trig.Trigger();
                }
            }
        }
    }

    public UIInteractionHint uiHint;

    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<CharacterControllerMockup>();
        if (player != null)
        {
            player.SetDialogue(this);
            if (ForcedDialogue)
            {
                StartDialogue();
            }

            if (uiHint) uiHint.Show();
        }
    }

    void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<CharacterControllerMockup>();
        if (player != null)
        {
            player.LeaveDialogue();
            if (uiHint) uiHint.Hide();
        }
    }
}