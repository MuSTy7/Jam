using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Fiziksel Boyut Ayarlarý")]
    public float targetHeight = 3.5f;
    public float targetRadius = 0.3f;

    [Header("Hareket Ayarlarý")]
    public float speed = 6f; // Hýzý biraz artýrdým ki boþluklardan daha rahat atla
    public float jumpHeight = 2.5f; // Zýplama yüksekliði
    public float gravity = -25f; // Daha tok bir düþüþ için yerçekimini artýrdým

    [Header("Kamera Ayarlarý")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    private float xRotation = 0f;

    [Header("Yürüme Hissi (Head Bob)")]
    public float bobSpeed = 12f;
    public float bobAmount = 0.06f;
    private float defaultCameraY = 0f;
    private float timer = 0f;

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        SetupCharacterDimensions();

        if (playerCamera != null)
        {
            float eyeLine = targetHeight * 0.45f;
            playerCamera.localPosition = new Vector3(0, eyeLine, 0);
            defaultCameraY = playerCamera.localPosition.y;
        }
    }

    void SetupCharacterDimensions()
    {
        controller.height = targetHeight;
        controller.radius = targetRadius;
        controller.center = new Vector3(0, targetHeight / 2f, 0);

        Transform body = transform.Find("Body");
        if (body != null)
        {
            body.localScale = new Vector3(targetRadius * 2, targetHeight / 2f, targetRadius * 2);
        }
    }

    void Update()
    {
        if (controller == null || !controller.enabled) return;

        // 1. MOUSE BAKIÞI
        HandleRotation();

        // 2. YERÇEKÝMÝ VE ZEMÝN KONTROLÜ
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 3. YATAY HAREKET (WASD)
        HandleMovement();

        // 4. ZIPLAMA (BOÞLUK TUÞU)
        // Eðer yerdeyse ve Space tuþuna basýldýysa zýpla
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            // Fizik formülü: v = sqrt(h * -2 * g)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 5. YERÇEKÝMÝNÝ UYGULA
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 6. YÜRÜME EFEKTÝ
        ApplyHeadBob();
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (move.magnitude > 1) move.Normalize();

        controller.Move(move * speed * Time.deltaTime);
    }

    void ApplyHeadBob()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (controller.isGrounded && (Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f))
        {
            timer += Time.deltaTime * bobSpeed;
            playerCamera.localPosition = new Vector3(
                playerCamera.localPosition.x,
                defaultCameraY + Mathf.Sin(timer) * bobAmount,
                playerCamera.localPosition.z
            );
        }
        else
        {
            timer = 0;
            playerCamera.localPosition = new Vector3(
                playerCamera.localPosition.x,
                Mathf.Lerp(playerCamera.localPosition.y, defaultCameraY, Time.deltaTime * 5f),
                playerCamera.localPosition.z
            );
        }
    }
}