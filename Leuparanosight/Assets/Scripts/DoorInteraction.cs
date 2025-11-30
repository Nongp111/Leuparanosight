using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public Animator doorAnimator;
    private bool playerNearby = false;
    private bool isOpen = false;

    void Update()
    {
        // ถ้าอยู่ใกล้และกดปุ่ม E
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
            doorAnimator.SetBool("isOpen", isOpen);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            // อาจโชว์ UI แจ้งว่า "กด E เพื่อเปิด/ปิดประตู"
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            // ซ่อน UI แจ้งเตือน
        }
    }
}
