using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Sahne Geçiþ Ayarlarý")]
    public string mainMenuSceneName = "MainMenu"; // Geçilecek sahnenin tam adý

    private void OnTriggerEnter(Collider other)
    {
        // Eðer çarptýðýmýz objenin adý "KillZone" ise
        if (other.gameObject.name == "KillZone")
        {
            GoToMainMenu();
        }
    }

    public void GoToMainMenu()
    {
        // Daha önce oluþturduðumuz SceneFader sistemi varsa karartarak geç
        if (SceneFader.instance != null)
        {
            SceneFader.instance.FadeToScene(mainMenuSceneName);
        }
        else
        {
            // Eðer Fader yoksa (yedek plan) anýnda yükle
            UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
        }

        Debug.Log("KillZone tetiklendi: Ana menüye dönülüyor.");
    }
}