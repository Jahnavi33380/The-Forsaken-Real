using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player;
    public float sensitivity = 2f;
    public float distance = 5f;
    public float height = 2f;

    private float yaw;
    private float pitch;

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, -30f, 60f); // limit up/down

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 targetPos = player.position - rotation * Vector3.forward * distance + Vector3.up * height;

        transform.position = targetPos;
        transform.LookAt(player.position + Vector3.up * 1.5f); // look at player chest/head
    }
}
