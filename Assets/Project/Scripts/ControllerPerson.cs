using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ControllerPerson : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speedMeasure = 5f;
    [SerializeField] float timeSmooth = 0.1f;
    [Header("Jump/Gravity")]
    [SerializeField] float heightOfJump = 1.6f;
    [SerializeField] float gravity = -20f;

    [Header("References")]
    [SerializeField] Transform cameraTransform;

    CharacterController controller;
    float turnSmoothVelocity;
    Vector3 velocity;

    void Awake() { controller = GetComponent<CharacterController>(); }

    void Update()
    {
        bool grounded = controller.isGrounded;
        if (grounded && velocity.y < 0f) velocity.y = -2f;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (Mathf.Approximately(h, 0f) && Mathf.Approximately(v, 0f)) {
            h = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
            v = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
        }

        Vector3 input = new Vector3(h, 0f, v);
        float mt = input.magnitude;

        if (mt >= 0.1f)
        {
            float tarAngl = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, tarAngl, ref turnSmoothVelocity, timeSmooth);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, tarAngl, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speedMeasure * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && grounded)
            velocity.y = Mathf.Sqrt(heightOfJump * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}