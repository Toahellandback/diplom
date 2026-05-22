using UnityEngine;

public class EnemyGround : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Combat")]
    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int damage = 1;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform attackPoint;

    private Animator animator;
    private Rigidbody2D rb;

    private bool playerDetected;
    private bool isDead;

    private float attackTimer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isDead)
            return;

        if (player == null)
        {
            Idle();
            return;
        }

        attackTimer -= Time.deltaTime;

        float distance =
            Vector2.Distance(
                transform.position,
                player.position
            );

        // ATTACK
        if (distance <= attackDistance)
        {
            Attack();
        }
        // CHASE
        else if (playerDetected)
        {
            ChasePlayer();
        }
        // IDLE
        else
        {
            Idle();
        }

        animator.SetFloat("AirSpeed", rb.linearVelocity.y);
    }

    private void ChasePlayer()
    {
        animator.SetInteger("AnimState", 2);

        float direction =
            player.position.x > transform.position.x
            ? 1f
            : -1f;

        rb.linearVelocity =
            new Vector2(
                direction * moveSpeed,
                rb.linearVelocity.y
            );

        // Flip
        if (direction > 0)
            transform.localScale =
                new Vector3(-1, 1, 1);
        else
            transform.localScale =
                new Vector3(1, 1, 1);
    }

    private void Attack()
    {
        rb.linearVelocity =
            new Vector2(
                0,
                rb.linearVelocity.y
            );

        animator.SetInteger("AnimState", 0);

        if (attackTimer <= 0)
        {
            animator.SetTrigger("Attack");

            attackTimer = attackCooldown;
        }
    }

    private void Idle()
    {
        rb.linearVelocity =
            new Vector2(
                0,
                rb.linearVelocity.y
            );

        animator.SetInteger("AnimState", 0);
    }

    public void SetPlayerDetected(bool detected)
    {
        playerDetected = detected;
    }

    public void SetPlayer(Transform target)
    {
        player = target;
    }

    public void DealDamage()
    {
        Collider2D hit =
            Physics2D.OverlapCircle(
                attackPoint.position,
                attackDistance,
                LayerMask.GetMask("Player")
            );

        if (hit != null)
        {
            PlayerHealth health =
                hit.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }

    public void Die()
    {
        isDead = true;

        rb.linearVelocity = Vector2.zero;

        animator.SetTrigger("Death");
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackDistance
        );
    }
}