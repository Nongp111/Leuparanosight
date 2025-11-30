using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ammo : MonoBehaviour
{
    public Revolver revolver;
    public TMP_Text ammoText;
 

    void Update()
    {
        ammoText.text = revolver.currentAmmo + " / " + revolver.reserveAmmo;

     
    }
}
