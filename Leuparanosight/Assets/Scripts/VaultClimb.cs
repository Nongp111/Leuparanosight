using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaultClimb : MonoBehaviour
{
    public float detectDistance = 1.2f;
    public float wallHeight = 1.5f;
    public float climbSpeed = 3f;
    public LayerMask obstacleMask;
    public KeyCode climbKey = KeyCode.E;

    private bool isClimbing = false;
    private Vector3 targetPos;
    private CharacterController controller;
    private Animator anim;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isClimbing)
        {
            // เคลื่อนตัวขึ้นไปยังจุดปีนแบบลื่น ๆ
            transform.position = Vector3.MoveTowards(transform.position, targetPos, climbSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
                isClimbing = false;

            return;
        }

        // กดปุ่มเพื่อปีน
        if (Input.GetKeyDown(climbKey))
        {
            TryClimb();
        }
    }

    void TryClimb()
    {
        RaycastHit hit;

        // ยิง ray หาสิ่งกีดขวางตรงหน้า
        if (Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, out hit, detectDistance, obstacleMask))
        {
            // ยิงขึ้นบนหาขอบด้านบน
            RaycastHit topHit;
            Vector3 topOrigin = hit.point + Vector3.up * wallHeight;

            if (Physics.Raycast(topOrigin, Vector3.down, out topHit, wallHeight + 1f))
            {
                // จุดสุดท้ายที่ต้องไป
                targetPos = topHit.point + Vector3.forward * 0.3f;

                // เปิดโหมดปีน
                isClimbing = true;

                if (anim != null)
                    anim.SetTrigger("Climb");
            }
        }
    }
}
