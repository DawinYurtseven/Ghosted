using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Ghosted.Dialogue {
    public class PlayerConversant : MonoBehaviour
    {
        Dialogue currentDialogue;
        DialogueEditorNode currentNode = null;
        KirillCharacterInteractionInput inputManager;

        public readonly UnityEvent<Dialogue> OnStartDialogue = new UnityEvent<Dialogue>();
        public readonly UnityEvent<Dialogue> OnEndDialogue = new UnityEvent<Dialogue>();
        public readonly UnityEvent<DialogueEditorNode> OnDialogueNode = new UnityEvent<DialogueEditorNode>();

        private Conversant currentConversant;


        [SerializeField] private float interactDistance = 20f;

        private int playerLayer, layerMask;
        void Awake()
        {

        }

        void Start()
        {
            playerLayer = LayerMask.NameToLayer("Player");
            layerMask = ~(1 << playerLayer);
            inputManager = GameObject.FindGameObjectWithTag("ScriptHolder")?.GetComponent<KirillCharacterInteractionInput>();
            inputManager.SubscribeInteract(OnInteract);
        }

        public void StartDialogue(Dialogue dialogue)
        {
            inputManager.UnsubscribeInteract(OnInteract);
            inputManager.SubscribePressedNext(OnNextClick);

            currentDialogue = dialogue;
            OnStartDialogue.Invoke(currentDialogue);

            Debug.Log(currentDialogue);
            currentNode = currentDialogue.GetRootNode();
            OnDialogueNode.Invoke(currentNode);
            TriggerEnterAction();
        }

        public void EndDialogue()
        {
            inputManager.UnsubscribePressedNext(OnNextClick);
            inputManager.SubscribeInteract(OnInteract);

            OnEndDialogue.Invoke(currentDialogue);
            currentDialogue = null;
            currentConversant = null;
            currentNode = null;
        }

        public void OnNextClick(InputAction.CallbackContext context)
        {
            Debug.Log("I clicked");
            if (currentNode as ReplyNode != null)
                return;

            Next();
        }

        // This is a moch function that has to be replaces with uniform interaction system
        private void OnInteract(InputAction.CallbackContext context)
        {
            Debug.Log("I try to interact");
            if (currentDialogue == null)
            {
                //Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()); // Use mouse position
                // Ray ray = new Ray(transform.position, transform.forward);
                
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                RaycastHit hit;

                Debug.Log("I shoot ray " + ray);

                if (Physics.Raycast(ray, out hit, interactDistance, layerMask))
                {
                    Debug.Log("I hit smth " + hit.collider.gameObject.name);
                    // Check for the target script
                    AIConversant aIConversant = hit.collider.GetComponent<AIConversant>();
                    if (aIConversant != null)
                    {
                        Debug.Log("It's a conversant!");
                        currentConversant = aIConversant;
                        aIConversant.Interact(this);
                        
                    }
                }
            }
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
        public void Next()
        {
            TriggerExitAction();
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

