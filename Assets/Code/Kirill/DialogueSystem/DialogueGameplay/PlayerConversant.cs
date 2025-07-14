using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Ghosted.Dialogue {
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] Transform checkForDialoguefrom;
        Dialogue currentDialogue;
        DialogueEditorNode currentNode = null;
        

        public readonly UnityEvent<Dialogue> OnStartDialogue = new UnityEvent<Dialogue>();
        public readonly UnityEvent<Dialogue> OnEndDialogue = new UnityEvent<Dialogue>();
        public readonly UnityEvent<DialogueEditorNode> OnDialogueNode = new UnityEvent<DialogueEditorNode>();
        
        public Func<bool> IsTextAnimating;
        public Action CompleteTextAnimation;
        
        private Conversant currentConversant;


        [SerializeField] private float interactDistance = 20f;

        private int playerLayer, layerMask;
        [SerializeField] private AIConversant dialogueAIConversant;
        
        void Start()
        {
            
            playerLayer = LayerMask.NameToLayer("Player");
            layerMask = ~(1 << playerLayer);
        }
        private float dialogueStartTime;
        private float dialogueInputDelay = 0.3f;
        public void StartDialogue(Dialogue dialogue)
        {
            
            currentDialogue = dialogue;
            OnStartDialogue.Invoke(currentDialogue);

            Debug.Log(currentDialogue);
            currentNode = currentDialogue.GetRootNode();
            OnDialogueNode.Invoke(currentNode);
            dialogueStartTime = Time.time;
            
            PlayerInputDisabler.Instance.SwitchInputMap("Dialogue");
            TriggerEnterAction();
            //Somehow is still laggy for the first dialogue in the scene
            //StartCoroutine(SwitchInputMapNextFrame("Dialogue"));
        }

        public void EndDialogue()
        {

            OnEndDialogue.Invoke(currentDialogue);
            currentDialogue = null;
            currentConversant = null;
            currentNode = null;
            LeaveDialogue();
            
            
            PlayerInputDisabler.Instance.SwitchInputMapDelayed("Character Control");
        }
        
        //called from Input asset 
        //
        // public void OnNextClick(InputAction.CallbackContext context)
        // {
        //     Debug.Log("I clicked");
        //     if (currentNode as ReplyNode != null)
        //         return;
        //
        //     Next();
        // }
        
        public void SetDialogue(AIConversant conversant)
        {
            if (dialogueAIConversant != null)
            {
                dialogueAIConversant.turnOffHint();
            }
            dialogueAIConversant = conversant;
            dialogueAIConversant.turnOnHint();
        }
        public void LeaveDialogue()
        {
            if (dialogueAIConversant != null)
            {
                dialogueAIConversant.turnOffHint();
                dialogueAIConversant = null;
            }
        }

        // This is a moch function that has to be replaces with uniform interaction system
        public void InteractDialogue()
        {
            Debug.Log("I try to interact for dialogue");
            if (currentDialogue == null &&  dialogueAIConversant != null)
            {
                Debug.Log("It's a conversant!");
                currentConversant = dialogueAIConversant;
                dialogueAIConversant.Interact(this);

            // if (currentDialogue == null)
            // {
            //     //Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()); // Use mouse position
            //     // Ray ray = new Ray(transform.position, transform.forward);
            //     
            //     Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            //     RaycastHit hit;
            //
            //     Debug.Log("I shoot ray " + ray);
            //
            //     if (Physics.Raycast(ray, out hit, interactDistance, layerMask))
            //     {
            //         Debug.Log("I hit smth " + hit.collider.gameObject.name);
            //         // Check for the target script
            //         AIConversant aIConversant = hit.collider.GetComponent<AIConversant>();
            //         if (aIConversant != null)
            //         {
            //             Debug.Log("It's a conversant!");
            //             currentConversant = aIConversant;
            //             aIConversant.Interact(this);
            //             
            //         }
            //     }
            // }
            
                    
            }
        }

        void Update()
        {
            
            // Ray ray = new Ray(checkForDialoguefrom.position, checkForDialoguefrom.transform.forward);
            // RaycastHit hit;
            // if (Physics.Raycast(ray, out hit, interactDistance, layerMask))
            // {
            //     AIConversant aIConversant = hit.collider.GetComponent<AIConversant>();
            //     if (aIConversant )
            //     {
            //         if (aIConversant != dialogueAIConversant)
            //         {
            //             Debug.Log("Found new dialogue!");
            //             if (dialogueAIConversant != null)
            //             {
            //                 dialogueAIConversant.turnOffHint();
            //             }
            //             dialogueAIConversant = aIConversant;
            //             dialogueAIConversant.turnOnHint();
            //         }
            //
            //         return;
            //     }
            // }
            //
            // if (dialogueAIConversant != null) 
            // {
            //     dialogueAIConversant.turnOffHint();
            //     
            //      dialogueAIConversant = null;
            // }
        }

        public void EnteredConversantArea(AreaConversant areaConversant)
        {
            currentConversant = areaConversant;
        }

        public void SetGlobalConversant(GlobalConversant globalConversant)
        {
            currentConversant = globalConversant;
        }

        // Next for DialogeNodes, Select for replies in replyNode
        public void Next(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (!currentDialogue) return;
                //delay so that the start dialogue e is not confused with the next line
                if (Time.time - dialogueStartTime < dialogueInputDelay) return;
//                Debug.Log("Went through input delay");
                if (IsTextAnimating != null && IsTextAnimating.Invoke())
                {
                    CompleteTextAnimation?.Invoke();
                    return;
                }
                
                TriggerExitAction();
                
                //TODO: maybe let the dialogue complete first (if revealing)
                currentNode = currentDialogue.GetChild((DialogueNode)currentNode);
                if (currentNode == null)
                {
                    EndDialogue();
                }
                else
                {
                    OnDialogueNode.Invoke(currentNode);
                    TriggerEnterAction();
                }
            }

        }

        public void Select(Reply reply)
        {
            currentNode = currentDialogue.GetNodeFromId(reply.Child);
            if (currentNode == null)
            {
                EndDialogue();
            }
            else
            {
                OnDialogueNode.Invoke(currentNode);
                TriggerEnterAction();
            }
        }

        private void TriggerEnterAction()
        {
            if (currentNode != null)
            {
                foreach (string action in currentNode.onEnterActions)
                {
                    TriggerAction(action);
                }
            }
        }
        private void TriggerExitAction()
        {
            if (currentNode != null)
            {
                foreach (string action in currentNode.onExitActions)
                {
                    TriggerAction(action);
                }
            }
        }

        private void TriggerAction(string action)
        {
            if (action == "") return;

            if (currentConversant == null)
            {
                Debug.Log("No conversant!");
                return;
            }
            foreach (DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }

        public DialogueNode GetReply(string id)
        {
            return currentDialogue.GetDialogueNodeFromId(id);
        }

        public DialogueEditorNode GetCurrentNode()
        {
            return currentNode;
        }

       
        
    }
}

