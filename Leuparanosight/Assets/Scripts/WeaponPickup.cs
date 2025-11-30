using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public int weaponIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WeaponManager wm = other.GetComponentInChildren<WeaponManager>();

            if (wm != null)
            {
                wm.PickupWeapon(weaponIndex);  // ⭐ เล่นอนิเมชันหยิบปืน
                Debug.Log("Picked up weapon index: " + weaponIndex);
            }

            Destroy(gameObject); // ⭐ เก็บแล้วหาย
        }
    }
}
