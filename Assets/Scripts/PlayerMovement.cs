using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float speed = 3.5f; // Hýzý biraz düþürdük, gerilim için daha iyi
    public float gravity = -15f; // Yere düþme hýzýmýz (Yerçekimi)

    [Header("Kamera Ayarlarý")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    private float xRotation = 0f;

    [Header("Yürüme Hissi (Head Bob)")]
    public float bobSpeed = 12f; // Adým atma hýzý (Kafanýn sallanma ritmi)
    public float bobAmount = 0.05f; // Kafanýn ne kadar þiddetli sallanacaðý
    private float defaultCameraY = 0f;
    private float timer = 0f;

    private CharacterController controller;
    private Vector3 velocity; // Karakterin düþüþ hýzý
    private bool isGrounded; // Yerde miyiz?

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        // Baþlangýçta kameranýn boyunu hafýzaya alalým ki hep o hizaya dönebilsin
        if (playerCamera != null)
        {
            defaultCameraY = playerCamera.localPosition.y;
        }
    }

    void Update()
    {
        // --- 1. KAMERA DÖNÜÞÜ ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // --- 2. YERÇEKÝMÝ KONTROLÜ (Uçmayý ve Havada Kalmayý Engeller) ---
        // CharacterController'ýn alt kýsmý yere deðiyor mu?
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Yerdeyken sürekli hafifçe yere bastýr ki merdivenlerden falan düzgün inebilsin
        }

        // --- 3. OYUNCU HAREKETÝ ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // Önce saða/sola/ileriye hareket ettir
        controller.Move(move * speed * Time.deltaTime);

        // Sonra yerçekimini hesapla ve aþaðý doðru çek
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // --- 4. YÜRÜME HÝSSÝ (HEAD BOB) ---
        // Eðer oyuncu herhangi bir tuþa basýyorsa (hareket ediyorsa)
        if (Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f)
        {
            if (isGrounded) // Sadece yerdeyken kafa sallansýn
            {
                timer += Time.deltaTime * bobSpeed;
                playerCamera.localPosition = new Vector3(
                    playerCamera.localPosition.x,
                    defaultCameraY + Mathf.Sin(timer) * bobAmount,
                    playerCamera.localPosition.z
                );
            }
        }
        else
        {
            // Oyuncu durduysa, kamerayý yumuþak bir þekilde eski yüksekliðine geri getir
            timer = 0;
            playerCamera.localPosition = new Vector3(
                playerCamera.localPosition.x,
                Mathf.Lerp(playerCamera.localPosition.y, defaultCameraY, Time.deltaTime * bobSpeed),
                playerCamera.localPosition.z
            );
        }
    }
}