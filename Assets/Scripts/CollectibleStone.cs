using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CollectableStone : MonoBehaviour
{
    [Header("Etkileþim Ayarlarý")]
    public float interactionDistance = 3f;
    private Transform cameraTransform;

    [Header("Ses Ayarlarý")]
    [Tooltip("Toplama anýnda çalýnacak ses dosyasý")]
    public AudioClip collectSound;

    [Tooltip("Sesin tam olarak kaç saniye çalacaðýný buradan belirleyebilirsin")]
    public float playDuration = 2f;

    [Header("Silinecek Obje Ayarlarý")]
    [Tooltip("Taþ toplandýðýnda sahneden silinmesini istediðin objeyi buraya sürükle. Boþ býrakýrsan kendisini siler.")]
    public GameObject objectToDestroy;

    private AudioSource audioSource;
    private bool isCollected = false;

    void Start()
    {
        // Karakterin kamerasýný (bakýþ yönünü) buluyoruz
        if (Camera.main != null)
            cameraTransform = Camera.main.transform;

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // EÐÝTMEN DOKUNUÞU: Eðer Unity'de silinecek objeyi seçmeyi unutursan, kod çökmesin diye otomatik kendisini seçer.
        if (objectToDestroy == null)
        {
            objectToDestroy = gameObject;
        }
    }

    void Update()
    {
        // Eðer kamera yoksa veya taþ zaten toplandýysa boþuna yorulma
        if (cameraTransform == null || isCollected) return;

        // Sadece E tuþuna basýldýðýnda kontrol et
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckForStone();
        }
    }

    void CheckForStone()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // Baktýðýmýz obje bu taþ mý?
            if (hit.collider.gameObject == gameObject)
            {
                Collect();
            }
        }
    }

    void Collect()
    {
        isCollected = true; // Ýki kere týklanmasýný engeller

        // Menajeri bul ve ona BU TAÞIN KONUMUNU (transform.position) gönder!
        StoneManager manager = FindObjectOfType<StoneManager>();
        if (manager != null)
        {
            manager.CollectStone(transform.position);
        }

        // Taþý sahneden gizle ve etkileþimi kapat (Sesin çalabilmesi için obje hemen yok olmamalý)
        HideStone();

        // Sesi çalma ve taþý tamamen silme iþlemini baþlatan zamanlayýcýyý çalýþtýr
        StartCoroutine(PlaySoundAndDestroy());
    }

    IEnumerator PlaySoundAndDestroy()
    {
        // Sesi çal
        if (collectSound != null)
        {
            audioSource.clip = collectSound;
            audioSource.Play();
        }

        // Sesin bitmesini bekle
        yield return new WaitForSeconds(playDuration);

        // Ses hala çalýyorsa durdur
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // Eðer seçilen obje bu taþ deðilse (mesela bir üst obje seçildiyse) önce onu sil
        if (objectToDestroy != null && objectToDestroy != gameObject)
        {
            Destroy(objectToDestroy);
            Debug.Log(objectToDestroy.name + " isimli ek obje silindi.");
        }

        // En son bu scriptin olduðu asýl taþý silerek iþlemi bitir
        Destroy(gameObject);
    }

    void HideStone()
    {
        // Görünmez yap
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if (mesh != null) mesh.enabled = false;

        // Çarpýþmayý kapat (Ýçinden geçilebilsin ve bir daha alýnamasýn)
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Varsa taþýn içindeki diðer görselleri de gizle
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}