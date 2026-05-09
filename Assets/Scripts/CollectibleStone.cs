using UnityEngine;

public class InteractableStone : MonoBehaviour
{
    public float interactionDistance = 3f;
    private Transform cameraTransform;

    void Start()
    {
        // Karakterin kamerasýný bulur (Bakýþ yönü için)
        if (Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (cameraTransform == null) return;

        // E tuþuna basýldýðýnda kontrol et
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckForStone();
        }
    }

    void CheckForStone()
    {
        // Kameradan ileriye doðru bir ýþýn fýrlatýyoruz
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        // Iþýn bir þeye çarptý mý ve çarptýðý þey BU taþ mý?
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
        StoneManager manager = FindObjectOfType<StoneManager>();
        if (manager != null)
        {
            manager.CollectStone();
        }

        Destroy(gameObject);
        Debug.Log("Sadece hedefteki taþ toplandý.");
    }
}