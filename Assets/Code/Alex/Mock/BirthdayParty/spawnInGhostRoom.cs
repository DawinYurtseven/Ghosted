using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnInGhostRoom : MonoBehaviour
{

    public GameObject[] toActivateAgain; 
    public GameObject spawnPoint;
    

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<CharacterControllerMockup>()!= null) {

            foreach (GameObject objectHidden in toActivateAgain  ) {
                objectHidden.SetActive(true);
            }
            other.gameObject.transform.position = spawnPoint.transform.position;
        }
        Destroy(gameObject);
    }
}
