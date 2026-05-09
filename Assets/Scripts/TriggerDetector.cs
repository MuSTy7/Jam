using UnityEngine;
using UnityEngine.InputSystem.XR;

public class TriggerDetector : MonoBehaviour
{
    [Header("Hangi Balta Tetiklenecek?")]
    public AxeController connectedAxe; // Baltadaki scripti buraya baðlayacaðýz

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (connectedAxe != null)
            {
                // Karaktere deðince baltadaki fonksiyonu çalýþtýr
                connectedAxe.ActivateTrap();
                Debug.Log("Balta düþtü!");

                // Ýstersen tetikleyiciyi sil ki bir daha çalýþmasýn
                Destroy(gameObject);
            }
        }
    }
}