using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fps : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float runSpeed = 10f;
    public float mouseSensitivity = 2f;
    public Camera playerCamera;

    [Header("Crouch Settings")]
    public KeyCode crouchKey = KeyCode.LeftControl;
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchSpeed = 2.5f;
    public float crouchTransitionSpeed = 6f;

    [Header("Gravity Settings")]
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Head Bob Settings")]
    public float bobSpeedWalk = 8f;
    public float bobSpeedRun = 12f;
    public float bobAmountWalk = 0.025f;
    public float bobAmountRun = 0.05f;

    private float bobTimer = 0f;
    private Vector3 camDefaultPos;
    private CharacterController controller;
    private float xRotation = 0f;
    private bool isCrouching = false;
    private float defaultCameraY;
    private Vector3 defaultCenter;

    private Vector3 velocity;
    private bool isGrounded;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        defaultCenter = controller.center;
        defaultCameraY = playerCamera.transform.localPosition.y;

        if (groundCheck == null)
        {
            // สร้างอัตโนมัติถ้าไม่ได้ตั้งค่า
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = Vector3.down * (controller.height / 2f);
            groundCheck = gc.transform;
        }

        camDefaultPos = playerCamera.transform.localPosition;
    }

    void Update()
    {
        HandleLook();
        HandleMovement();
        HandleCrouch();
    }

    void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        // ตรวจสอบพื้น
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // ตรึงให้ติดพื้น

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) currentSpeed = runSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;

        controller.Move(move * currentSpeed * Time.deltaTime);
        // เรียก Head Bob
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        HandleHeadBob(x, z, isRunning);

        // เพิ่มแรงโน้มถ่วง
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            isCrouching = !isCrouching;

            controller.height = isCrouching ? crouchHeight : standingHeight;
            controller.center = isCrouching
                ? defaultCenter - new Vector3(0, (standingHeight - crouchHeight) / 2f, 0)
                : defaultCenter;
        }

        float targetCameraY = isCrouching ? defaultCameraY - 1f : defaultCameraY;
        Vector3 camPos = playerCamera.transform.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, targetCameraY, Time.deltaTime * crouchTransitionSpeed);
        playerCamera.transform.localPosition = camPos;
    }

    void HandleHeadBob(float moveX, float moveZ, bool isRunning)
    {
        if (playerCamera == null) return;

        // ถ้าไม่เดิน / ไม่ลงพื้น → ไม่ bob
        if (moveX == 0 && moveZ == 0 || !isGrounded)
        {
            // กลับตำแหน่งปกติอย่างนุ่มนวล
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                camDefaultPos,
                Time.deltaTime * 5f
            );
            return;
        }

        // ความเร็ว bob ต่างกันระหว่างเดิน/วิ่ง
        float bobSpeed = isRunning ? bobSpeedRun : bobSpeedWalk;
        float bobAmount = isRunning ? bobAmountRun : bobAmountWalk;

        bobTimer += Time.deltaTime * bobSpeed;

        // เคลื่อนกล้องขึ้นลงแบบคลื่น
        float bobX = Mathf.Sin(bobTimer) * bobAmount;
        float bobY = Mathf.Abs(Mathf.Cos(bobTimer)) * bobAmount;

        Vector3 targetPos = camDefaultPos + new Vector3(bobX, bobY, 0);

        playerCamera.transform.localPosition = targetPos;
    }
}
