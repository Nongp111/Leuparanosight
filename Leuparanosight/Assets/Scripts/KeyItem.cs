using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : MonoBehaviour
{
    public string keyName = "Key"; // ตั้งชื่อเฉพาะของกุญแจ

    [Header("Summon Settings")]
    public List<GameObject> enemyPrefabs;    // ✅ ศัตรูหลายชนิด
    public List<Transform> spawnPoints;      // ✅ จุดเกิดหลายจุด
    public bool summonOnPickup = true;       // เปิด/ปิดระบบเรียกศัตรู

    void OnTriggerEnter(Collider other)
    {
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddKey(keyName);

            if (summonOnPickup)
                StartCoroutine(SummonEnemies());

            Destroy(gameObject);
        }
    }

    IEnumerator SummonEnemies()
    {
        if (enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("No enemyPrefabs assigned in KeyItem!");
            yield break;
        }

        // ถ้าไม่มี spawnPoints ให้ใช้ตำแหน่งของ KeyItem เอง
        if (spawnPoints.Count == 0)
        {
            spawnPoints.Add(transform);
        }

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            // วน prefab ถ้าจำนวนน้อยกว่าจุดเกิด
            GameObject prefab = enemyPrefabs[i % enemyPrefabs.Count];
            Transform point = spawnPoints[i];

            Instantiate(prefab, point.position, point.rotation);
            Debug.Log($"Enemy {prefab.name} spawned at {point.position}");
        }
    }
}
