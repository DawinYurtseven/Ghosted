using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDown : MonoBehaviour
{
    public GameObject[] gameObjectstoDelete;
    

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<CharacterControllerMockup>()!= null) {
            foreach (GameObject objectToDelete in gameObjectstoDelete  ) {
                Destroy(objectToDelete);
            }
        }
        Destroy(gameObject);
    }
}
