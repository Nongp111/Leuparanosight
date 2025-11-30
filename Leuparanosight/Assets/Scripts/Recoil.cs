using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    public Vector3 recoilKickback = new Vector3(0f, 0.1f, -0.2f); // X=เงยขึ้น, Y=ขยับขึ้น, Z=ถอยหลัง
    public float recoilSpeed = 10f;   // ความเร็วตอนดีด
    public float returnSpeed = 5f;    // ความเร็วตอนคืนตำแหน่ง


    private Vector3 initialPos;
    private Vector3 currentRecoil;
    private Vector3 targetPos;

    void Start()
    {
        initialPos = transform.localPosition;
        targetPos = initialPos;
    }

    void Update()
    {
        // ค่อย ๆ คืนตำแหน่งกลับมา
        currentRecoil = Vector3.Lerp(currentRecoil, Vector3.zero, returnSpeed * Time.deltaTime);
        targetPos = initialPos + currentRecoil;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, recoilSpeed * Time.deltaTime);
    }

    // 🔥 เรียกฟังก์ชันนี้ตอนยิง
    public void ApplyRecoil()
    {
        currentRecoil += recoilKickback;
    }
}
