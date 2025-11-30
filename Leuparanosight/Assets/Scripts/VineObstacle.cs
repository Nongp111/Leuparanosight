using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineObstacle : MonoBehaviour
{
    [Header("Health Settings")]
    public float health = 30f;              // พลังชีวิตของพุ่มไม้
    public GameObject breakEffectPrefab;    // เอฟเฟกต์แตก (optional)
    public AudioClip breakSound;            // เสียงเวลาแตก
    public float destroyDelay = 0.5f;       // เวลาหลังโดนฟันก่อนหาย

    private bool isDestroyed = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(float amount)
    {
        if (isDestroyed) return;

        health -= amount;
        if (health <= 0f)
        {
            Break();
        }
    }

    void Break()
    {
        isDestroyed = true;

        // เล่นเสียง
        if (audioSource != null && breakSound != null)
            audioSource.PlayOneShot(breakSound);

        // สร้างเอฟเฟกต์ (ใบไม้กระจาย)
        if (breakEffectPrefab != null)
            Instantiate(breakEffectPrefab, transform.position, Quaternion.identity);

        // ปิดการแสดงผล & collider
        GetComponent<Collider>().enabled = false;
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if (mesh != null) mesh.enabled = false;

        // ทำลายหลังจาก delay เล็กน้อย (เผื่อเสียง)
        Destroy(gameObject, destroyDelay);
    }
}
