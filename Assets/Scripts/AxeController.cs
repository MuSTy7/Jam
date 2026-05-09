using UnityEngine;

public class AxeController : MonoBehaviour
{
    [Header("Tuzak Ayarlarý")]
    public Transform pivotPoint;
    public float swingSpeed = 2f;
    public float maxAngle = 70f;

    private bool isTriggered = false;

    void Update()
    {
        if (isTriggered)
        {
            // Zamanla gidip gelen sarkaç matematiði
            float angle = Mathf.Sin(Time.time * swingSpeed) * maxAngle;
            pivotPoint.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }

    // Bu fonksiyonu diðer scriptten çaðýracaðýz!
    public void ActivateTrap()
    {
        isTriggered = true;
    }
}