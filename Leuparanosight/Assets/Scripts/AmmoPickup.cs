using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 12; // จำนวนกระสุนที่เก็บได้

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // หา Revolver ที่ Player ถืออยู่
            Revolver revolver = other.GetComponentInChildren<Revolver>();

            if (revolver != null)
            {
                revolver.reserveAmmo += ammoAmount; // เติมเข้า reserveAmmo โดยตรง
                Debug.Log("Picked up ammo! Reserve: " + revolver.reserveAmmo);
            }

            Destroy(gameObject); // เก็บแล้วให้กล่องหายไป
        }
    }
}
