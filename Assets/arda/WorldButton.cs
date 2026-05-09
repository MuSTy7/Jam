using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WorldButton : MonoBehaviour
{
    public enum ButtonType { LoadScene, QuitGame }

    [Header("Buton Tipi")]
    public ButtonType type = ButtonType.LoadScene;

    [Header("Görsel Ayarlar")]
    public TextMeshProUGUI buttonText;
    public float revealDuration = 1.0f;
    public float fadeSpeed = 2f;

    [Header("Eðer Sahne Yüklenecekse")]
    public string sceneToLoad;
    public float requiredVisibility = 0.5f;

    private float lastScannedTime;
    private Color textColor;

    void Start()
    {
        if (buttonText == null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();

        textColor = buttonText.color;
        textColor.a = 0;
        buttonText.color = textColor;
    }

    void Update()
    {
        bool isScanned = (Time.time - lastScannedTime < revealDuration);

        // Görünürlük mantýðý
        if (isScanned)
            textColor.a = Mathf.Lerp(textColor.a, 1f, Time.deltaTime * fadeSpeed * 2);
        else
            textColor.a = Mathf.Lerp(textColor.a, 0f, Time.deltaTime * fadeSpeed);

        buttonText.color = textColor;

        // Etkileþim: Yazý yeterince görünürse ve E'ye basýlýrsa
        if (textColor.a >= requiredVisibility && isScanned)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ExecuteAction();
            }
        }
    }

    void ExecuteAction()
    {
        if (type == ButtonType.LoadScene)
        {
            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        else if (type == ButtonType.QuitGame)
        {
            Debug.Log("Sistem kapatýlýyor...");
            Application.Quit(); // Oyundan çýkar
        }
    }

    public void OnScanned()
    {
        lastScannedTime = Time.time;
    }
}