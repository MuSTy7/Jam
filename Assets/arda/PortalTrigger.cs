using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    [Header("Ayarlar")]
    public string targetScene = "MainMenu"; // Geçilecek sahne adý

    // Tetikleyiciye bir þey girdiðinde çalýþýr
    void OnTriggerEnter(Collider other)
    {
        // Giren objenin Tag'i "Player" ise (Senin karakterin)
        if (other.CompareTag("Player"))
        {
            // Eðer sahne geçiþ karartmasý (Fader) varsa onu tetikle
            if (SceneFader.instance != null)
            {
                SceneFader.instance.FadeToScene(targetScene);
            }
            else
            {
                // Eðer Fader yoksa direkt sahneyi yükle (Yedek plan)
                UnityEngine.SceneManagement.SceneManager.LoadScene(targetScene);
            }
        }
    }
}