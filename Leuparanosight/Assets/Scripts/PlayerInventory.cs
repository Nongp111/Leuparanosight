using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private HashSet<string> keys = new HashSet<string>();

    public void AddKey(string keyName)
    {
        if (!keys.Contains(keyName))
        {
            keys.Add(keyName);
            Debug.Log("Picked up key: " + keyName);
        }
    }

    public bool HasKey(string keyName)
    {
        return keys.Contains(keyName);
    }

}
