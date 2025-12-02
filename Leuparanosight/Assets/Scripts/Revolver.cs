using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Revolver : MonoBehaviour
{
    [Header("References")]
    public Camera fpsCam;
    public Transform weaponRoot; // transform ของโมเดลปืน (local position ถูก lerp)
    public Transform aimTarget;  // empty GameObject ตำแหน่งที่ปืนจะไปเวลา ADS (set ใน inspector)

    [Header("Gun Settings")]
    public float range = 100f;
    public float damage = 20f;

    [Header("Fire Rate Settings")]
    public float fireRate = 2f;
    private float nextTimeToFire = 0f;

    [Header("Ammo Settings")]
    public int maxAmmoInChamber = 6;
    public int currentAmmo;
    public int reserveAmmo = 24;
    public float reloadPerBulletTime = 0.5f;
    private bool isReloading = false;

    [Header("Accuracy Settings")]
    public float baseSpread = 0.01f;     // spread ปกติ
    public float moveSpread = 0.05f;     // spread ตอนเดิน
    public float runSpread = 0.1f;       // spread ตอนวิ่ง (เพิ่มเข้ามาใหม่)
    public float aimingSpread = 0.003f;  // spread ขณะ ADS (จะลดลง)
    public float currentSpread;          // updated runtime
    public float spreadDecayRate = 5f;   // ความเร็วที่ spread จะลดลง

    [Header("ADS Settings")]
    public bool isAiming = false;
    public float aimSpeed = 4f;          // ความเร็ว lerp ระหว่าง hip <-> ads
    public float aimFOV = 50f;           // FOV ขณะ ADS
    private float originalFOV;
    private Vector3 weaponInitialLocalPos;
    private Quaternion weaponInitialLocalRot;
    private Vector3 aimLocalPos;
    private Quaternion aimLocalRot;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public GameObject bulletHolePrefab;
    public float bulletHoleLifeTime = 10f;
    public Animator gunAnimator;

    [Header("Muzzle Light")]
    public Light muzzleLight;
    public float muzzleLightDuration = 0.05f;


    void Start()
    {
        currentAmmo = maxAmmoInChamber;

        if (fpsCam != null) originalFOV = fpsCam.fieldOfView;
        if (weaponRoot != null)
        {
            weaponInitialLocalPos = weaponRoot.localPosition;
            weaponInitialLocalRot = weaponRoot.localRotation;
        }
        if (aimTarget != null)
        {
            // ใช้ตำแหน่ง/โรเตชันของ aimTarget (เป็น local space ของ weaponRoot parent/camera)
            aimLocalPos = aimTarget.localPosition;
            aimLocalRot = aimTarget.localRotation;
        }

        currentSpread = baseSpread;
    }

    void Update()
    {
        if (isReloading) return;

        // ยิง
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + (1f / fireRate);
            Shoot();
        }

        // รีโหลด
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmoInChamber && reserveAmmo > 0)
        {
            StartCoroutine(ReloadOneByOne());
        }

        // Animate walking
        bool isWalking = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;
        if (gunAnimator != null) gunAnimator.SetBool("IsWalking", isWalking);

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isWalking;
        if (gunAnimator != null) gunAnimator.SetBool("IsRunning", isRunning);

        // Update aim smoothing
        UpdateAimingLerp();
    }

    void UpdateAimingLerp()
    {
        if (weaponRoot == null || fpsCam == null || aimTarget == null) return;

        // Lerp camera FOV
        float targetFOV = isAiming ? aimFOV : originalFOV;
        fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView, targetFOV, Time.deltaTime * aimSpeed);

        // Lerp weapon local position/rotation toward aim target (assumes aimTarget is under same parent as weaponRoot)
        Vector3 targetLocalPos = isAiming ? aimLocalPos : weaponInitialLocalPos;
        Quaternion targetLocalRot = isAiming ? aimLocalRot : weaponInitialLocalRot;

        weaponRoot.localPosition = Vector3.Lerp(weaponRoot.localPosition, targetLocalPos, Time.deltaTime * aimSpeed);
        weaponRoot.localRotation = Quaternion.Slerp(weaponRoot.localRotation, targetLocalRot, Time.deltaTime * aimSpeed);

        // Update current spread to use during Shoot()
        // --- ส่วนที่แก้ไข ---
        bool isWalking = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;
        bool isRunning = isWalking && Input.GetKey(KeyCode.LeftShift);

        float targetSpread;
        if (isAiming)
        {
            targetSpread = aimingSpread; // ถ้ากำลังเล็ง ให้ใช้ค่า spread ที่แม่นยำที่สุดเสมอ
        }
        else if (isRunning)
        {
            targetSpread = runSpread; // ถ้าไม่ได้เล็ง และกำลังวิ่ง
        }
        else if (isWalking) 
        {
            targetSpread = moveSpread;
        }
        else
        {
            targetSpread = baseSpread; // ถ้าไม่ได้เล็ง และอยู่นิ่งๆ
        }
        currentSpread = Mathf.Lerp(currentSpread, targetSpread, Time.deltaTime * spreadDecayRate);
    }

    void Shoot()
    {
        if (currentAmmo <= 0) return;

        currentAmmo--;
        if (muzzleFlash != null) muzzleFlash.Play();
        if (gunAnimator != null) gunAnimator.SetTrigger("Shoot");

        // ยิง Raycast
        Vector3 shootDir = fpsCam.transform.forward;
        shootDir += fpsCam.transform.right * Random.Range(-currentSpread, currentSpread);
        shootDir += fpsCam.transform.up * Random.Range(-currentSpread, currentSpread);
        shootDir.Normalize();

        // --- DEBUG: วาดเส้นแสดงทิศทางกระสุน ---
        Debug.DrawRay(fpsCam.transform.position, shootDir * range, Color.red, 1.0f);

        if (Physics.Raycast(fpsCam.transform.position, shootDir, out RaycastHit hit, range))
        {
            // Hit handling (เหมือนเดิม)
            Hitbox hitbox = hit.collider.GetComponent<Hitbox>();
            BodyPart part = BodyPart.Body;

            if (hitbox != null)
            {
                part = hitbox.bodyPart;
                if (hitbox.parentEnemy != null)
                {
                    hitbox.parentEnemy.TakeDamage(damage, part);
                }
                else
                {
                    Enemy enemy = hit.transform.GetComponentInParent<Enemy>();
                    if (enemy != null) enemy.TakeDamage(damage, part);
                }
            }
            else
            {
                Enemy enemy = hit.transform.GetComponentInParent<Enemy>();
                if (enemy != null) enemy.TakeDamage(damage, BodyPart.Body);
            }

            BreakableCrate crate = hit.transform.GetComponent<BreakableCrate>();
            if (crate != null) crate.TakeDamage(damage);

            if (bulletHolePrefab != null && hit.collider != null)
            {
                Quaternion rot = Quaternion.LookRotation(hit.normal);
                GameObject hole = Instantiate(bulletHolePrefab, hit.point + hit.normal * 0.001f, rot);
                hole.transform.SetParent(hit.collider.transform);
                Destroy(hole, bulletHoleLifeTime);
            }

            if (muzzleFlash != null)
                muzzleFlash.Play();

            if (muzzleLight != null)
                StartCoroutine(MuzzleLightRoutine());
        }
    }

    IEnumerator ReloadOneByOne()
    {
        isReloading = true;

        while (currentAmmo < maxAmmoInChamber && reserveAmmo > 0)
        {
            yield return new WaitForSeconds(reloadPerBulletTime);
            currentAmmo++;
            reserveAmmo--;
            Debug.Log("Reloaded one bullet. Ammo: " + currentAmmo + "/" + reserveAmmo);
        }

        isReloading = false;
    }

    // --- Public API เพื่อให้ Breathing หรืออื่นๆ เรียก ---
    public void SetAiming(bool aim)
    {
        isAiming = aim;
        // ถ้าต้องการใช้ Animator parameter เพิ่มได้ เช่น:
        if (gunAnimator != null) gunAnimator.SetBool("IsAiming", aim);
    }

    public bool IsAiming() => isAiming;

    IEnumerator MuzzleLightRoutine()
    {
        if (muzzleLight == null) yield break;

        muzzleLight.enabled = true;
        yield return new WaitForSeconds(muzzleLightDuration);
        muzzleLight.enabled = false;
    }
}
