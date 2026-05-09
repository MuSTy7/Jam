using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    public Transform player;
    public Camera playerCamera;

    private NavMeshAgent agent;

    [Header("Settings")]
    public float stopDistance = 5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Oyuncunun dibine girmez
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
        }
        else
        {
            // Bakmýyorsa TAKÝP ET
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    bool IsPlayerLookingAtMonster()
    {
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        RaycastHit hit;

        // Kameranýn tam ortasýna bakýyor mu?
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