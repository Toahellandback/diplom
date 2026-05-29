using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    private int currentHealth;

    private Animator animator;

    private bool dead = false;

    private void Start()
    {
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (dead)
            return;

        currentHealth -= damage;

        // HURT ANIMATION
        animator.SetTrigger("Hurt");

        // DEATH
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        dead = true;

        // Остановить движение
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        // Отключить AI
        EnemyGround enemyAI = GetComponent<EnemyGround>();

        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }

        // Отключить коллайдер
        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
        {
            col.enabled = false;
        }

        // Запустить смерть
        animator.SetTrigger("Death");

        // Удалить позже
        Destroy(gameObject, 1.5f);
    }
}