using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float health = 50f;
    public float damage = 10f;
    public float attackRange = 2f;
    public float visionRange = 10f;
    public float attackCooldown = 1.5f;

    [Header("Visual Effects")]
    public Color hitColor = Color.red;
    public float flashTime = 0.2f;

    [Header("Hitzone Settings")]
    public float headMultiplier = 2f;
    public float limbMultiplier = 0.75f;
    public float stunDurationHead = 1.5f;
    public float stunDurationLimb = 0.5f;

    private float lastAttackTime = 0f;
    private bool isStunned = false;
    private bool isDead = false;

    private NavMeshAgent agent;
    private Transform player;
    private Renderer rend;
    private Color originalColor;

    private Animator anim; // ✅ ตัวแปร Animator ใหม่

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            originalColor = rend.material.color;

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>(); // ✅ หา Animator
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null || isDead) return;

        // ถ้าอยู่ในสถานะสตัน หยุดทุกอย่าง
        if (isStunned)
        {
            if (anim != null)
                anim.SetFloat("Speed", 0f);
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // อัปเดต Speed ให้กับ Animator ตลอดเวลา
        if (anim != null)
            anim.SetFloat("Speed", agent.velocity.magnitude);

        if (distance <= visionRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            // ถ้าอยู่ในระยะโจมตี
            if (distance <= attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            agent.ResetPath();
            if (anim != null)
                anim.SetFloat("Speed", 0f);
        }
    }

    public void TakeDamage(float amount)
    {
        TakeDamage(amount, BodyPart.Body);
    }

    public void TakeDamage(float baseAmount, BodyPart part)
    {
        if (isDead) return;

        float finalDamage = baseAmount;

        // apply multiplier
        if (part == BodyPart.Head) finalDamage = baseAmount * headMultiplier;
        else if (part == BodyPart.Limb) finalDamage = baseAmount * limbMultiplier;

        health -= finalDamage;
        Debug.Log($"Enemy hit on {part}. Damage: {finalDamage}. HP: {health}");

        if (rend != null)
            StartCoroutine(FlashHit());

        // apply stun
        if (part == BodyPart.Head && stunDurationHead > 0f)
        {
            StartCoroutine(ApplyStun(stunDurationHead));
        }
        else if (part == BodyPart.Limb && stunDurationLimb > 0f)
        {
            StartCoroutine(ApplyStun(stunDurationLimb));
        }

        if (health <= 0f)
        {
            Die();
        }
    }

    IEnumerator FlashHit()
    {
        if (rend == null) yield break;
        rend.material.color = hitColor;
        yield return new WaitForSeconds(flashTime);
        if (rend != null)
            rend.material.color = originalColor;
    }

    IEnumerator ApplyStun(float duration)
    {
        if (isStunned) yield break;

        isStunned = true;
        if (agent != null) agent.isStopped = true;
        if (anim != null) anim.SetBool("Stunned", true);

        yield return new WaitForSeconds(duration);

        if (anim != null) anim.SetBool("Stunned", false);
        if (agent != null) agent.isStopped = false;
        isStunned = false;
    }

    void AttackPlayer()
    {
        if (isDead) return;

        if (anim != null)
            anim.SetTrigger("Attack");

        Player stats = player.GetComponent<Player>();
        if (stats != null)
        {
            stats.health -= damage;

            // เอฟเฟกต์โดนตี
            PlayerDamageEffect dmgFx = player.GetComponent<PlayerDamageEffect>();
            if (dmgFx != null) dmgFx.ShowDamageEffect();

            PlayerDamageVignette vignette = player.GetComponentInChildren<PlayerDamageVignette>();
            if (vignette != null) vignette.ShowHitEffect(0.3f);

            Debug.Log("Enemy attacked Player! Player HP: " + stats.health);
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Enemy Died!");

        if (agent != null)
            agent.isStopped = true;

        if (anim != null)
            anim.SetBool("Dead", true);

        // ปิดคอลลิเดอร์เพื่อไม่ให้โดนซ้ำ
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // รอแอนิเมชันจบก่อนลบตัวละคร
        Destroy(gameObject, 10f);
    }



}
