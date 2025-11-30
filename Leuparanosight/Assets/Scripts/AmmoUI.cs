using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    public Revolver revolver;   // ปืน
    public GameObject ammoPanel; // ✅ Panel UI กระสุน
    public Text ammoText;        // Text สำหรับแสดงกระสุน

    void Update()
    {
        if (revolver != null && revolver.gameObject.activeInHierarchy)
        {
            ammoPanel.SetActive(true);
            ammoText.text = revolver.currentAmmo + " / " + revolver.reserveAmmo;
        }
        else
        {
            ammoPanel.SetActive(false);
        }
    }
}
