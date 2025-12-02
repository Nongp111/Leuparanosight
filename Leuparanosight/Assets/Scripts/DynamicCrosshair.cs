using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCrosshair : MonoBehaviour
{
    [Header("Breathing Effect")]
    public float breathingAmplitude = 3f;   // ระยะขยาย/หด
    public float breathingSpeed = 1.5f;     // ความเร็วของจังหวะหายใจ
    public bool isFocusing = false;         // รับค่าจาก BreathingFocus


    public RectTransform top, bottom, left, right;

    [Header("Crosshair Spacing")]
    public float defaultSpacing = 20f;   // ระยะห่างปกติ
    public float moveSpread = 40f;      // ระยะเพิ่มเมื่อเดิน (เพิ่มค่าให้เห็นชัด)
    public float sprintSpread = 70f;     // ระยะเพิ่มเมื่อวิ่ง (ค่าใหม่)
    public float shootSpread = 50f;     // ระยะเพิ่มเมื่อยิง
    public float smoothSpeed = 10f;     // ความเร็วในการปรับ

    private float targetSpacing;
    private float currentSpacing;

    void Start()
    {
        targetSpacing = defaultSpacing;
        currentSpacing = defaultSpacing;
    }

    void Update()
    {
        // ตรวจสอบสถานะต่างๆ
        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        bool isSprinting = isMoving && Input.GetKey(KeyCode.LeftShift); // เช็คว่าวิ่งหรือไม่ (เคลื่อนที่ + กด Shift)
        bool isShooting = Input.GetMouseButton(0);

        // จัดลำดับความสำคัญ: ยิง > วิ่ง > เดิน > หยุดนิ่ง
        if (isShooting)
        {
            targetSpacing = defaultSpacing + shootSpread;
        }
        else if (isSprinting)
        {
            targetSpacing = defaultSpacing + sprintSpread;
        }
        else if (isMoving)
        {
            targetSpacing = defaultSpacing + moveSpread;
        }
        else
        {
            targetSpacing = defaultSpacing;
        }

        // ✅ คำนวณ "ลมหายใจ"
        float amplitude = isFocusing ? breathingAmplitude * 0.2f : breathingAmplitude;
        float breathing = Mathf.Sin(Time.time * breathingSpeed) * amplitude;

        // รวมค่า spread + breathing
        float finalSpacing = targetSpacing + breathing;

        // ค่อย ๆ ปรับให้นุ่มนวล
        currentSpacing = Mathf.Lerp(currentSpacing, finalSpacing, Time.deltaTime * smoothSpeed);

        // อัปเดตตำแหน่ง Crosshair
        top.anchoredPosition = new Vector2(0, currentSpacing);
        bottom.anchoredPosition = new Vector2(0, -currentSpacing);
        left.anchoredPosition = new Vector2(-currentSpacing, 0);
        right.anchoredPosition = new Vector2(currentSpacing, 0);
    }

    public void SetSpread(float spread)
    {
        // ฟังก์ชันนี้อาจถูกเรียกจาก script อื่นและเขียนทับค่า targetSpacing
        // หากปัญหายังคงอยู่หลังจากแก้ไขนี้ เราจะต้องตรวจสอบว่ามี script อื่นเรียกใช้ฟังก์ชันนี้หรือไม่
        targetSpacing = defaultSpacing + spread * 100f;
    }
}
