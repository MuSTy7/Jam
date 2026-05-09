using UnityEngine;

public class WeepingAngel : MonoBehaviour
{
    [Header("Hedef Ayarlarý")]
    public Transform player;
    public Camera playerCamera;

    [Header("Hareket Ayarlarý")]
    public float moveSpeed = 3f;
    public float stopDistance = 4f;
    public float turnSpeed = 4f; // YENÝ: Dönüþleri yumuþatýp daha ürkütücü yapacaðýz

    void Update()
    {
        if (player == null || playerCamera == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= stopDistance) return;

        if (!IsPlayerLookingAtMe())
        {
            MoveTowardsPlayer();
        }
    }

    bool IsPlayerLookingAtMe()
    {
        Vector3 viewportPoint = playerCamera.WorldToViewportPoint(transform.position);
        bool inScreenBounds = viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1 && viewportPoint.z > 0;

        if (inScreenBounds)
        {
            Vector3 directionToEnemy = (transform.position - playerCamera.transform.position).normalized;

            // ÇÖZÜM: Lazer bizim kendi karakterimize çarpmasýn diye tüm objeleri taratýyoruz
            RaycastHit[] hits = Physics.RaycastAll(playerCamera.transform.position, directionToEnemy, 100f);

            // Çarpan þeyler arasýnda DÜÞMAN var mý diye kontrol et
            foreach (RaycastHit hit in hits)
            {
                // Eðer çarptýðýmýz þey oyuncunun kendisiyse (veya görünmez bir tetikleyiciyse) bunu görmezden gel!
                if (hit.collider.isTrigger || hit.collider.CompareTag("Player"))
                    continue;

                // Eðer ilk çarptýðýmýz KATI cisim düþmansa, demek ki net bir þekilde görüyoruz
                if (hit.transform == this.transform)
                {
                    return true;
                }
                else
                {
                    // Düþmandan önce BAÞKA BÝR DUVARA çarptýysak, göremiyoruz demektir.
                    return false;
                }
            }
        }
        return false;
    }

    void MoveTowardsPlayer()
    {
        // YENÝ: Sadece X ve Z ekseninde oyuncuya doðru olan yönü bul
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Kapsülün boynunu eðip yeri öpmemesi için

        // Robot gibi anýnda dönmek yerine, yumuþak ve kavisli bir þekilde (Slerp) dön
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // Baktýðý yöne doðru (transform.forward) pürüzsüzce ilerle
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }
}