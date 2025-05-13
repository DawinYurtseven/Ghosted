using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BindingMock : MonoBehaviour
{
    // Start is called before the first frame update
     private CharacterControllerMockup _controller;

     public List<GameObject> gameObjectsList = new List<GameObject>();

     public int maxTalismans = 3;
     [SerializeField] TextMeshProUGUI talismansUsed;
    
    private void Awake()
    {
        _controller = this.GetComponent<CharacterControllerMockup>();

        if (_controller == null) {
            Debug.Log("No controller for binding");
            this.enabled = false;
            
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) {

            Debug.Log("Got key");

            if (_controller.target != null) {

                GameObject lockObject = _controller.target.gameObject;


                if (lockObject.GetComponent<Lockable>() != null) {
                    if (gameObjectsList.Contains(lockObject)) {
                        gameObjectsList.Remove(lockObject);
                        lockObject.GetComponent<Lockable>().Unlock();
                    }
                    else {
                         if (gameObjectsList.Count == maxTalismans) {
                            Debug.Log("All talismans used!");
                            return; } 
                         gameObjectsList.Add(lockObject); 
                         lockObject.GetComponent<Lockable>().Lock();
                    }
                }
            }
            talismansUsed.text = "Talismans used: " + gameObjectsList.Count + " / " + maxTalismans;
                
        } 
        //Recall talismans
        if (Input.GetKeyDown(KeyCode.R)) {
             foreach (GameObject go in gameObjectsList)
             {
                 go.GetComponent<Lockable>().Unlock();
             }
             
             gameObjectsList.Clear();
             
             talismansUsed.text = "Talismans used: " + gameObjectsList.Count + " / " + maxTalismans;
        }
    }

       
}

    

