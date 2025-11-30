using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [Header("References")]
    public Camera uiCamera;
    public Revolver revolver;
    public Sway sway;
    public RectTransform canvasRect;
    public WeaponManager weaponManager;
    public CharacterController playerController;

    [Header("Crosshair UI")]
    public RectTransform left;
    public RectTransform right;
    public RectTransform top;
    public RectTransform bottom;
    public RectTransform centerDot;

    [Header("Base Behavior")]
    public float baseGap = 20f;           // ระยะห่างพื้นฐาน
    public float spreadToPixels = 1000f;  // คูณ spread จากปืน
    public float swayToPixels = 200f;     // คูณ sway
    public float smoothSpeed = 10f;       // ความนุ่มนวลของการเคลื่อนไหว

    [Header("Dynamic Behavior")]
    public float moveExpand = 15f;        // ขยายเมื่อเดิน
    public float runExpand = 25f;         // ขยายเมื่อวิ่ง
    public float shootExpand = 20f;       // ขยายตอนยิง
    public float speedThreshold = 0.1f;   // ความเร็วขั้นต่ำที่ถือว่า “เดิน”
    public float runThreshold = 5f;       // ความเร็วที่ถือว่า “วิ่ง”

    [Header("Breathing Effect")]
    public float breathingAmplitude = 3f; // ระยะหายใจ
    public float breathingSpeed = 1.5f;   // ความเร็วหายใจ

    // internal targets
    Vector2 targetLeftPos, targetRightPos, targetTopPos, targetBottomPos, targetCenterPos;

    // สำหรับตรวจยิง
    private bool isShooting = false;
    private float shootExpandTimer = 0f;
    public float shootExpandDuration = 0.2f; // ระยะเวลาขยายตอนยิง
    private float currentMoveExpand = 0f;

    void Start()
    {
        if (revolver == null)
            revolver = GetComponentInParent<Revolver>();

        if (sway == null)
            sway = GetComponentInParent<Sway>();

        if (left == null || right == null || top == null || bottom == null)
            Debug.LogWarning("Crosshair: assign UI parts (left, right, top, bottom) in inspector.");

        targetLeftPos = left != null ? left.anchoredPosition : Vector2.zero;
        targetRightPos = right != null ? right.anchoredPosition : Vector2.zero;
        targetTopPos = top != null ? top.anchoredPosition : Vector2.zero;
        targetBottomPos = bottom != null ? bottom.anchoredPosition : Vector2.zero;
        targetCenterPos = centerDot != null ? centerDot.anchoredPosition : Vector2.zero;
    }

    void Update()
    {
        // 🔹 ตรวจอาวุธปัจจุบัน
        if (weaponManager != null)
        {
            int index = weaponManager.GetSelectedWeaponIndex();
            if (index < 0 || index >= weaponManager.weapons.Length) return;

            GameObject selectedWeapon = weaponManager.weapons[index];
            bool isGun = selectedWeapon.GetComponentInChildren<Revolver>() != null;
            SetCrosshairVisible(isGun);

            if (!isGun)
                return;

            revolver = selectedWeapon.GetComponentInChildren<Revolver>();
        }

        // 🔹 คำนวณ spread จากปืน
        float spread = (revolver != null) ? revolver.currentSpread : 0f;
        float spreadPixels = baseGap + spread * spreadToPixels;

        // 🔹 ตรวจ sway
        Vector3 swayOffsetLocal = (sway != null) ? sway.CurrentOffset : Vector3.zero;
        Vector2 swayPixels = new Vector2(swayOffsetLocal.x * swayToPixels, swayOffsetLocal.y * swayToPixels);

        // 🔹 ตรวจเล็ง
        bool aiming = revolver != null ? revolver.IsAiming() : false;
        float aimMultiplier = aiming ? 0.25f : 1f;

        // 🔹 ตรวจการเคลื่อนไหวแบบ smooth
        float targetMoveExpand = 0f;

        if (playerController != null)
        {
            float speed = playerController.velocity.magnitude;

            // เดิน
            if (speed > speedThreshold && speed < runThreshold)
                targetMoveExpand = moveExpand;

            // วิ่ง
            else if (speed >= runThreshold)
                targetMoveExpand = runExpand;
        }
        
        currentMoveExpand = Mathf.Lerp(currentMoveExpand, targetMoveExpand, Time.deltaTime * 5f);

        // ใช้ค่าที่ลื่นแทนค่าคงที่
        float moveExpandValue = currentMoveExpand;

        // 🔹 ตรวจยิง (Input หรือเช็คจาก revolver ก็ได
        if (Input.GetMouseButtonDown(0))
        {
            isShooting = true;
            shootExpandTimer = shootExpandDuration;
        }

        if (isShooting)
        {
            moveExpandValue += shootExpand;
            shootExpandTimer -= Time.deltaTime;
            if (shootExpandTimer <= 0f)
                isShooting = false;
        }

        // 🔹 เพิ่มเอฟเฟกต์หายใจ
        float breathing = Mathf.Sin(Time.time * breathingSpeed) * breathingAmplitude;

        // 🔹 รวมค่า crosshair gap ทั้งหมด
        float finalGap = spreadPixels * aimMultiplier + moveExpandValue + breathing;

        // 🔹 ตั้งเป้าหมายตำแหน่ง
        targetLeftPos = new Vector2(-finalGap, 0) + swayPixels;
        targetRightPos = new Vector2(finalGap, 0) + swayPixels;
        targetTopPos = new Vector2(0, finalGap) + swayPixels;
        targetBottomPos = new Vector2(0, -finalGap) + swayPixels;
        targetCenterPos = swayPixels;

        // 🔹 เคลื่อนไหว crosshair อย่างนุ่มนวล
        if (left) left.anchoredPosition = Vector2.Lerp(left.anchoredPosition, targetLeftPos, Time.deltaTime * smoothSpeed);
        if (right) right.anchoredPosition = Vector2.Lerp(right.anchoredPosition, targetRightPos, Time.deltaTime * smoothSpeed);
        if (top) top.anchoredPosition = Vector2.Lerp(top.anchoredPosition, targetTopPos, Time.deltaTime * smoothSpeed);
        if (bottom) bottom.anchoredPosition = Vector2.Lerp(bottom.anchoredPosition, targetBottomPos, Time.deltaTime * smoothSpeed);
        if (centerDot) centerDot.anchoredPosition = Vector2.Lerp(centerDot.anchoredPosition, targetCenterPos, Time.deltaTime * smoothSpeed);

        // 🔹 ปรับขนาดจุดกลางเวลาเล็ง
        if (centerDot)
        {
            float targetScale = aiming ? 0.6f : 1f;
            centerDot.localScale = Vector3.Lerp(centerDot.localScale, Vector3.one * targetScale, Time.deltaTime * smoothSpeed);
        }
    }

    void SetCrosshairVisible(bool visible)
    {
        if (left) left.gameObject.SetActive(visible);
        if (right) right.gameObject.SetActive(visible);
        if (top) top.gameObject.SetActive(visible);
        if (bottom) bottom.gameObject.SetActive(visible);
        if (centerDot) centerDot.gameObject.SetActive(visible);
    }
}