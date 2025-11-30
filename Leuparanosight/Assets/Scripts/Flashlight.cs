using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public Light flashlight;        // ไฟฉาย (Spotlight)
    public KeyCode toggleKey = KeyCode.F; // ปุ่มกดเปิด/ปิด
    private bool isOn = true;

    void Start()
    {
        if (flashlight != null)
            flashlight.enabled = isOn;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isOn = !isOn;
            if (flashlight != null)
                flashlight.enabled = isOn;
        }
    }
}
