using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Scanner : MonoBehaviour
{
    [Header("Tarama Ayarları")]
    public float scanRange = 15f;
    public Transform playerCamera;

    [Header("Görsel Ayarlar")]
    public ParticleSystem pointCloudPrefab;
    [Range(0f, 1f)]
    public float particleAlpha = 0.5f;

    [Header("Sınırlı Tarama Sistemi")]
    public int maxScanCount = 5;
    public int raysPerFrame = 200;

    // ─── Rün Renk Eşleşmeleri ───────────────────────────────────────────────
    // Tag adlarını Inspector'dan değiştirebilirsiniz.
    [Header("Rün Yolu Renkleri (Tag → Renk)")]
    public string rune1Tag = "Rune1 Tag";   // 1. rüne giden yol → Mavi
    public string rune2Tag = "Rune2 Tag";   // 2. rüne giden yol → Yeşil
    public string rune3Tag = "Rune3 Tag";   // 3. rüne giden yol → Sarı
    public string rune4Tag = "Rune4 Tag";
    public string dangerTag = "Danger";  // Tehlikeli alan      → Kırmızı

    // Kanalların baz renkleri
    private static readonly Color ColorRune1 = new Color(0.10f, 0.55f, 1.00f); // Mavi
    private static readonly Color ColorRune2 = new Color(0.10f, 1.00f, 0.30f); // Yeşil
    private static readonly Color ColorRune3 = new Color(1.00f, 0.90f, 0.10f); // Sarı
    private static readonly Color ColorRune4 = new Color(1.00f, 0.71f, 0.76f); // Sarı
    private static readonly Color ColorDanger = Color.red;
    private static readonly Color ColorDefault = Color.white;
    // ────────────────────────────────────────────────────────────────────────

    private Queue<ParticleSystem> activeScans = new Queue<ParticleSystem>();
    private ParticleSystem currentScan;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            StartNewScanSession();
        }

        if (Input.GetButton("Fire1") && currentScan != null)
        {
            ContinueScanning();
        }
    }

    void StartNewScanSession()
    {
        currentScan = Instantiate(pointCloudPrefab, transform.position, Quaternion.identity);
        activeScans.Enqueue(currentScan);

        if (activeScans.Count > maxScanCount)
        {
            ParticleSystem oldestScan = activeScans.Dequeue();
            if (oldestScan != null)
                Destroy(oldestScan.gameObject);
        }
    }

    void ContinueScanning()
    {
        for (int i = 0; i < raysPerFrame; i++)
        {
            Vector3 randomDirection = Random.onUnitSphere;
            RaycastHit hit;

            if (!Physics.Raycast(playerCamera.position, randomDirection, out hit, scanRange))
                continue;

            // Kendi karakterimizi taramayı atla
            if (hit.collider.CompareTag("Player")) continue;

            // ── Tag'e göre baz rengi seç ──────────────────────────────────
            Color baseColor = GetColorByTag(hit.collider.tag);

            // ── Derinliğe göre baz renkten siyaha lerp ────────────────────
            float distanceRatio = hit.distance / scanRange;
            Color finalColor = Color.Lerp(baseColor, Color.black, distanceRatio);
            finalColor.a = particleAlpha;

            // ── Partikülü yayınla ──────────────────────────────────────────
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
            emitParams.position = hit.point;
            emitParams.startLifetime = Mathf.Infinity;
            emitParams.startColor = finalColor;

            currentScan.Emit(emitParams, 1);
        }
    }

    Color GetColorByTag(string tag)
    {
        if (tag == rune1Tag) return ColorRune1;
        else if (tag == rune2Tag) return ColorRune2;
        else if (tag == rune3Tag) return ColorRune3;
        else if (tag == rune4Tag) return ColorRune4;
        else if (tag == dangerTag) return ColorDanger;
        else return ColorDefault;
    }
}