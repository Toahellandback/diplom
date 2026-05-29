using UnityEngine;

public class EnemyGround : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float jumpCooldown = 2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Edge Detection")]
    [SerializeField] private Transform edgeSensorL;
    [SerializeField] private Transform edgeSensorR;
    [SerializeField] private float edgeCheckDistance = 0.5f;

    [Header("Combat")]
    [SerializeField] private float attackDistance = 1.2f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackDamageDelay = 0.4f;
    [SerializeField] private int damage = 1;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform attackPoint;

    private Animator animator;
    private Rigidbody2D rb;
    private GroundSensor groundSensor;

    private bool playerDetected;
    private bool isDead;
    private bool isGrounded;
    private bool hasJumped; // флаг что уже прыгнули

    private float attackTimer;
    private float jumpTimer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        groundSensor = GetComponentInChildren<GroundSensor>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (attackPoint == null)
            attackPoint = transform.Find("AttackPoint");
    }

    private void Update()
    {
        if (isDead) return;

        bool wasGrounded = isGrounded;
        isGrounded = groundSensor != null && groundSensor.IsGrounded;

        // Сбрасываем флаг прыжка когда приземлились
        if (!wasGrounded && isGrounded)
            hasJumped = false;

        attackTimer -= Time.deltaTime;
        jumpTimer -= Time.deltaTime;

        if (player == null) { Idle(); return; }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackDistance)
            Attack();
        else if (playerDetected)
            ChasePlayer();
        else
            Idle();

        animator.SetFloat("AirSpeed", rb.linearVelocity.y);
        animator.SetBool("Grounded", isGrounded);
    }

    private void ChasePlayer()
    {
        // В воздухе — только анимация, без новых решений
        if (!isGrounded)
        {
            animator.SetInteger("AnimState", 0);
            float dir = player.position.x > transform.position.x ? 1f : -1f;
            rb.linearVelocity = new Vector2(
                Mathf.Lerp(rb.linearVelocity.x, dir * moveSpeed, 0.1f),
                rb.linearVelocity.y);
            return;
        }

        animator.SetInteger("AnimState", 2);
        float direction = player.position.x > transform.position.x ? 1f : -1f;

        // Разворот
        transform.localScale = direction > 0
            ? new Vector3(-1, 1, 1)
            : new Vector3(1, 1, 1);

        // Стена впереди
        if (IsTouchingWall(direction))
        {
            // Прыгаем через стену только один раз
            if (!hasJumped && player.position.y > transform.position.y + 0.3f)
                TryJump();
            else
                Idle();
            return;
        }

        // Край платформы
        if (IsEdgeAhead(direction))
        {
            // Прыгаем вниз только если игрок ниже
            if (!hasJumped && player.position.y < transform.position.y - 1f)
                TryJump();
            else
                Idle();
            return;
        }

        // Игрок на платформе выше — прыгаем один раз
        if (!hasJumped && player.position.y > transform.position.y + 1.5f)
        {
            TryJump();
            return;
        }

        // Обычное движение
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    private void TryJump()
    {
        if (!isGrounded || jumpTimer > 0) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpTimer = jumpCooldown;
        hasJumped = true;
    }

    private bool IsEdgeAhead(float direction)
    {
        Transform sensor = direction > 0 ? edgeSensorR : edgeSensorL;
        if (sensor == null) return false;

        RaycastHit2D hit = Physics2D.Raycast(
            sensor.position, Vector2.down,
            edgeCheckDistance, groundLayer);

        Debug.DrawRay(sensor.position, Vector2.down * edgeCheckDistance,
            hit.collider != null ? Color.green : Color.red);

        return hit.collider == null;
    }

    private bool IsTouchingWall(float direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            new Vector2(direction, 0),
            0.4f, groundLayer);

        Debug.DrawRay(transform.position,
            new Vector2(direction, 0) * 0.4f, Color.blue);

        return hit.collider != null;
    }

    private void Attack()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetInteger("AnimState", 0);

        if (attackTimer <= 0)
        {
            animator.SetTrigger("Attack");
            attackTimer = attackCooldown;
            Invoke(nameof(DealDamage), attackDamageDelay);
        }
    }

    public void DealDamage()
    {
        if (isDead || player == null) return;

        float dist = Vector2.Distance(
            transform.position, player.position);

        if (dist <= attackDistance + 0.5f)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(damage);
        }
    }

    private void Idle()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetInteger("AnimState", 0);
    }

    public void SetPlayerDetected(bool detected) => playerDetected = detected;
    public void SetPlayer(Transform target) => player = target;

    public void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Death");
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackDistance);
        }
        if (edgeSensorL != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(edgeSensorL.position, 0.1f);
        }
        if (edgeSensorR != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(edgeSensorR.position, 0.1f);
        }
    }
}