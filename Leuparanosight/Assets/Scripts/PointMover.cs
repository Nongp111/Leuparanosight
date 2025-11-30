using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMover : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;

    private int currentIndex = 0;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        MoveAlongWaypoints();
        CheckButtonForAnimation();
    }

    void MoveAlongWaypoints()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentIndex];
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentIndex = (currentIndex + 1) % waypoints.Length;
        }
    }

    void CheckButtonForAnimation()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("PlayAnim");
        }
    }
}
