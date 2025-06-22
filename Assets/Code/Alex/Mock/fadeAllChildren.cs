using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fadeAllChildren : MonoBehaviour
{
    public void SearchAndFade(GameObject target)
    {
        // If this object has a GameObjectChanger component, call DoFade
        ObjectFade changer = target.GetComponent<ObjectFade>();
        // Recursively check all children
        foreach (Transform child in target.transform)
        {
            SearchAndFade(child.gameObject);
        }
        if (changer != null)
        {
            changer.doFading();
        }

        
    }
}
