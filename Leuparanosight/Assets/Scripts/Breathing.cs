using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Breathing : MonoBehaviour
{
    [Header("Focus Settings")]
    public KeyCode focusKey = KeyCode.LeftShift;
    public float focusDuration = 5f;     // เวลากลั้นหายใจได้สูงสุด
    public float cooldown = 3f;          // เวลาพักก่อนใช้ใหม่
    public float swayMultiplier = 0.2f;  // ลดการส่าย (20% ของปกติ)
    public float spreadMultiplier = 0.3f; // ลด spread (30% ของปกติ)

    [Header("Breathing Settings")]
    public float maxBreath = 100f;        // ค่าหลอดเต็ม
    public float breathDrainRate = 20f;   // ลดต่อวินาที
    public float breathRecoverRate = 10f; // เพิ่มต่อวินาที

    [Header("UI")]
    public GameObject breathUI;  // กล่อง UI ทั้งหมด (จะโชว์เฉพาะตอนใช้)
    public Slider breathBar;     // ตัว slider หลอด

    private float currentBreath;
    private bool isFocusing = false;
    private float cooldownTimer;
    private Animator anim;

    // reference ไปที่ระบบอื่น
    private Sway sway;
    private Revolver gun;

    void Start()
    {
        currentBreath = maxBreath;

        if (breathBar != null)
        {
            breathBar.maxValue = maxBreath;
            breathBar.value = currentBreath;
        }

        if (breathUI != null)
            breathUI.SetActive(false);

        sway = GetComponent<Sway>();
        gun = GetComponent<Revolver>();

        anim = GetComponent<Animator>();

    }

    void Update()
    {
        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

        HandleFocus();
        UpdateUI();
    }

    void HandleFocus()
    {
        if (Input.GetKey(focusKey) && cooldownTimer <= 0 && currentBreath > 0f)
        {
            isFocusing = true;

            currentBreath -= breathDrainRate * Time.deltaTime;
            currentBreath = Mathf.Clamp(currentBreath, 0f, maxBreath);

            // ลดอาการส่าย / spread
            if (sway != null) sway.swayAmount = 0.02f * swayMultiplier;
            if (gun != null) gun.baseSpread = 0.01f * spreadMultiplier;

            // >>> เรียกอนิเมชั่นแทนการเล็งแบบเดิม
            if (anim != null)
                anim.SetBool("IsAiming", true);

            if (currentBreath <= 0f)
                StopFocus();
        }
        else
        {
            if (isFocusing) StopFocus();

            currentBreath += breathRecoverRate * Time.deltaTime;
            currentBreath = Mathf.Clamp(currentBreath, 0f, maxBreath);
        }

    }

    void StopFocus()
    {
        isFocusing = false;
        cooldownTimer = cooldown;

        if (sway != null) sway.swayAmount = 0.04f;
        if (gun != null) gun.baseSpread = 0.01f;

        // >>> หยุดอนิเมชั่นเล็ง
        if (anim != null)
            anim.SetBool("IsAiming", false);
    }

    void UpdateUI()
    {
        if (breathBar != null)
            breathBar.value = currentBreath;

        if (breathUI != null)
        {
            // โชว์เฉพาะตอนกดใช้ หรือหลอดยังไม่เต็ม
            bool shouldShow = isFocusing || currentBreath < maxBreath;
            breathUI.SetActive(shouldShow);
        }
    }

    // optional: ตรวจสอบว่าใช้ได้ไหม
    public bool CanUse()
    {
        return currentBreath > 0f;
    }
}
