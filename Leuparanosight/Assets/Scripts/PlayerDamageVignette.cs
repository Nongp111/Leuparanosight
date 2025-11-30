using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamageVignette : MonoBehaviour
{
    public Player player;          // อ้างอิง Player
    public Image vignetteOverlay;  // UI Image วงกลมโปร่งตรงกลาง
    public float maxAlpha = 0.8f;  // ความเข้มสูงสุด
    public float fadeOutSpeed = 2f; // ความเร็วที่เลือดจาง (ต่อวินาที)

    private float currentAlpha = 0f;

    void Update()
    {
        if (player == null || vignetteOverlay == null) return;

        // คำนวณสัดส่วนเลือด
        float healthPercent = player.health / player.maxHealth;

        // ยิ่งเลือดน้อย → ยิ่งแดงเข้ม
        float targetAlpha = Mathf.Lerp(maxAlpha, 0f, healthPercent);

        // ค่อย ๆ ลดลงเองตามเวลา (auto heal effect)
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, fadeOutSpeed * Time.deltaTime);

        // เซ็ตค่า alpha
        Color c = vignetteOverlay.color;
        c.a = currentAlpha;
        vignetteOverlay.color = c;
    }

    // ✅ เรียกฟังก์ชันนี้ตอน Player โดนโจมตี เพื่อดันค่าเลือดแดงขึ้นทันที
    public void ShowHitEffect(float intensity = 0.5f)
    {
        currentAlpha = Mathf.Clamp01(currentAlpha + intensity);
    }
}
