using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class InteractableStone : MonoBehaviour
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
    [Tooltip("Taþ toplandýðýnda sahneden silinmesini istediðin objeyi buraya sürükle.")]
    public GameObject objectToDestroy; // Ýnspektörden seçeceðin obje

    private AudioSource audioSource;
    private bool isCollected = false;

    void Start()
    {
        if (Camera.main != null)
            cameraTransform = Camera.main.transform;

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (cameraTransform == null || isCollected) return;

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
            if (hit.collider.gameObject == gameObject)
            {
                Collect();
            }
        }
    }

    void Collect()
    {
        isCollected = true;

        StoneManager manager = FindObjectOfType<StoneManager>();
        if (manager != null)
        {
            manager.CollectStone();
        }

        // SEÇÝLEN OBJEYÝ SÝLME ÝÞLEMÝ
        if (objectToDestroy != null)
        {
            Destroy(objectToDestroy);
            Debug.Log(objectToDestroy.name + " isimli obje baþarýyla silindi.");
        }

        // Taþý sahneden gizle ve etkileþimi kapat
        HideStone();

        // Sesi çalma ve taþý silme iþlemini baþlatan zamanlayýcýyý çalýþtýr
        StartCoroutine(PlaySoundAndDestroy());
    }

    IEnumerator PlaySoundAndDestroy()
    {
        if (collectSound != null)
        {
            audioSource.clip = collectSound;
            audioSource.Play();
        }

        yield return new WaitForSeconds(playDuration);

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        Destroy(gameObject);
    }

    void HideStone()
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if (mesh != null) mesh.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}