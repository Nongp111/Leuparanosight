using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedkitPickup : MonoBehaviour
{
    public int amount = 1; // เก็บได้กี่ชิ้น

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.medkitCount += amount;
                Debug.Log("Picked up Medkit! Total: " + player.medkitCount);
            }

            Destroy(gameObject); // หายไปหลังเก็บ
        }
    }
}
