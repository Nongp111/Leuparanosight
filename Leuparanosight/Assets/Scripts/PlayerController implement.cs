using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemyscreamer;

public class PlayerControllerimplement : MonoBehaviour, IPlayerController
{
    CharacterController cc;
    bool canMove = true;

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!canMove) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0, v);
        cc.Move(move * Time.deltaTime * 5f);
    }

    public void EnableMovement(bool enable)
    {
        canMove = enable;
    }

    public void OnGrabbed()
    {
        // animation/sound when grabbed
        Debug.Log("Player grabbed!");
    }

    public void OnReleased()
    {
        Debug.Log("Player released!");
    }
}
