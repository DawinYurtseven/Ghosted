using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ghosted.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ThisIsAProperDialogueSystem : MonoBehaviour
{
    [SerializeField] private Dialogue fialogue;
    [SerializeField] private List<DialogueTrigger> triggers;
    private TextMeshPro _name, _text;

    public readonly UnityEvent<Dialogue> OnStartDialogue = new UnityEvent<Dialogue>();
    public readonly UnityEvent<Dialogue> OnEndDialogue = new UnityEvent<Dialogue>();
    
    private int _index = 0;
    private DialogueNode[] nodes;

    private void Awake()
    {
        triggers = gameObject.GetComponentsInChildren<DialogueTrigger>().ToList();
        nodes = fialogue.GetAllNodes().ToArray();
    }

    public void StartDialogue()
    {
        PlayerInputDisabler.Instance.SwitchInputMapDelayed("Dialogue");
        _index = 0;
        _text.text = nodes[0].text;
        _name.text = nodes[0].speaker;
        FMODUnity.RuntimeManager.PlayOneShot(nodes[0].voiceClip);
        
        TriggerDialogueEnterEvents();
        
        
    }
    
    public void Next(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            
            TriggerDialogueExitEvents();
            _index++;
            if (_index >= triggers.Count)
            {
                PlayerInputDisabler.Instance.SwitchInputMapDelayed("Character Control");
                return;
            }
            _text.text = nodes[_index].text;
            _name.text = nodes[_index].speaker;
            FMODUnity.RuntimeManager.PlayOneShot(nodes[_index].voiceClip);
            
            TriggerDialogueEnterEvents();
            
        }

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

    public void turnOnHint()
    {
        if (uiHint != null)
        {
            Debug.Log("Turn On Hint");
            uiHint.Show();
        }
    }

    public void turnOffHint()
    {
        if (uiHint != null)
        {
            uiHint.Hide();
        }
    }
}