using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 1.5f;
    public Transform playerCamera;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference lookAction;
    public InputActionReference jumpAction;
    public InputActionReference sprintAction;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalVelocity;
    private float xRotation = 0f;
    private bool isSprinting;
    private bool jumpPressed;

    void OnEnable()
    {
        controller = GetComponent<CharacterController>();

        moveAction.action.Enable();
        lookAction.action.Enable();
        jumpAction.action.Enable();
        sprintAction.action.Enable();

        jumpAction.action.performed += _ => jumpPressed = true;
        jumpAction.action.canceled += _ => jumpPressed = false;
        sprintAction.action.performed += _ => isSprinting = true;
        sprintAction.action.canceled += _ => isSprinting = false;
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
        jumpAction.action.Disable();
        sprintAction.action.Disable();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();
        lookInput = lookAction.action.ReadValue<Vector2>();

        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        float speed = isSprinting ? sprintSpeed : walkSpeed;

        if (controller.isGrounded)
        {
            verticalVelocity = -2f;
            if (jumpPressed)
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        move.y = verticalVelocity;
        controller.Move(move * speed * Time.deltaTime);
    }
}
