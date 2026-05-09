using UnityEngine;

public class StoneManager : MonoBehaviour
{
    public int totalStones = 0; // Toplanan taþ sayýsý
    public int requiredStones = 3; // Gereken taþ sayýsý
    public GameObject portal; // Buraya portal objeni sürükleyeceksin

    void Start()
    {
        if (portal != null) portal.SetActive(false); // Baþta portal kapalý
    }

    public void CollectStone()
    {
        totalStones++;
        Debug.Log("Taþ toplandý! Mevcut: " + totalStones);

        if (totalStones >= requiredStones)
        {
            OpenPortal();
        }
    }

    void OpenPortal()
    {
        if (portal != null)
        {
            portal.SetActive(true);
            Debug.Log("Tüm taþlar toplandý. Portal açýldý!");
            // Buraya istersen bir ses veya görsel efekt ekleyebilirsin.
        }
    }
}