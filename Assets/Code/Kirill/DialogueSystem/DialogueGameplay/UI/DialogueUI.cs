using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ghosted.Dialogue;
using TMPro;
using UnityEngine.UI;

namespace Ghosted.UI
{
    public class DialogueUI : MonoBehaviour
    {
        PlayerConversant playerConversant;
        [SerializeField] TextMeshProUGUI messageText;
        [SerializeField] Button nextButton;
        [SerializeField] Transform choiceRoot;
        [SerializeField] GameObject choicePrefab;
        // Start is called before the first frame update
        void Start()
        {
            playerConversant = GameObject.FindGameObjectsWithTag("Player")[0]
                .GetComponent<PlayerConversant>(); //Can go wrong
            nextButton.onClick.AddListener(Next);

            UpdateUI();
        }
        
        void Next() {
            playerConversant.Next();
            UpdateUI();
        }

        // Update is called once per frame
        void UpdateUI()
        {
            messageText.text = playerConversant.GetText();
            nextButton.gameObject.SetActive(playerConversant.HasNext());
            foreach (Transform child in choiceRoot) {
                Destroy(child.gameObject);
            }
            foreach(string choiceText in playerConversant.GetChoices()) {
                var choiceGO = Instantiate(choicePrefab);
                choiceGO.GetComponentInChildren<TextMeshProUGUI>().text = choiceText;
                choiceGO.transform.parent = choiceRoot;
            }
        }
    }
}
