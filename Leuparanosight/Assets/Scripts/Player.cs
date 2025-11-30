using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour , IPlayerController
{
    public float maxHealth = 100f;
    public float health;

    [Header("Items")]
    public int medkitCount = 0;   // จำนวนยาในกระเป๋า
    public float healAmount = 30f; // ฟื้นเลือดต่อ 1 ยา

    [Header("UI")]
    public GameObject youDiedPanel;   // UI "YOU DIED"
    public GameObject ammoPanel;      // UI กระสุน
    public GameObject crosshair;      // UI Crosshair
    public GameObject medkitTextUI; // ✅ อ้างอิง MedkitText

    private bool isDead = false;

    void Start()
    {
        health = maxHealth;

        if (youDiedPanel != null) youDiedPanel.SetActive(false);
        if (medkitTextUI != null) medkitTextUI.SetActive(true); // แสดงตอนเริ่ม
    }

    void Update()
    {
        if (health <= 0 && !isDead)
        {
            Die();
        }

        // ✅ ใช้ยาเมื่อกด H
        if (Input.GetKeyDown(KeyCode.H))
        {
            UseMedkit();
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    public void UseMedkit()
    {
        if (medkitCount > 0 && health < maxHealth)
        {
            medkitCount--;
            health += healAmount;
            health = Mathf.Clamp(health, 0, maxHealth);
            Debug.Log("Used Medkit! Heal +" + healAmount + " | Current HP: " + health);
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player Died!");

        // เปิด "YOU DIED"
        if (youDiedPanel != null)
            youDiedPanel.SetActive(true);

        // ปิด Crosshair + Ammo
        if (ammoPanel != null) ammoPanel.SetActive(false);
        if (crosshair != null) crosshair.SetActive(false);

        // ปิดการควบคุม
        if (GetComponent<Fps>() != null) GetComponent<Fps>().enabled = false;
        if (GetComponent<CharacterController>() != null) GetComponent<CharacterController>().enabled = false;

        // หยุดเกม
        Time.timeScale = 0f;
    }

    void LateUpdate()
    {
        if (isDead && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    // -------------------------------------------
    // IPlayerController REQUIREMENTS
    // -------------------------------------------
    public void EnableMovement(bool enable)
    {
        if (GetComponent<Fps>() != null)
            GetComponent<Fps>().enabled = enable;

        if (GetComponent<CharacterController>() != null)
            GetComponent<CharacterController>().enabled = enable;
    }

    public void OnGrabbed()
    {
        Debug.Log("Player grabbed by enemy!");
    }

    public void OnReleased()
    {
        Debug.Log("Player released!");
    }

}
