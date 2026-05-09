using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    public Transform player;
    public Camera playerCamera;

    private NavMeshAgent agent;
    private AudioSource audioSource;

    [Header("Settings")]
    public float stopDistance = 5f;

    [Header("Audio Settings")]
    public float sesBaslangic = 5.0f; // Sesin baþlayacaðý saniye
    public float sesBitis = 7.0f;     // Sesin baþa saracaðý saniye

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        agent.stoppingDistance = stopDistance;
    }

    void Update()
    {
        if (player == null || playerCamera == null)
            return;

        bool playerLooking = IsPlayerLookingAtMonster();

        if (playerLooking)
        {
            // Oyuncu bakýyorsa DUR
            agent.isStopped = true;

            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
        else
        {
            // Bakmýyorsa TAKÝP ET
            agent.isStopped = false;
            agent.SetDestination(player.position);

            // Sesi çalmaya baþla (Eðer çalmýyorsa ve baþlangýç saniyesinden baþla)
            if (!audioSource.isPlaying)
            {
                audioSource.time = sesBaslangic; // Sesi 5. saniyeye al
                audioSource.Play();
            }

            // DÖNGÜ KONTROLÜ: Ses çalýþýyorken 7. saniyeyi geçerse tekrar 5'e sar
            if (audioSource.isPlaying && audioSource.time >= sesBitis)
            {
                audioSource.time = sesBaslangic;
            }
        }
    }

    bool IsPlayerLookingAtMonster()
    {

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider.CompareTag("Monster"))
            {
                return true;
            }
        }

        return false;
    }
}