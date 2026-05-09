using UnityEngine;
using System.Collections.Generic;

public class MenuExplorer : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 3f;
    public float lookSensitivity = 2f;
    public float fixedYHeight = 1.5f;

    [Header("Tarama Ayarları")]
    public ParticleSystem pointCloudPrefab;
    public float scanRange = 15f;
    public int raysPerScan = 120;
    public float scanInterval = 0.05f;

    [Header("Görsel Detaylar")]
    public Color pathColor = Color.red;
    public Color buttonRevealColor = Color.cyan;
    [Range(0f, 1f)]
    public float particleAlpha = 0.5f;

    private float rotationX = 0f;
    private float rotationY = 0f;
    private ParticleSystem currentScan;
    private float nextScanTime;

    void Start()
    {
        // Yüksekliği sabitle
        transform.position = new Vector3(transform.position.x, fixedYHeight, transform.position.z);

        // MOUSE KİLİTLEME: Fareyi ekranın ortasına gizle ve kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (pointCloudPrefab != null)
        {
            currentScan = Instantiate(pointCloudPrefab, Vector3.zero, Quaternion.identity);
        }

        Vector3 rot = transform.localRotation.eulerAngles;
        rotationY = rot.y;
        rotationX = rot.x;
    }

    void Update()
    {
        // ESC tuşuna basınca fareyi serbest bırak (Editörde kolaylık sağlar)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        HandleMovement();
        HandleRotation();

        if (Time.time >= nextScanTime)
        {
            PerformMenuScan();
            nextScanTime = Time.time + scanInterval;
        }
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 direction = (forward * moveZ + right * moveX).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, fixedYHeight, transform.position.z);
    }

    void HandleRotation()
    {
        // Mouse hareket ettiği anda (sağ tık beklemeden) rotasyonu güncelle
        rotationY += Input.GetAxis("Mouse X") * lookSensitivity;
        rotationX -= Input.GetAxis("Mouse Y") * lookSensitivity;
        rotationX = Mathf.Clamp(rotationX, -60f, 60f);

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
    }

    void PerformMenuScan()
    {
        if (currentScan == null) return;

        for (int i = 0; i < raysPerScan; i++)
        {
            // Mouse kilitli olduğu için artık ScreenPointToRay(Input.mousePosition) 
            // otomatik olarak ekranın tam ortasından ışın gönderir.
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            Vector3 spread = ray.direction + transform.TransformDirection(new Vector3(
                Random.Range(-0.03f, 0.03f),
                Random.Range(-0.03f, 0.03f),
                0));

            RaycastHit hit;
            if (Physics.Raycast(ray.origin, spread, out hit, scanRange))
            {
                ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
                emitParams.position = hit.point;
                emitParams.startLifetime = 2f;

                Color baseColor = Color.white;

                if (hit.collider.CompareTag("MenuButton"))
                {
                    baseColor = buttonRevealColor;
                    WorldButton wb = hit.collider.GetComponent<WorldButton>();
                    if (wb != null) wb.OnScanned();
                }
                else if (hit.collider.CompareTag("RunePath") || hit.collider.CompareTag("Danger"))
                {
                    baseColor = pathColor;
                }

                float distanceRatio = hit.distance / scanRange;
                Color finalColor = Color.Lerp(baseColor, Color.black, distanceRatio);
                finalColor.a = particleAlpha;

                emitParams.startColor = finalColor;
                currentScan.Emit(emitParams, 1);
            }
        }
    }
}