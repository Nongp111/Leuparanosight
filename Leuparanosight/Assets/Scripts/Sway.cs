using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour
{
    public float swayAmount = 0.02f;
    public float swaySpeed = 3f;
    public float moveSwayAmount = 0.05f;

    private Vector3 initialPos;

    void Start()
    {
        initialPos = transform.localPosition;
    }

    void Update()
    {
        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        float amount = isMoving ? moveSwayAmount : swayAmount;

        float swayX = Mathf.Sin(Time.time * swaySpeed) * amount;
        float swayY = Mathf.Cos(Time.time * swaySpeed * 0.8f) * amount;

        Vector3 offset = new Vector3(swayX, swayY, 0);

        // ✔ ไม่เขียนทับ animation pose
        // ใช้ตำแหน่งจาก Animator ก่อน แล้ว + sway เพิ่มเข้าไป
        transform.localPosition = initialPos + offset;
    }

    // ให้ script อื่นอ่านค่า offset ได้ (local space offset)
    public Vector3 CurrentOffset
    {
        get
        {
            return transform.localPosition - initialPos;
        }
    }
}
