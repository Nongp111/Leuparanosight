using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] weapons;      // 0 = ปืน, 1 = มีด, 2 = มือเปล่า
    private int selectedWeapon = 0;

    public float hideTime = 0.3f;     // เวลาอนิเมชันเก็บอาวุธ
    public float drawTime = 0.3f;     // เวลาอนิเมชันหยิบอาวุธ

    private bool isSwitching = false;

    private Animator currentWeaponAnimator;

    void Start()
    {
        SelectWeaponInstant(0);
    }

    void Update()
    {
        if (isSwitching) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) StartCoroutine(SwitchWeapon(0));  // ปืน
        if (Input.GetKeyDown(KeyCode.Alpha2)) StartCoroutine(SwitchWeapon(1));  // มีด
        if (Input.GetKeyDown(KeyCode.Alpha3)) StartCoroutine(SwitchWeapon(2));  // มือเปล่า
    }

    // -------------------------------
    // เปลี่ยนอาวุธแบบมีอนิเมชัน
    // -------------------------------
    IEnumerator SwitchWeapon(int newIndex)
    {
        if (newIndex == selectedWeapon || isSwitching)
            yield break;

        isSwitching = true;

        // 🔥 เล่นอนิเมชันเก็บอาวุธเก่า
        currentWeaponAnimator = GetWeaponAnimator(selectedWeapon);
        if (currentWeaponAnimator != null)
            currentWeaponAnimator.SetTrigger("HideWeapon");

        yield return new WaitForSeconds(hideTime);

        // ปิดอาวุธเก่า-เปิดอาวุธใหม่
        weapons[selectedWeapon].SetActive(false);
        selectedWeapon = newIndex;
        weapons[selectedWeapon].SetActive(true);

        // 🔥 เล่นอนิเมชันหยิบอาวุธใหม่
        currentWeaponAnimator = GetWeaponAnimator(selectedWeapon);
        if (currentWeaponAnimator != null)
            currentWeaponAnimator.SetTrigger("DrawWeapon");

        yield return new WaitForSeconds(drawTime);

        isSwitching = false;
    }

    // -------------------------------
    // ใช้ตอนเริ่มเกม ไม่ต้องเล่นอนิเมชัน
    // -------------------------------
    void SelectWeaponInstant(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
            weapons[i].SetActive(i == index);

        selectedWeapon = index;
        currentWeaponAnimator = GetWeaponAnimator(index);
    }

    // ดึง Animator จากอาวุธนั้น ๆ
    private Animator GetWeaponAnimator(int index)
    {
        return weapons[index].GetComponentInChildren<Animator>();
    }

    // ดึง index ของอาวุธปัจจุบัน
    public int GetSelectedWeaponIndex() => selectedWeapon;

    // -------------------------------
    // ใช้เวลาหยิบอาวุธจาก Pickup
    // -------------------------------
    public void PickupWeapon(int index)
    {
        StartCoroutine(PickupRoutine(index));
    }

    IEnumerator PickupRoutine(int newIndex)
    {
        // ปิดอาวุธเก่า
        weapons[selectedWeapon].SetActive(false);

        // เปิดอาวุธใหม่
        selectedWeapon = newIndex;
        weapons[selectedWeapon].SetActive(true);

        // เล่นอนิเมชัน Draw ของอาวุธใหม่
        currentWeaponAnimator = GetWeaponAnimator(selectedWeapon);
        if (currentWeaponAnimator != null)
            currentWeaponAnimator.SetTrigger("DrawWeapon");

        yield return new WaitForSeconds(drawTime);
    }
}
