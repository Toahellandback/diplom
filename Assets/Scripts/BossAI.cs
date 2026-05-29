using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Health")]
    public int maxHealth = 20;
    private int currentHealth;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float followDistance = 8f;
    public float stopDistance = 2f;

    [Header("Attack")]
    public int attackDamage = 1;
    public float attackRange = 2f; 
    public float attackDistanceCheck = 2.2f; 
    public float attackCooldown = 2f;

    [Header("Spell")]
    public int spellDamage = 2;
    public float spellRange = 5f;
    public float spellCooldown = 6f;
    public GameObject spellPrefab;
    public Transform spellPoint;

    [Header("References")]
    public Transform attackPoint;

    private Rigidbody2D rb;
    private Animator animator;

    private float attackTimer = 0f;
    private float spellTimer = 3f;

    private bool isAttacking = false;
    private bool isCasting = false;
    private bool isDead = false;
    private bool spellReady = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (player == null)
        {
            GameObject hero = GameObject.FindGameObjectWithTag("Player");
            if (hero != null)
                player = hero.transform;
        }

        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hurt");
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        // ==========================================
        // 1. ВІДЛІК ТАЙМЕРІВ ЗАВЖДИ ПРАЦЮЄ ПЕРШИМ!
        // ==========================================
        if (attackTimer > 0) attackTimer -= Time.deltaTime;

        if (spellTimer > 0)
        {
            spellTimer -= Time.deltaTime;
            spellReady = false;
        }
        else
        {
            spellReady = true;
        }

        // ==========================================
        // 2. ПЕРЕВІРКА СТАНУ АТАКИ АБО КАСТУ
        // ==========================================
        // Якщо босс ЗАРАЗ махає мечем або чаклує — зупиняємо рух, 
        // але таймери (вище) вже успішно зменшилися на Time.deltaTime!
        if (isAttacking || isCasting)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            return;
        }

        // ==========================================
        // 3. ЛОГІКА ПОВЕДІНКИ (ПРАЦЮЄ ТІЛЬКИ КОЛИ БОСС ВІЛЬНИЙ)
        // ==========================================
        float distance = Vector2.Distance(transform.position, player.position);

        Flip();

        // SPELL
        if (spellReady && distance <= spellRange && distance > attackDistanceCheck)
        {
            CastSpell();
            return;
        }

        // ATTACK (тепер КД буде враховуватися чесно!)
        if (attackTimer <= 0f && distance <= attackDistanceCheck)
        {
            Attack();
            return;
        }

        // MOVE
        if (distance <= followDistance && distance > stopDistance)
            MoveToPlayer();
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("isWalking", false);
        }
    }

    void MoveToPlayer()
    {
        float direction = player.position.x > transform.position.x ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        animator.SetBool("isWalking", true);
    }

    void Flip()
    {
        if (isAttacking || isCasting) return;

        float deltaX = player.position.x - transform.position.x;

        // Мертвая зона 0.6 юнита, чтобы избежать дерганья и "телепортаций" вплотную
        if (Mathf.Abs(deltaX) < 0.6f) return;

        if (deltaX > 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-4, 4, 1);
        }
        else if (deltaX < 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(4, 4, 1);
        }
    }

    void Attack()
    {
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        attackTimer = attackCooldown;

        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isWalking", false);
        animator.SetTrigger("Attack");

        // Время до фактического удара (подстройте под вашу анимацию замаха)
        yield return new WaitForSeconds(0.4f);

        if (!isDead)
        {
            // Ищем ВСЕ объекты в радиусе атаки (более надежный способ без жесткой привязки к слоям)
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);

            foreach (Collider2D hit in hitObjects)
            {
                // Проверяем, что это игрок (по тегу)
                if (hit.CompareTag("Player"))
                {
                    PlayerHealth hp = hit.GetComponent<PlayerHealth>();
                    if (hp != null)
                    {
                        hp.TakeDamage(attackDamage);
                        break; // Урон нанесен, выходим из цикла
                    }
                }
            }
        }

        // Время до полного окончания анимации атаки
        yield return new WaitForSeconds(0.6f);

        isAttacking = false;
    }

    void CastSpell()
    {
        StartCoroutine(SpellRoutine());
    }

    IEnumerator SpellRoutine()
    {
        isCasting = true;
        spellReady = false;
        spellTimer = spellCooldown;

        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isWalking", false);

        animator.ResetTrigger("Cast");
        animator.ResetTrigger("Spell");
        animator.SetTrigger("Cast");

        // Чекаємо, поки босс замахнеться (підніме руку)
        yield return new WaitForSeconds(1.0f);

        if (isDead) { isCasting = false; yield break; }

        // Запускаємо анімацію кінця касту БОССА (не магії!)
        animator.SetTrigger("Spell");

        // Спавним снаряд магії. Він з'явиться і далі житиме своїм життям
        if (spellPrefab != null)
        {
            GameObject spell = Instantiate(spellPrefab);
            Vector3 spawnPos = spellPoint != null ? spellPoint.position : new Vector3(player.position.x, player.position.y + 2f, 0);
            spell.transform.position = spawnPos;
        }

        // Видаляємо WaitForSeconds(0.3f) та блок урону "hp.TakeDamage", 
        // тому що урон має наносити сам створений об'єкт spellPrefab при торканні об колайдер гравця!

        // Просто чекаємо, поки босс опустить руки і вийде зі стану касту
        yield return new WaitForSeconds(0.8f);

        isCasting = false;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Death");
        Destroy(gameObject, 3f);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, spellRange);
    }
}