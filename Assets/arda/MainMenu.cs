using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // "Sistemi Baþlat" butonu için
    public void StartGame()
    {
        // Oyunun olduðu sahnenin adýný buraya yazýn
        SceneManager.LoadScene("Mustafa");
    }

    // "Baðlantýyý Kes" butonu için
    public void QuitGame()
    {
        Debug.Log("Oyun kapatýldý.");
        Application.Quit();
    }
}