using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 1f;
    public float runSpeed = 3f;
    public float jumpPower = 0.5f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // preserve vertical movement (jump/gravity)
        float movementDirectionY = moveDirection.y;

        // combine input with speed
        float moveForward = Input.GetAxis("Vertical");   // W/S
        float moveRight = Input.GetAxis("Horizontal");   // A/D

        float curSpeedForward = canMove ? (isRunning ? runSpeed : walkSpeed) * moveForward : 0;
        float curSpeedRight = canMove ? (isRunning ? runSpeed : walkSpeed) * moveRight : 0;

        // final movement vector
        moveDirection = (forward * curSpeedForward) + (right * curSpeedRight);

        // restore Y (jump / gravity)
        moveDirection.y = movementDirectionY;


        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (characterController.isGrounded)
        {
            if (Input.GetButton("Jump") && canMove)
            {
                moveDirection.y = jumpPower;
            }
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }


        if (Input.GetKey(KeyCode.R) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;

        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 1f;
            runSpeed = 3f;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            // Vertical look (pitch) for the camera
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            // Horizontal look (yaw) for the player
            float rotationY = Input.GetAxis("Mouse X") * lookSpeed;
            transform.rotation *= Quaternion.Euler(0, rotationY, 0);
        }

    }
}