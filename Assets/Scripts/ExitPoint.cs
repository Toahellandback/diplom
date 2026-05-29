using UnityEngine;

public class ExitPoint : MonoBehaviour
{
    [SerializeField] private bool requiresBossKill = false;

    private bool used = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        // Проверяем босса если нужно
        if (requiresBossKill)
        {
            BossAI boss = FindObjectOfType<BossAI>();
            if (boss != null)
            {
                Debug.Log("Убей босса сначала!");
                return;
            }
        }

        used = true;
        if (RoomManager.Instance != null)
            RoomManager.Instance.NextRoom();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = requiresBossKill ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}