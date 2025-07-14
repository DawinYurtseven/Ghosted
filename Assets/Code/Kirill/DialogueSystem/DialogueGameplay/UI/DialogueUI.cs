using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ghosted.Dialogue;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Ghosted.UI
{
    public class DialogueUI : MonoBehaviour
    {
        PlayerConversant playerConversant;
        [SerializeField] TextMeshProUGUI messageText;
        [SerializeField] TextMeshProUGUI speakerText;
        [SerializeField] Button exitButton;
        [SerializeField] Transform choiceRoot;
        [SerializeField] GameObject choicePrefab;
        [SerializeField] GameObject replicWindow;
        [SerializeField] GameObject choiceWindow;
        
        [SerializeField] private AudioSource audioSource;

        [SerializeField] private TextFadeReveal textAnimator;

        [SerializeField] private bool charReveal;
        // Start is called before the first frame update
        void Start()
        {
            playerConversant = GameObject.FindGameObjectsWithTag("Player")[0]
                .GetComponent<PlayerConversant>(); //Can go wrong

            if (playerConversant == null)
            {
                Debug.Log("Missing Player Conversant");
                return;
            }
            playerConversant.OnStartDialogue.AddListener(StartDialogue);
            playerConversant.OnEndDialogue.AddListener(EndDialogue);
            playerConversant.OnDialogueNode.AddListener(DisplayNodeInfo);
            playerConversant.IsTextAnimating = () => textAnimator != null && textAnimator.isAnimating;

            playerConversant.CompleteTextAnimation = () =>
            {
                textAnimator?.Complete();
            };
            
            exitButton.onClick.AddListener(OnExitDialogueClick);

            gameObject.SetActive(false);
        }

        public void StartDialogue(Ghosted.Dialogue.Dialogue dialogue)
        {
            gameObject.SetActive(true);
        }

        public void EndDialogue(Ghosted.Dialogue.Dialogue dialogue)
        {
            gameObject.SetActive(false);
        }

        private void OnExitDialogueClick()
        {
            playerConversant.EndDialogue();
        }

        void DisplayNodeInfo(DialogueEditorNode node) {
            UpdateUI(node);
        }

        // Update is called once per frame
        void UpdateUI(DialogueEditorNode node)
        {
            var curNode = node;
            if (curNode == null)
            {
                Debug.LogError("I have null curNode in UI");
            }
            else if (curNode as DialogueNode != null)
            {
                audioSource.Stop();
                DialogueNode dialogueNode = (DialogueNode)curNode;
                if (dialogueNode.voiceClip != null)
                {
                    audioSource.clip = dialogueNode.voiceClip;
                    audioSource.Play();
                }
                replicWindow.SetActive(true);
                choiceWindow.SetActive(false);
                if (charReveal)
                {
                    textAnimator.Reset();
                    textAnimator.animateText(dialogueNode.text);
                }
                else
                {
                    messageText.text = dialogueNode.text;
                }
                
                speakerText.text = dialogueNode.speaker;
            }
            else if (curNode as ReplyNode != null)
            {
                ReplyNode replyNode = (ReplyNode)curNode;
                replicWindow.SetActive(false);
                choiceWindow.SetActive(true);
                foreach (Transform child in choiceRoot)
                {
                    Destroy(child.gameObject);
                }

                foreach (Reply reply in replyNode.replies)
                {
                    var choiceGO = Instantiate(choicePrefab);
                    choiceGO.GetComponentInChildren<TextMeshProUGUI>().text = reply.text;
                    choiceGO.transform.SetParent(choiceRoot);
                    Button btn = choiceGO.GetComponentInChildren<Button>();
                    btn.onClick.AddListener(() =>
                    {
                        playerConversant.Select(reply);
                    });
                }
            }
        }
    }
}
