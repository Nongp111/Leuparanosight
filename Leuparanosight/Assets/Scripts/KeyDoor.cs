using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyDoor : MonoBehaviour
{
    public string requiredKey = "BasementKey"; // ชื่อกุญแจที่ต้องมี
    public bool destroyAfterUnlock = true;     // ให้หายไปหลังเปิดไหม
    public string nextSceneName;               // (ทางเลือก) ถ้าจะเปลี่ยนด่าน

    private bool isUnlocked = false;

    void OnTriggerEnter(Collider other)
    {
        if (isUnlocked) return;

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            if (inventory.HasKey(requiredKey))
            {
                UnlockDoor();
            }
            else
            {
                Debug.Log("คุณยังไม่มีไอเท็มที่ใช้เปิดสิ่งนี้...");
            }
        }
    }

    void UnlockDoor()
    {
        Debug.Log("Unlocked with key: " + requiredKey);
        isUnlocked = true;

        // ถ้ามีประตูให้เปิด / พื้นปลดล็อก
        if (destroyAfterUnlock)
            Destroy(gameObject);

        // หรือจะเปลี่ยนฉาก
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
