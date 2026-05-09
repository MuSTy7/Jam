using System.Collections.Generic;
using UnityEngine;

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

    [Header("Ses Ayarları")]
    public AudioClip scanSound;           // Inspector'dan ses dosyasını sürükle
    [Range(0f, 1f)]
    public float scanVolume = 1f;

    // ─── Rün Renk Eşleşmeleri ───────────────────────────────────────────────
    [Header("Rün Yolu Renkleri (Tag → Renk)")]
    public string rune1Tag = "Rune1 Tag";
    public string rune2Tag = "Rune2 Tag";
    public string rune3Tag = "Rune3 Tag";
    public string rune4Tag = "Rune4 Tag";
    public string dangerTag = "Danger";

    private static readonly Color ColorRune1 = new Color(0.10f, 0.55f, 1.00f);
    private static readonly Color ColorRune2 = new Color(0.10f, 1.00f, 0.30f);
    private static readonly Color ColorRune3 = new Color(1.00f, 0.90f, 0.10f);
    private static readonly Color ColorRune4 = new Color(1.00f, 0.71f, 0.76f);
    private static readonly Color ColorDanger = Color.red;
    private static readonly Color ColorDefault = Color.white;

    private Queue<ParticleSystem> activeScans = new Queue<ParticleSystem>();
    private ParticleSystem currentScan;
    private AudioSource audioSource;

    void Start()
    {
        // AudioSource'u otomatik oluştur — Inspector'da ayrıca eklemeye gerek yok
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;     // 2D ses
        audioSource.loop = true;   // Basılı tutunca sürekli çalsın
        audioSource.volume = scanVolume;
    }

    void Update()
    {
        // Sol tık basıldığı an: yeni tarama + sesi başlat
        if (Input.GetButtonDown("Fire1"))
        {
            StartNewScanSession();
            StartScanSound();
        }

        // Sol tık basılı tutulurken: taramaya devam et
        if (Input.GetButton("Fire1") && currentScan != null)
        {
            ContinueScanning();
        }

        // Sol tık bırakılınca: sesi durdur
        if (Input.GetButtonUp("Fire1"))
        {
            StopScanSound();
        }
    }

    void StartScanSound()
    {
        if (scanSound == null || audioSource == null) return;
        audioSource.clip = scanSound;
        audioSource.volume = scanVolume;
        audioSource.Play();
    }

    void StopScanSound()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
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

            if (hit.collider.CompareTag("Player")) continue;

            Color baseColor = GetColorByTag(hit.collider.tag);
            float distanceRatio = hit.distance / scanRange;
            Color finalColor = Color.Lerp(baseColor, Color.black, distanceRatio);
            finalColor.a = particleAlpha;

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