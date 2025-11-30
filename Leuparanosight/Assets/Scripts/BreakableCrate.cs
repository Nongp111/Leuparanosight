using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableCrate : MonoBehaviour
{
    public float health = 10f; // HP ของกล่อง
    public GameObject dropItemPrefab; // Prefab ที่จะดรอป (เช่น AmmoBox)
    public int dropAmount = 1; // จำนวนของที่ดรอป
    public Transform dropPoint; // จุด spawn (ถ้าไม่ใส่ ใช้ตำแหน่งกล่อง)

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0) BreakCrate();
    }

    void BreakCrate()
    {
        Debug.Log("Crate broken!");

        Destroy(gameObject); // ทำลายกล่อง
    }
}
