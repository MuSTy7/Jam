using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    [Header("Tarama Ayarlarý")]
    public float scanRange = 20f;
    public Transform playerCamera;

    [Header("Görsel Ayarlar")]
    public ParticleSystem pointCloudPrefab;
    [Range(0f, 1f)]
    public float particleAlpha = 0.4f; // YENÝ: Saydamlýk ayarý! 0 = Görünmez, 1 = Tamamen Katý

    [Header("Sýnýrlý Tarama Sistemi")]
    public int maxScanCount = 5;
    public int raysPerFrame = 20;
    public float spreadAngle = 0.3f;

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
            Vector3 randomDirection = playerCamera.forward + new Vector3(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle)
            );

            RaycastHit hit;
            if (Physics.Raycast(playerCamera.position, randomDirection, out hit, scanRange))
            {
                ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
                emitParams.position = hit.point;
                emitParams.startLifetime = Mathf.Infinity;

                // Renk ve derinlik hesabý
                float distanceRatio = hit.distance / scanRange;
                Color finalColor = Color.Lerp(Color.white, Color.black, distanceRatio);

                // ÝÞTE BURASI: Rengin Alpha (Saydamlýk) kanalýna bizim belirlediðimiz deðeri atýyoruz
                finalColor.a = particleAlpha;
                emitParams.startColor = finalColor;

                currentScan.Emit(emitParams, 1);
            }
        }
    }
}