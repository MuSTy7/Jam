using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    [Header("Tarama Ayarlarý")]
    public float scanRange = 15f;
    public Transform playerCamera;

    [Header("Görsel Ayarlar")]
    public ParticleSystem pointCloudPrefab;
    [Range(0f, 1f)]
    public float particleAlpha = 0.5f;

    [Header("Sýnýrlý Tarama Sistemi")]
    public int maxScanCount = 5;
    public int raysPerFrame = 200;

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
            {
                Destroy(oldestScan.gameObject);
            }
        }
    }

    void ContinueScanning()
    {
        for (int i = 0; i < raysPerFrame; i++)
        {
            Vector3 randomDirection = Random.onUnitSphere;

            RaycastHit hit;

            if (Physics.Raycast(playerCamera.position, randomDirection, out hit, scanRange))
            {
                // Kendi karakterimizi boyamamak için
                if (hit.collider.CompareTag("Player")) continue;

                ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
                emitParams.position = hit.point;
                emitParams.startLifetime = Mathf.Infinity;

                // ÝÞTE YENÝ TEHDÝT SÝSTEMÝ BURADA:
                Color baseColor = Color.white; // Varsayýlan rengimiz beyaz

                // Eðer çarptýðýmýz objenin Tag'i "Danger" ise rengi KIRMIZI yap!
                if (hit.collider.CompareTag("Danger"))
                {
                    baseColor = Color.red;
                }

                // Derinlik hesabý: Belirlediðimiz baseColor'dan (Beyaz veya Kýrmýzý) siyaha doðru karar
                float distanceRatio = hit.distance / scanRange;
                Color finalColor = Color.Lerp(baseColor, Color.black, distanceRatio);

                finalColor.a = particleAlpha;
                emitParams.startColor = finalColor;

                currentScan.Emit(emitParams, 1);
            }
        }
    }
}