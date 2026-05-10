using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class JumpscareTrigger : MonoBehaviour
{
    [Header("Görsel Ayarlar")]
    public GameObject jumpscarePhoto;

    [Header("Ses Ayarlarý")]
    [Tooltip("Patlayacak çýðlýk sesini buraya sürükle")]
    public AudioClip scareSound;

    [Tooltip("Sesin dosyadaki kaçýncý saniyeden baþlayacaðýný yaz (Örn: 2.5)")]
    public float sesBaslangic = 0f;

    [Tooltip("Sesin kaçýncý saniyede kesileceðini yaz (Örn: 4.0)")]
    public float sesBitis = 2.5f;

    [Header("Zamanlama ve Geçiþ")]
    [Tooltip("Jumpscare (Fotoðraf) ekranda kaç saniye kalsýn?")]
    public float scareDuration = 2.5f;
    [Tooltip("Jumpscare bitince dönülecek ana menü sahnesinin tam adý")]
    public string mainMenuSceneName = "MainMenu";

    private AudioSource screamSound;
    private bool hasTriggered = false;

    void Start()
    {
        screamSound = GetComponent<AudioSource>();
        screamSound.playOnAwake = false;

        if (scareSound != null)
        {
            screamSound.clip = scareSound;
        }

        if (jumpscarePhoto != null)
        {
            jumpscarePhoto.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("MainCamera")) && !hasTriggered)
        {
            StartCoroutine(ExecuteJumpscare());
        }
    }

    IEnumerator ExecuteJumpscare()
    {
        hasTriggered = true;

        // 1. OYUNCU HAREKETÝNÝ KES
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null) playerMovement.enabled = false;

        MenuExplorer menuExplorer = FindObjectOfType<MenuExplorer>();
        if (menuExplorer != null) menuExplorer.enabled = false;

        // 2. SESÝ ÇAL VE ZAMANLAMASINI AYARLA
        if (screamSound.clip != null)
        {
            // Sesin baþlayacaðý saniyeyi ayarla
            screamSound.time = sesBaslangic;
            screamSound.Play();

            // Sesin belirlenen bitiþ saniyesinde durmasý için arka planda sayaç baþlat
            if (sesBitis > sesBaslangic)
            {
                StartCoroutine(StopSoundRoutine());
            }
        }

        // 3. FOTOÐRAFI GÖSTER
        if (jumpscarePhoto != null)
        {
            jumpscarePhoto.SetActive(true);
        }

        // 4. BEKLE (Sahne geçiþi için)
        yield return new WaitForSeconds(scareDuration);

        // 5. FAREYÝ SERBEST BIRAK
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 6. ANA MENÜYE DÖN
        Debug.Log("Jumpscare bitti, " + mainMenuSceneName + " sahnesine dönülüyor...");
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Bu yeni fonksiyon, sadece sesi tam zamanýnda kesmekle görevlidir
    IEnumerator StopSoundRoutine()
    {
        // Sesin çalma süresi = Bitiþ saniyesi - Baþlangýç saniyesi
        float calmaSuresi = sesBitis - sesBaslangic;
        yield return new WaitForSeconds(calmaSuresi);

        if (screamSound.isPlaying)
        {
            screamSound.Stop();
        }
    }
}