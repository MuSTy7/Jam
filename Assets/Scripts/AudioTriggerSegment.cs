using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioTriggerSegment : MonoBehaviour
{
    [Header("Ses Dosyası ve Seviyesi")]
    [Tooltip("Çalmasını istediğin sesi buraya sürükle.")]
    public AudioClip tetiklenecekSes;

    [Tooltip("Sesin yüksekliği (0 ile 1 arasında).")]
    [Range(0f, 1f)]
    public float sesSeviyesi = 1f;

    [Header("Zaman Ayarları (Saniye)")]
    public float baslangicSaniyesi = 0f;
    public float bitisSaniyesi = 5f;

    [Header("Tetiklenme Ayarları")]
    [Tooltip("Oyuncunun Tag'i 'Player' olmalıdır.")]
    public string oyuncuTag = "Player";

    [Tooltip("İşaretliyse, oyuncu buradan geçtiğinde ses sadece 1 KERE çalar. İşareti kaldırırsan her geçişinde çalar.")]
    public bool sadeceBirKereCal = true;

    private AudioSource audioSource;
    private bool dahaOnceCaldi = false;
    private bool sureyiTakipEt = false;

    void Start()
    {
        // Scriptin eklendiği objedeki Audio Source'u otomatik bulur
        audioSource = GetComponent<AudioSource>();

        // Oyun başlar başlamaz çalmasını engellemek için güvenlik önlemi
        audioSource.playOnAwake = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // Eğer temas eden obje "Player" ise
        if (other.CompareTag(oyuncuTag))
        {
            // Sadece 1 kere çalması istenmişse ve zaten çalmışsa, işlemi iptal et
            if (sadeceBirKereCal && dahaOnceCaldi)
                return;

            // Eğer Inspector'dan bir ses dosyası koyulmuşsa işlemlere başla
            if (tetiklenecekSes != null)
            {
                audioSource.clip = tetiklenecekSes;
                audioSource.volume = sesSeviyesi;
                audioSource.time = baslangicSaniyesi; // Sesi belirlediğin saniyeden başlat
                audioSource.Play();

                sureyiTakipEt = true; // Update içinde bitiş süresini kontrol etmeye başla
                dahaOnceCaldi = true; // "Bir kere çaldı" olarak işaretle
            }
            else
            {
                Debug.LogWarning("AudioTriggerSegment: Lütfen Inspector üzerinden bir ses dosyası atayın!");
            }
        }
    }

    void Update()
    {
        // Ses çalıyorsa ve süreyi takip etmemiz gerekiyorsa
        if (sureyiTakipEt && audioSource.isPlaying)
        {
            // Eğer sesin şu anki zamanı, belirlediğimiz bitiş saniyesine ulaştıysa veya geçtiyse
            if (audioSource.time >= bitisSaniyesi)
            {
                audioSource.Stop(); // Sesi durdur
                sureyiTakipEt = false; // Takibi bırak
            }
        }
    }
}