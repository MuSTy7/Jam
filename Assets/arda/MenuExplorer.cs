using UnityEngine;

public class MenuExplorer : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 2f;
    public float lookSensitivity = 2f;

    [Header("Otomatik Tarama (Mouse Odaklı)")]
    public ParticleSystem pointCloudPrefab;
    public float scanRange = 15f;
    public int raysPerScan = 100;
    public float scanInterval = 0.05f;

    [Header("Görsel Detaylar")]
    public Color pathColor = Color.red;
    public Color buttonRevealColor = Color.cyan;
    [Range(0f, 1f)]
    public float particleAlpha = 0.5f;

    // ── İç durum ────────────────────────────────────────────────────────
    private float rotationX = 0f;
    private float rotationY = 0f;
    private ParticleSystem currentScan;
    private float nextScanTime;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (pointCloudPrefab != null)
            currentScan = Instantiate(pointCloudPrefab, Vector3.zero, Quaternion.identity);
    }

    void Update()
    {
        HandleRotation();
        HandleMovement();

        if (Time.time >= nextScanTime)
        {
            PerformMenuScan();
            nextScanTime = Time.time + scanInterval;
        }
    }

    // ── Rotasyon — her zaman aktif ───────────────────────────────────────
    void HandleRotation()
    {
        rotationY += Input.GetAxis("Mouse X") * lookSensitivity;
        rotationX -= Input.GetAxis("Mouse Y") * lookSensitivity;
        rotationX = Mathf.Clamp(rotationX, -89f, 89f);

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }

    // ── WASD Hareketi ────────────────────────────────────────────────────
    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal"); // A / D
        float moveZ = Input.GetAxis("Vertical");   // W / S

        // transform.right ve forward rotasyondan sonra güncellendi, doğru yönü gösterir
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        if (move.magnitude > 1f) move.Normalize();

        transform.position += move * moveSpeed * Time.deltaTime;
    }

    // ── Mouse odaklı tarama ──────────────────────────────────────────────
    void PerformMenuScan()
    {
        if (currentScan == null || cam == null) return;

        // Ekran ortasından ışın gönder (mouse kilitli olduğu için merkez = bakış)
        Ray centerRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        for (int i = 0; i < raysPerScan; i++)
        {
            Vector3 spread = centerRay.direction + new Vector3(
                Random.Range(-0.1f, 0.1f),
                Random.Range(-0.1f, 0.1f),
                0f);

            RaycastHit hit;
            if (!Physics.Raycast(centerRay.origin, spread, out hit, scanRange)) continue;

            Color baseColor = Color.white;

            if (hit.collider.CompareTag("MenuButton"))
            {
                baseColor = buttonRevealColor;
                hit.collider.GetComponentInParent<WorldButton>()?.OnScanned();
            }
            else if (hit.collider.CompareTag("RunePath"))
            {
                baseColor = pathColor;
            }

            float distanceRatio = hit.distance / scanRange;
            Color finalColor = Color.Lerp(baseColor, Color.black, distanceRatio);
            finalColor.a = particleAlpha;

            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
            emitParams.position = hit.point;
            emitParams.startLifetime = 2f;
            emitParams.startColor = finalColor;

            currentScan.Emit(emitParams, 1);
        }
    }
}