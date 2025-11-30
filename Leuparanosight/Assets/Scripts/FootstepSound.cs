using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    public AudioSource footstepSource;
    public CharacterController controller;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        bool isMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f;

        // ถ้าขยับตัว + อยู่บนพื้น
        if (controller.isGrounded && isMoving)
        {
            if (!footstepSource.isPlaying)
                footstepSource.Play();
        }
        else
        {
            if (footstepSource.isPlaying)
                footstepSource.Stop();
        }

    }
}
