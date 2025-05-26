using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDown : MonoBehaviour
{
    public GameObject[] activeObjects;
    public GameObject[] disabledObjects;
    

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<CharacterControllerMockup>()!= null) {
            foreach (GameObject objectActive in activeObjects  ) {
                objectActive.SetActive(false);
            }

            foreach (GameObject objectHidden in disabledObjects  ) {
                objectHidden.SetActive(true);
            }
            
            EmotionSingletonMock.Instance.disableAll = false;
        }
        Destroy(gameObject);
    }
}
