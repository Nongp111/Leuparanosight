using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    [Header("References")]
    public Camera fpsCam;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float damage = 15f;
    public float attackCooldown = 0.5f;

    [Header("Durability Settings")]
    public int maxDurability = 5;         // ค่า durability สูงสุด
    public int currentDurability = 5;     // ค่า durability ปัจจุบัน
    public bool breakOnVine = true;       // ถ้าฟัน Vine ให้มีดพังทันที

    private float lastAttackTime = 0f;

    private Recoil recoil;
    private WeaponManager weaponManager;

    void Start()
    {
        recoil = GetComponent<Recoil>();
        weaponManager = GetComponentInParent<WeaponManager>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time > lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;

            if (recoil != null) recoil.ApplyRecoil();
        }
    }

    void Attack()
    {
        Debug.Log("Knife Attack!");

        // ลด durability เฉพาะเวลาฟัน (ถ้าต้องการ)
        ReduceDurability(1);

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out RaycastHit hit, attackRange))
        {
            // ⭐ ฟัน VineObstacle
            VineObstacle vine = hit.transform.GetComponent<VineObstacle>();
            if (vine != null)
            {
                float beforeHealth = vine.health;

                vine.TakeDamage(damage);

                // ถ้า Vine พังในจังหวะนี้ → Knife พังพร้อมกัน
                if (beforeHealth > 0 && vine.health <= 0)
                {
                    BreakKnife();
                    return;
                }
            }

            // ฟันศัตรูปกติ
            Hitbox hitbox = hit.collider.GetComponent<Hitbox>();
            BodyPart part = BodyPart.Body;

            if (hitbox != null && hitbox.parentEnemy != null)
            {
                part = hitbox.bodyPart;
                hitbox.parentEnemy.TakeDamage(damage, part);
            }
            else
            {
                Enemy enemy = hit.transform.GetComponentInParent<Enemy>();
                if (enemy != null)
                    enemy.TakeDamage(damage, BodyPart.Body);
            }

            // ฟันลัง
            BreakableCrate crate = hit.transform.GetComponent<BreakableCrate>();
            if (crate != null)
            {
                crate.TakeDamage(damage);
            }
        }
    }

    void ReduceDurability(int amount)
    {
        currentDurability -= amount;

        if (currentDurability <= 0)
        {
            BreakKnife();
        }
    }

    void BreakKnife()
    {
        Debug.Log("Knife destroyed together with vine!");

        // ปิดมีด
        gameObject.SetActive(false);

        // durability = 0 เพื่อให้อาวุธนี้ใช้ไม่ได้อีกแล้ว
        currentDurability = 0;

        // สลับไปมือเปล่า
        if (weaponManager != null)
            weaponManager.PickupWeapon(0);

        // ❌ ไม่ต้อง Destroy(gameObject)
        // Destroy(gameObject, 0.1f);  <-- ลบออก
    }
}
