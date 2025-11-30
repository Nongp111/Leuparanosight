using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamageOverlay : MonoBehaviour
{
    public Player player;       // อ้างอิง Player
    public Image damageOverlay; // UI Image สีแดงเต็มจอ
    public float maxAlpha = 0.7f;  // ความเข้มสูงสุด
    public float smoothSpeed = 5f; // ความเร็วในการเปลี่ยน

    void Update()
    {
        if (player == null || damageOverlay == null) return;

        // คำนวณสัดส่วนเลือดที่เหลือ
        float healthPercent = player.health / player.maxHealth;

        // ยิ่งเลือดน้อย → ยิ่งแดง
        float targetAlpha = Mathf.Lerp(maxAlpha, 0f, healthPercent);

        // ค่อย ๆ ไล่สีแบบ smooth
        Color c = damageOverlay.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * smoothSpeed);
        damageOverlay.color = c;
    }
}
