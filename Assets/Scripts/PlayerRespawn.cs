using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Iþýnlanma Ayarlarý")]
    public Transform spawnPoint; // Buraya Hierarchy'deki SpawnPoint objesini sürükle

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Bu fonksiyon, "Is Trigger" iþaretli bir objeye girdiðinde çalýþýr
    private void OnTriggerEnter(Collider other)
    {
        // Eðer çarptýðýmýz objenin adý "KillZone" ise
        if (other.gameObject.name == "KillZone")
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        if (spawnPoint != null && controller != null)
        {
            // ÝPUCU: CharacterController aktifken ýþýnlanma yapýlamaz. 
            // Önce kapatýyoruz, ýþýnlýyoruz, sonra tekrar açýyoruz.
            controller.enabled = false;

            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;

            controller.enabled = true;

            Debug.Log("Sistem Hatasý: Karakter baþlangýç noktasýna döndürüldü.");
        }
    }
}