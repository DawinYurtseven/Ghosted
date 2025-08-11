using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using Ghosted.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ThisIsAProperDialogueSystem : MonoBehaviour
{
    [SerializeField] private Dialogue fialogue;
    [SerializeField] private List<DialogueTrigger> triggers;
    [SerializeField] private TextMeshProUGUI _name, _text;
    [SerializeField] private FMODUnity.StudioEventEmitter _emitter;
    
    private int _index = 0;
    private DialogueNode[] nodes;

    private void Awake()
    {
        triggers = gameObject.GetComponentsInChildren<DialogueTrigger>().ToList();
        nodes = fialogue.GetAllNodes().ToArray();
    }
    
    public void SetTexts(TextMeshProUGUI name, TextMeshProUGUI text, StudioEventEmitter emitter)
    {
        _name = name;
        _text = text;
        _emitter = emitter;
    }

    public void StartDialogue()
    {
        PlayerInputDisabler.Instance.SwitchInputMapDelayed("Dialogue");
        _index = 0;
        _text.text = nodes[0].text;
        _name.text = nodes[0].speaker;
        _emitter.EventReference = nodes[0].voiceClip;
        _emitter.Stop();
        _emitter.Play();

        TriggerDialogueEnterEvents();
    }

    public bool Next()
    {
        _emitter.Stop();
         TriggerDialogueExitEvents();
        _index++;
        if (_index > triggers.Count)
        {
            PlayerInputDisabler.Instance.SwitchInputMapDelayed("Character Control");
            _emitter.Stop();
            return false;
        }

        _text.text = nodes[_index].text;
        _name.text = nodes[_index].speaker;
        _emitter.EventReference = nodes[_index].voiceClip;
        _emitter.Play();

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
            if (uiHint)
            {
                uiHint.Show();
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<CharacterControllerMockup>();
        if (player != null)
        {
            player.LeaveDialogue();
            if (uiHint)
            {
                uiHint.Hide();
            }
        }
    }
}