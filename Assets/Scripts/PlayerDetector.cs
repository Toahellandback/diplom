using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    private EnemyGround enemy;

    private void Start()
    {
        enemy = GetComponentInParent<EnemyGround>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.SetPlayerDetected(true);
            enemy.SetPlayer(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.SetPlayerDetected(false);
        }
    }
}