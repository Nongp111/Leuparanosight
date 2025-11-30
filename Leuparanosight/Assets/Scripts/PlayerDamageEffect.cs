using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamageEffect : MonoBehaviour
{
    [Header("UI Overlay")]
    public Image damageOverlay;          // ภาพ UI เลือด (โปร่งใส)
    public float fadeSpeed = 2f;         // ความเร็วในการจาง

    [Header("Camera Shake")]
    public Camera playerCamera;
    public float shakeDuration = 0.2f;
    public float shakeStrength = 0.1f;

    private Color overlayColor;
    private float shakeTimer = 0f;
    private Vector3 originalCamPos;

    void Start()
    {
        if (damageOverlay != null)
        {
            overlayColor = damageOverlay.color;
            overlayColor.a = 0; // เริ่มโปร่งใส
            damageOverlay.color = overlayColor;
        }

        if (playerCamera != null)
        {
            originalCamPos = playerCamera.transform.localPosition;
        }
    }

    void Update()
    {
        // ทำให้เลือดค่อย ๆ จาง
        if (damageOverlay != null && damageOverlay.color.a > 0)
        {
            overlayColor.a = Mathf.Lerp(overlayColor.a, 0, Time.deltaTime * fadeSpeed);
            damageOverlay.color = overlayColor;
        }

        // กล้องสั่น
        if (shakeTimer > 0)
        {
            playerCamera.transform.localPosition = originalCamPos + Random.insideUnitSphere * shakeStrength;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            playerCamera.transform.localPosition = originalCamPos;
        }
    }

    // ✅ เรียกตอน Player โดนโจมตี
    public void ShowDamageEffect()
    {
        if (damageOverlay != null)
        {
            overlayColor.a = 0.7f; // ความเข้มของเลือด
            damageOverlay.color = overlayColor;
        }

        shakeTimer = shakeDuration;
    }
}
