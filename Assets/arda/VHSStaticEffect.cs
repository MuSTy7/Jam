using UnityEngine;

public class VHSStaticEffect : MonoBehaviour
{
    public float shakeAmount = 0.02f;
    public float flickerSpeed = 25f;

    private Vector3 originalPos;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        // Kamera hafif titreþim
        float x = Random.Range(-shakeAmount, shakeAmount);
        float y = Random.Range(-shakeAmount, shakeAmount);

        transform.localPosition =
            originalPos + new Vector3(x, y, 0);

        // Rastgele frame drop hissi
        if (Random.Range(0, flickerSpeed) < 1)
        {
            transform.localRotation =
                Quaternion.Euler(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                );
        }
        else
        {
            transform.localRotation = Quaternion.identity;
        }
    }
}