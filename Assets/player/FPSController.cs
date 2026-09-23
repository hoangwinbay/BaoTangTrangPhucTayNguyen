using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Cài đặt Di chuyển")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    private float currentSpeed;

    [Header("Cài đặt Camera")]
    public Transform playerCamera;
    public float mouseSensitivity = 200f;
    private float xRotation = 0f;

    [Header("Vật lý cơ bản")]
    public float gravity = -9.81f;
    private Vector3 velocity;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Khóa và ẩn con trỏ chuột vào giữa màn hình khi chơi
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. XỬ LÝ XOAY CAMERA (MOUSE LOOK)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Giới hạn góc nhìn lên/xuống (không ngửa cổ quá đà)

        // Xoay camera lên/xuống
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // Xoay thân nhân vật trái/phải
        transform.Rotate(Vector3.up * mouseX);

        // 2. XỬ LÝ CHUYỂN ĐỔI ĐI BỘ / CHẠY
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        // 3. XỬ LÝ DI CHUYỂN (WASD)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // 4. ÁP DỤNG TRỌNG LỰC
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Giữ nhân vật bám sát mặt đất
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}