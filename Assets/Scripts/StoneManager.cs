using UnityEngine;
using System.Collections;

public class StoneManager : MonoBehaviour
{
    public int totalStones = 0;
    public int requiredStones = 3;
    public GameObject portal;

    [Header("Halüsinasyon (3. Taþýn Orada Beliren Karakter)")]
    public GameObject hallucinationMonster;

    private Transform playerTransform; // Oyuncunun yerini bilmeliyiz ki canavar ona baksýn

    void Start()
    {
        if (portal != null) portal.SetActive(false);
        if (hallucinationMonster != null) hallucinationMonster.SetActive(false);

        // Sahnede "Player" etiketli oyuncuyu bul
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    // Parametre olarak lokasyon (Vector3) alacak þekilde güncelledik
    public void CollectStone(Vector3 lastStonePosition)
    {
        totalStones++;
        Debug.Log("Taþ toplandý! Mevcut: " + totalStones);

        if (totalStones >= requiredStones)
        {
            OpenPortal(lastStonePosition);
        }
    }

    void OpenPortal(Vector3 spawnPos)
    {
        if (portal != null)
        {
            portal.SetActive(true);
            Debug.Log("Tüm taþlar toplandý, portal açýldý!");
        }

        // Halüsinasyonu son taþýn olduðu noktaya yolluyoruz
        if (hallucinationMonster != null)
        {
            StartCoroutine(FlashMonsterRoutine(spawnPos));
        }
    }

    IEnumerator FlashMonsterRoutine(Vector3 spawnPos)
    {
        // 1. Canavarý tam 3. taþýn olduðu koordinata taþý
        hallucinationMonster.transform.position = spawnPos;

        // 2. Canavarýn yönünü oyuncuya çevir (Doðrudan sana baksýn)
        if (playerTransform != null)
        {
            Vector3 lookPos = playerTransform.position;
            // Canavar yukarý/aþaðý eðilmesin, sadece kendi ekseni etrafýnda dönsün diye Y'sini sabitliyoruz
            lookPos.y = hallucinationMonster.transform.position.y;
            hallucinationMonster.transform.LookAt(lookPos);
        }

        // 3. Canavarý görünür yap
        hallucinationMonster.SetActive(true);

        // 4. 1.5 saniye bekle
        yield return new WaitForSeconds(1.5f);

        // 5. Canavarý tekrar gizle
        hallucinationMonster.SetActive(false);
    }
}