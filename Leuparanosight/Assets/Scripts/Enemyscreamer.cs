using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public interface IPlayerController
{
    void EnableMovement(bool enable);
    void OnGrabbed();
    void OnReleased();
}

[RequireComponent(typeof(NavMeshAgent))]
public class Enemyscreamer : MonoBehaviour
{

    [Header("Damage")]
    public float grabDamage = 10f;        // ดาเมจต่อ 1 tick
    public float damageInterval = 0.5f;   // ทุก 0.5 วิทำดาเมจ 1 ครั้ง
    private enum State { Idle, Investigate, ChasePlayer, Grabbing, Stunned }
    private State currentState = State.Idle;

    private NavMeshAgent agent;

    [Header("Detection")]
    public float hearingRadius = 10f;
    public float grabRange = 1.5f;

    [Header("Grab Behaviour")]
    public float grabDuration = 3f;
    public float stunDuration = 2f;

    private Transform targetPlayer;
    private IPlayerController playerController;

    private Vector3 lastHeardPosition;

    private void OnEnable()
    {
        PlayerNoise.OnNoiseEmitted += HandleNoise;
    }

    private void OnDisable()
    {
        PlayerNoise.OnNoiseEmitted -= HandleNoise;
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                break;

            case State.Investigate:
                agent.SetDestination(lastHeardPosition);

                if (Vector3.Distance(transform.position, lastHeardPosition) < 1f)
                {
                    SetIdle();
                }
                break;


            case State.ChasePlayer:

                if (currentState == State.Stunned)
                    return;
                // ถ้ากำลัง Grab หรือ Stun → หยุดทันที
                if (currentState != State.ChasePlayer)
                    return;

                if (targetPlayer == null)
                {
                    currentState = State.Investigate;
                    return;
                }

                agent.SetDestination(targetPlayer.position);

                float dist = Vector3.Distance(transform.position, targetPlayer.position);

                if (dist <= grabRange)
                {
                    // ล็อกก่อนทุกครั้ง
                    currentState = State.Grabbing;
                    agent.isStopped = true;
                    StartCoroutine(PerformGrab());
                    return; // ❗ ออกจาก ChasePlayer ทันที
                }
                break;

            case State.Grabbing:
                // ไม่ทำอะไร
                break;

            case State.Stunned:
                // ไม่ทำอะไร
                break;
        }
    }

    private void HandleNoise(Vector3 position, float radius)
    {
        // ถ้า Enemy หูดับอยู่ → ไม่ได้ยินเสียงผู้เล่น
        if (currentState == State.Stunned)
            return;

        float d = Vector3.Distance(transform.position, position);

        if (d <= radius || d <= hearingRadius)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");

            if (p != null)
            {
                targetPlayer = p.transform;
                playerController = p.GetComponent<IPlayerController>();
            }

            lastHeardPosition = position;
            currentState = State.ChasePlayer;
            agent.isStopped = false;

            Debug.Log("Enemy heard noise -> Chase");
        }
    }

    private void SetIdle()
    {
        currentState = State.Idle;
        agent.isStopped = true;
    }

    private IEnumerator PerformGrab()
    {
        Debug.Log("Enemy started grab!");

        currentState = State.Grabbing;
        agent.isStopped = true;

        // Disable player movement
        if (playerController != null)
        {
            playerController.EnableMovement(false);
            playerController.OnGrabbed();
        }
        // เริ่มทำดาเมจระหว่างจับ
        StartCoroutine(DealGrabDamage());

        yield return new WaitForSeconds(grabDuration);

        // Release player
        if (playerController != null)
        {
            playerController.EnableMovement(true);
            playerController.OnReleased();
        }

        // หลังปล่อย → ให้ถอยออกจาก player ก่อน
        StartCoroutine(MoveBackwardThenStun());
    }
    private IEnumerator DealGrabDamage()
    {
        float timer = 0f;

        while (currentState == State.Grabbing)
        {
            // ทำดาเมจทุก damageInterval
            timer += Time.deltaTime;

            if (timer >= damageInterval)
            {
                timer = 0f;

                // ส่งดาเมจไปที่ Player
                var health = targetPlayer.GetComponent<Player>();
                if (health != null)
                {
                    health.TakeDamage(grabDamage);
                }
            }

            yield return null;
        }
    }

    private IEnumerator MoveBackwardThenStun()
    {
        currentState = State.Grabbing; // กันไม่ให้เริ่ม grab ใหม่
        agent.isStopped = false;

        // ทิศทางถอยออก
        Vector3 dir = (transform.position - targetPlayer.position).normalized;
        Vector3 backPos = transform.position + dir * 2.5f; // ถอย 2.5 เมตร

        agent.SetDestination(backPos);

        // รอจนกว่าจะถอยถึงจุดหรือเกิน 0.6 วินาที (กันติด)
        float timer = 0f;
        while (timer < 0.6f)
        {
            timer += Time.deltaTime;

            // ถ้าถึงตำแหน่งแล้วหยุด
            if (Vector3.Distance(transform.position, backPos) < 0.4f)
                break;

            yield return null;
        }

        // รอให้หยุดอยู่กับที่ก่อนเข้า Stun
        agent.isStopped = true;

        // เริ่ม stun
        StartCoroutine(ApplyStun());
    }

    private IEnumerator ApplyStun()
    {
        Debug.Log("Enemy stunned!");

        currentState = State.Stunned;
        agent.isStopped = true;

        yield return new WaitForSeconds(stunDuration);

        SetIdle();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, grabRange);
    }
}