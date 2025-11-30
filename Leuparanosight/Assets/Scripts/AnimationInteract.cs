using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationInteract : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;

    [Header("Animation")]
    public Animator playerAnimator;
    public string vaultTrigger = "Vault";   // ชื่อ Trigger ใน Animator

    [Header("Player Control")]
    public Fps playerMovement;              // script movement 
    public CharacterController controller;  // player CharacterController

    [Header("Target Position")]
    public Transform vaultEndPoint;         // จุดที่ผู้เล่นต้องไปหลังปีนข้าม
    public bool movePlayerToEndPoint = true;

    private bool isPlayerNear = false;
    private bool isVaulting = false;

    void Update()
    {
        if (isPlayerNear && !isVaulting && Input.GetKeyDown(interactKey))
        {
            StartCoroutine(DoVault());
        }
    }

    private IEnumerator DoVault()
    {
        isVaulting = true;

        // ปิดการควบคุมผู้เล่น
        playerMovement.enabled = false;

        // เล่นอนิเมชัน
        playerAnimator.SetTrigger(vaultTrigger);

        // ระยะเวลาตามอนิเมชันของคุณ (แก้เองได้)
        yield return new WaitForSeconds(1.2f);

        // ย้ายตัวผู้เล่นไปอีกด้านของหน้าต่าง
        if (movePlayerToEndPoint && vaultEndPoint != null)
        {
            controller.enabled = false;
            playerMovement.transform.position = vaultEndPoint.position;
            controller.enabled = true;
        }

        // เปิดการควบคุมอีกครั้ง
        playerMovement.enabled = true;
        isVaulting = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            Debug.Log("Press E to vault.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
