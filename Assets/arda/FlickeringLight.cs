using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    private Light lightSource;

    [Header("Iþýk Ayarlarý")]
    public float minIntensity = 0.5f; // En az ne kadar sönük olsun?
    public float maxIntensity = 2.0f; // En fazla ne kadar parlasýn?

    [Header("Hýz Ayarý")]
    [Range(0.01f, 0.2f)]
    public float flickerSpeed = 0.07f; // Deðiþim hýzý (ne kadar küçükse o kadar hýzlý titrer)

    private float targetIntensity;

    void Awake()
    {
        lightSource = GetComponent<Light>();
        targetIntensity = lightSource.intensity;
    }

    void Update()
    {
        // Rastgele bir hedef þiddet belirle
        targetIntensity = Random.Range(minIntensity, maxIntensity);

        // Iþýðýn þiddetini yumuþak bir geçiþle (Lerp) güncelle
        lightSource.intensity = Mathf.Lerp(lightSource.intensity, targetIntensity, flickerSpeed);
    }
}